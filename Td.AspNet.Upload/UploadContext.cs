using Microsoft.AspNet.Http;
using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Net.Http.Headers;

namespace Td.AspNet.Upload
{
    /// <summary>
    /// 上传的文件内容
    /// </summary>
    public class UploadContext
    {
        public UploadContext(IFormFile file, string uploadFolder) : this(file, uploadFolder, null)
        {
            //
        }

        public UploadContext(IFormFile file, string uploadFolder, string uploadFileName)
        {
            this.FormFile = file;
            this.UploadFileFolder = uploadFolder;
            this.UploadFileName = uploadFileName;
        }

        /// <summary>
        /// 文件对象
        /// </summary>
        public IFormFile FormFile { get; private set; }

        /// <summary>
        /// 文件保存的文件夹路径
        /// </summary>
        public string UploadFileFolder { get; private set; }

        /// <summary>
        /// 保存的文件名（不包含扩展名，为空时默认自动生成）
        /// </summary>
        public string UploadFileName { get; private set; }

        /// <summary>
        /// 保存文件
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public async Task<UploadResult> Save()
        {
            return await this.Save();
        }

        /// <summary>
        /// 判断文件类型是否为图片
        /// </summary>
        /// <param name="contentType"></param>
        /// <returns></returns>
        private bool IsImage(string contentType)
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
        private string GetRandomCode(int length)
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
    }
}
