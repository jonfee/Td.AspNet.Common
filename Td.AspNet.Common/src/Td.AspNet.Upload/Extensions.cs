using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System;

namespace Td.AspNet.Upload
{
    /// <summary>
    /// 文件上传扩展类
    /// </summary>
    public static class Extensions
    {
        /// <summary>
        /// 添加FormFile文件流
        /// </summary>
        /// <param name="content"></param>
        /// <param name="file"></param>
        /// <param name="limitSize"></param>
        /// <param name="filter"></param>
        /// <param name="error"></param>
        /// <returns></returns>
        private static MultipartFormDataContent AddFormFileContent(this MultipartFormDataContent content, FormFileContent file, long limitSize, string[] filter, out string error)
        {
            error = string.Empty;

            #region //文件对象不存在时，忽略
            if (null == file || file.FormFile == null || file.FileSize < 1)
            {
                error = "文件对象不存在";
            }
            #endregion
            #region//文件大小限制检测
            else if (limitSize > 0 && file.FileSize > limitSize * 1024)
            {
                error = string.Format("上传文件最大不能超过{0}KB", limitSize);
            }
            #endregion
            #region//文件格式限制检测
            else if (filter != null && filter.Length > 0)
            {
                //当前文件扩展名
                string currentExt = Path.GetExtension(file.FormFile.FileName).ToLower();

                //非有效文件格式时，返回提示
                if (!filter.Contains(currentExt))
                {
                    error = string.Format("上传文件类型失败，限制为{0}", string.Join("|", filter));
                }
            }
            #endregion

            #region//文件正常，则添加文件对象到Post数据
            if (string.IsNullOrWhiteSpace(error))
            {
                StreamContent fileContent = new StreamContent(file.FormFile.OpenReadStream());//文件流
                fileContent.Headers.ContentType = new MediaTypeHeaderValue(file.ContentType);//文件内容类型
                fileContent.Headers.ContentDisposition = new ContentDispositionHeaderValue("form-data");//form-data表示一个表单对象数据
                fileContent.Headers.ContentDisposition.FileName = file.FormFile.FileName;//文件名
                fileContent.Headers.ContentDisposition.Name = file.FieldName;//文件标识字段名称

                content.Add(fileContent);
            }
            #endregion

            return content;
        }

        /// <summary>
        /// 添加Base64文件流
        /// </summary>
        /// <param name="content"></param>
        /// <param name="file"></param>
        /// <param name="filedName">标志字段名</param>
        /// <param name="limitSize"></param>
        /// <param name="error"></param>
        /// <returns></returns>
        private static MultipartFormDataContent AddBase64FileContent(this MultipartFormDataContent content, Base64FileContent file, long limitSize, out string error)
        {
            error = string.Empty;

            #region //文件对象不存在时，忽略
            if (null == file || file.Data == null || file.FileSize < 1)
            {
                error = "文件对象不存在";
            }
            #endregion
            #region//文件大小限制检测
            else if (limitSize > 0 && file.FileSize > limitSize * 1024)
            {
                error = string.Format("上传文件最大不能超过{0}KB", limitSize);
            }
            #endregion

            #region//文件正常，则添加文件对象到Post数据
            if (string.IsNullOrWhiteSpace(error))
            {
                content.Add(new StringContent(file.Data), file.FieldName);
            }
            #endregion

            return content;
        }

        /// <summary>
        /// 保存文件
        /// </summary>
        /// <param name="context">上传文件上下文</param>
        /// <returns></returns>
        public static async Task<List<UploadResult>> Save(this UploadContext context)
        {
            if (string.IsNullOrWhiteSpace(context.ApiAddress)) throw new ArgumentException("请指定上传接口URL", nameof(context.ApiAddress));

            List<UploadResult> result = new List<UploadResult>();

            //文件上传对象不存在时，返回null
            if (null == context || context.FileList.Count < 1) return null;

            //处理文件上传
            using (var postContent = new MultipartFormDataContent())
            {
                //实际有效上传的文件数
                int effectCount = 0;

                #region 1、//遍历上传文件对象
                foreach (var fc in context.FileList)
                {
                    #region//预定义上传结果
                    var itemRst = new UploadResult
                    {
                        FieldName = fc.FieldName,
                        FileName = context.UploadName,
                        FilePath = "",
                        FileSize = fc.FileSize,
                        ContentType = fc.ContentType,
                        IsImage = fc.IsImage,
                        Message = ""
                    };
                    #endregion

                    string error = null;

                    //FormFile 对象流
                    if (fc is FormFileContent)
                    {
                        var file = (FormFileContent)fc;

                        postContent.AddFormFileContent(file, context.MaxSize, context.Filter, out error);
                    }
                    //Base64图片文件流
                    else if (fc is Base64FileContent)
                    {
                        var base64 = (Base64FileContent)fc;

                        postContent.AddBase64FileContent(base64, context.MaxSize, out error);
                    }

                    if (string.IsNullOrWhiteSpace(error))
                    {
                        effectCount++;
                    }

                    result.Add(itemRst);
                }

                #endregion

                #region 2、//如果存在有效文件上传，则请求到上传接口
                if (effectCount > 0)
                {
                    #region 加入其它参数及值到Post数据
                    postContent.Add(new StringContent(context.UploadFolder), "savepath");
                    postContent.Add(new StringContent(context.IsFixedPath.ToString()), "fixedpath");
                    postContent.Add(new StringContent(context.UploadName), "name");
                    postContent.Add(new StringContent(context.SaveExtension), "extension");
                    postContent.Add(new StringContent(context.BeOverride.ToString()), "beOverride");
                    postContent.Add(new StringContent(context.CompressIfGreaterSize.ToString()), "compressIfGreaterSize");
                    postContent.Add(new StringContent(context.MaxWidth.ToString()), "maxWidth");
                    postContent.Add(new StringContent(context.MaxHeight.ToString()), "maxHeight");
                    postContent.Add(new StringContent(context.CutIfOut.ToString()), "cutIfOut");
                    #endregion

                    //定义一个上传文件返回结果变量
                    List<UploadBackResult> backData = null;

                    #region//文件Post上传
                    using (HttpClient client = new HttpClient())
                    {
                        var response = await client.PostAsync(context.ApiAddress, postContent);
                        //确保HTTP成功状态值
                        response.EnsureSuccessStatusCode();

                        string strContent = await response.Content.ReadAsStringAsync();

                        try
                        {
                            backData = JsonConvert.DeserializeObject<List<UploadBackResult>>(strContent) ?? new List<UploadBackResult>();
                        }
                        catch
                        {
                            backData = new List<UploadBackResult>();
                        }
                    }
                    #endregion

                    #region//上传结果处理
                    foreach (var rst in result)
                    {
                        var upRst = backData.Where(p => p.FieldName == rst.FieldName).FirstOrDefault();

                        if (null != upRst)
                        {
                            rst.FilePath = upRst.FilePath;
                            rst.Message = upRst.Message;
                        }
                    }
                    #endregion
                }
                #endregion
            }

            return result;
        }

        /// <summary>
        /// 保存文件队列
        /// </summary>
        /// <param name="queueContext">上传文件队列</param>
        /// <returns></returns>
        public static async Task<List<UploadResult>> Save(this IEnumerator<UploadContext> queueContext)
        {
            List<UploadResult> list = new List<UploadResult>();

            while (queueContext.MoveNext())
            {
                var result = await queueContext.Current.Save();

                if (null != result) list.AddRange(result);
            }

            return list;
        }

        /// <summary>
        /// 保存文件队列
        /// </summary>
        /// <param name="queueContext">上传文件队列</param>
        /// <returns></returns>
        public static async Task<List<UploadResult>> Save(this IEnumerable<UploadContext> queueContext)
        {
            List<UploadResult> list = new List<UploadResult>();

            if (null != queueContext && queueContext.Count() > 0)
            {
                foreach (var context in queueContext)
                {
                    var result = await context.Save();

                    if (null != result) list.AddRange(result);
                }
            }

            return list;
        }
    }
}
