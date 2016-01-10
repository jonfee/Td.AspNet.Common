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
        /// <summary>
        /// 实始化文件上传内容实例（自动随机生成文件名称）
        /// </summary>
        /// <param name="file">待上传的文件</param>
        /// <param name="webrootPath">需保存的文件根路径</param>
        /// <param name="uploadFolder">需保存的文件夹路径（从根路径后）</param>
        public UploadContext(IFormFile file, string webrootPath, string uploadFolder) : this(file, webrootPath, uploadFolder, null)
        {
            //
        }

        /// <summary>
        /// 实始化文件上传内容实例（存在同名文件时不覆盖）
        /// </summary>
        /// <param name="file">待上传的文件</param>
        /// <param name="webrootPath">需保存的文件根路径</param>
        /// <param name="uploadFolder">需保存的文件夹路径（从根路径后）</param>
        /// <param name="uploadName">重命名后的文件名称（为null时随机生成）</param>
        public UploadContext(IFormFile file, string webrootPath, string uploadFolder, string uploadName) : this(file, webrootPath, uploadFolder, uploadName, null)
        {
            //
        }

        /// <summary>
        /// 实始化文件上传内容实例
        /// </summary>
        /// <param name="file">待上传的文件</param>
        /// <param name="webrootPath">需保存的文件根路径</param>
        /// <param name="uploadFolder">需保存的文件夹路径（从根路径后）</param>
        /// <param name="uploadName">重命名后的文件名称（为null时随机生成）</param>
        /// <param name="extensionName">扩展名（如：jpg）</param>
        public UploadContext(IFormFile file, string webrootPath, string uploadFolder, string uploadName, string extensionName) : this(file, webrootPath, uploadFolder, uploadName, extensionName, false)
        {
            //
        }

        /// <summary>
        /// 实始化文件上传内容实例
        /// </summary>
        /// <param name="file">待上传的文件</param>
        /// <param name="webrootPath">需保存的文件根路径</param>
        /// <param name="uploadFolder">需保存的文件夹路径（从根路径后）</param>
        /// <param name="uploadName">重命名后的文件名称（为null时随机生成）</param>
        /// <param name="extensionName">扩展名（如：jpg）</param>
        /// <param name="beOverride">存在同名文件时是否覆盖</param>
        public UploadContext(IFormFile file, string webrootPath, string uploadFolder, string uploadName, string extensionName, bool beOverride)
        {
            this.FormFile = file;
            this.UploadFolder = uploadFolder;
            this._uploadFileName = uploadName;
            this._extension = extensionName;
            this.WebRootPath = webrootPath;
            this.BeOverride = BeOverride;
        }

        /// <summary>
        /// 文件扩展名
        /// </summary>
        private string _extension = null;

        /// <summary>
        /// 文件上传后保存的文件名
        /// </summary>
        private string _uploadFileName = null;

        /// <summary>
        /// 文件媒体类型
        /// </summary>
        private string _contentType = null;

        /// <summary>
        /// 文件对象
        /// </summary>
        public IFormFile FormFile { get; private set; }

        /// <summary>
        /// 站点根目录
        /// </summary>
        public string WebRootPath { get; private set; }

        /// <summary>
        /// 文件保存的文件夹路径
        /// </summary>
        public string UploadFolder { get; private set; }

        /// <summary>
        /// 存在同名文件时覆盖
        /// </summary>
        public bool BeOverride { get; private set; }

        #region 只读属性

        /// <summary>
        /// 原始文件名
        /// </summary>
        public string OriginalName
        {
            get
            {
                return null != HeaderValue ? HeaderValue.FileName.Replace("\"", "") : string.Empty;
            }
        }

        /// <summary>
        /// 保存的文件名（包含扩展名）
        /// </summary>
        public string UploadName
        {
            get
            {
                //保存后的文件名称
                var newFileName = _uploadFileName;

                if (string.IsNullOrWhiteSpace(newFileName))
                {
                    string rnd = GetRandomCode(10);
                    newFileName = string.Format("{0}{1}", DateTime.Now.ToString("HHmmss"), rnd);
                }

                return newFileName + ExtensionName;
            }
        }

        /// <summary>
        /// 保存的文件扩展名（如：jpg）
        /// </summary>
        public string ExtensionName
        {
            get
            {
                string extension = _extension;

                if (string.IsNullOrWhiteSpace(extension))
                {
                    extension = Path.GetExtension(OriginalName);
                }
                if (!extension.StartsWith(".")) extension = "." + extension;

                return extension;
            }
        }

        /// <summary>
        /// 文件类型
        /// </summary>
        public string ContentType
        {
            get
            {
                if (string.IsNullOrWhiteSpace(_contentType))
                {
                    //文件内容信息
                    if (null != HeaderValue && null != HeaderValue.Parameters)
                    {
                        foreach (var p in HeaderValue.Parameters)
                        {
                            if (p.Name.Equals("ContentType", StringComparison.OrdinalIgnoreCase))
                            {
                                _contentType = p.Value.Replace("-", "/");
                                break;
                            }
                        }
                    }

                    if (string.IsNullOrWhiteSpace(_contentType))
                    {
                        _contentType = FileContentType.GetMimeType(ExtensionName);
                    }

                    if (string.IsNullOrWhiteSpace(_contentType))
                    {
                        _contentType = FormFile.ContentType;
                    }
                }

                return _contentType;
            }
        }

        /// <summary>
        /// 文件标志名
        /// </summary>
        public string FiledName
        {
            get
            {
                return null != HeaderValue ? HeaderValue.Name : string.Empty;
            }
        }

        /// <summary>
        /// 是否为图片
        /// </summary>
        public bool IsImage
        {
            get
            {
                if (!string.IsNullOrWhiteSpace(ContentType))
                {
                    return ContentType.StartsWith("image/", StringComparison.OrdinalIgnoreCase);
                }

                return false;
            }
        }

        /// <summary>
        /// ContentDispositionHeaderValue
        /// </summary>
        public ContentDispositionHeaderValue HeaderValue
        {
            get
            {
                return ContentDispositionHeaderValue.Parse(FormFile.ContentDisposition);
            }
        }

        #endregion

        #region 方法

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

        #endregion
    }
}
