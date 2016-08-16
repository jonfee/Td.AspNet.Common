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
        /// 保存文件
        /// </summary>
        /// <param name="context">上传文件上下文</param>
        /// <returns></returns>
        public static async Task<List<UploadResult>> Save(this UploadContext context)
        {
            if (string.IsNullOrWhiteSpace(context.ApiAddress)) throw new ArgumentException("请指定上传接口URL", nameof(context.ApiAddress));

            List<UploadResult> result = new List<UploadResult>();

            //文件上传对象不存在时，返回null
            if (null == context || context.FormFileList.Count < 1) return null;

            //处理文件上传
            using (var postContent = new MultipartFormDataContent())
            {
                //遍历上传文件对象
                foreach (var fc in context.FormFileList)
                {
                    #region//预定义上传结果
                    var itemRst = new UploadResult
                    {
                        FieldName = fc.FieldName,
                        FileName = context.UploadName,
                        FilePath = "",
                        FileSize = fc.FormFile.Length,
                        ContentType = fc.ContentType,
                        IsImage = fc.IsImage,
                        Message = ""
                    };
                    #endregion
                    #region //文件对象不存在时，忽略
                    if (null == fc || fc.FormFile == null || fc.FormFile.Length < 1)
                    {
                        itemRst.Message = "文件对象不存在";
                    }
                    #endregion
                    #region//文件格式限制检测
                    else if (context.Filter != null && context.Filter.Length > 0)
                    {
                        //当前文件扩展名
                        string currentExt = Path.GetExtension(fc.FormFile.FileName).ToLower();

                        //非有效文件格式时，返回提示
                        if (!context.Filter.Contains(currentExt))
                        {
                            itemRst.Message = string.Format("上传文件类型失败，限制为{0}", string.Join("|", context.Filter));
                        }
                    }
                    #endregion
                    #region//文件大小限制检测
                    else if (context.MaxSize > 0 && fc.FormFile.Length > context.MaxSize * 1024)
                    {
                        itemRst.Message = string.Format("上传文件最大不能超过{0}KB", context.MaxSize);
                    }
                    #endregion
                    #region//文件正常，则添加文件对象到Post数据
                    else
                    {
                        StreamContent fileContent = new StreamContent(fc.FormFile.OpenReadStream());//文件流
                        fileContent.Headers.ContentType = new MediaTypeHeaderValue(fc.FormFile.ContentType);//文件内容类型
                        fileContent.Headers.ContentDisposition = new ContentDispositionHeaderValue("form-data");//form-data表示一个表单对象数据
                        fileContent.Headers.ContentDisposition.FileName = fc.FormFile.FileName;//文件名
                        fileContent.Headers.ContentDisposition.Name = fc.FieldName;//文件标识字段名称

                        postContent.Add(fileContent);
                    }
                    #endregion

                    result.Add(itemRst);
                }

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
