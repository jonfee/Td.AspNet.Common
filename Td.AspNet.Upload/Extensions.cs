using Microsoft.AspNet.Http;
using Microsoft.Net.Http.Headers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

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
        public static async Task<UploadResult> Save(this UploadContext context)
        {
            UploadResult result = null;

            if (null != context && context.FormFile.Length > 0 && !string.IsNullOrWhiteSpace(context.WebRootPath))
            {
                var file = context.FormFile;

                //保存后的文件名称
                var newFileName = context.UploadName;

                if (string.IsNullOrWhiteSpace(newFileName))
                {
                    string rnd = GetRandomCode(10);
                    newFileName = string.Format("{0}{1}", DateTime.Now.ToString("HHmmss"), rnd);
                }

                //文件内容信息
                var parsedContentDisposition =
                      ContentDispositionHeaderValue.Parse(file.ContentDisposition);

                //原始文件名称
                var originalName = parsedContentDisposition.FileName.Replace("\"", "");

                //文件全名（含扩展名）
                newFileName = newFileName + Path.GetExtension(originalName);

                //文件上传的绝对文件夹目录，如：
                var uploadFullFolder = context.WebRootPath;

                //文件相对站点的路径，默认为文件名，如：aa.jpg
                string relFilePath = newFileName;

                if (!string.IsNullOrWhiteSpace(context.UploadFolder))
                {
                    //更新最终上传文件的目录路径，如：D:\\WebSite\\wwwroot\\upload\\photo
                    uploadFullFolder = Path.Combine(context.WebRootPath, context.UploadFolder);

                    //更新文件相对站点的路径，如：\\upload\\photo\\aa.jpg
                    relFilePath = Path.Combine(context.UploadFolder, newFileName);
                }

                //文件在磁盘中的路径，如：D:\\WebSite\\wwwroot\\upload\\photo\\aa.jpg
                string absFileName = Path.Combine(context.WebRootPath, relFilePath);

                //文件夹不存在,则创建
                if (!Directory.Exists(context.UploadFolder))
                {
                    Directory.CreateDirectory(context.UploadFolder);
                }

                await context.FormFile.SaveAsAsync(absFileName);

                result = new UploadResult
                {
                    FieldName = parsedContentDisposition.Name,
                    FileName = newFileName,
                    FilePath = relFilePath.Replace(@"\", @"/"),
                    FileSize = file.Length,
                    ContentType = file.ContentType,
                    IsImage = file.ContentType.IsImage()
                };
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

                if (null != result) list.Add(result);
            }

            return list;
        }

        /// <summary>
        /// 判断文件类型是否为图片
        /// </summary>
        /// <param name="contentType"></param>
        /// <returns></returns>
        private static bool IsImage(this string contentType)
        {
            if (!string.IsNullOrWhiteSpace(contentType))
            {
                return contentType.ToLower().StartsWith("image/");
            }

            return false;
        }

        /// <summary>
        /// 生成随机码
        /// </summary>
        /// <param name="length"></param>
        /// <returns></returns>
        private static string GetRandomCode(int length)
        {
            string chars = "abcdefghijklmnopqrstuvwxyz0123456789";

            StringBuilder sb = new StringBuilder(length);

            for (var i = 0; i < length; i++)
            {
                int index = new Random(Guid.NewGuid().GetHashCode()).Next(0, chars.Length);

                sb.Append(chars[index]);
            }

            return sb.ToString();
        }

        /// <summary>
        /// 流转换为byte数组
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        private static byte[] ConvertToByteBuffer(this Stream stream)
        {
            int b;
            System.IO.MemoryStream tempStream = new System.IO.MemoryStream();
            while ((b = stream.ReadByte()) != -1)
            {
                tempStream.WriteByte(((byte)b));
            }
            return tempStream.ToArray();
        }
    }
}
