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
        public UploadContext(IFormFile file, string webrootPath, string uploadFolder) : this(file, webrootPath, uploadFolder, null)
        {
            //
        }

        public UploadContext(IFormFile file, string webrootPath, string uploadFolder, string uploadName)
        {
            this.FormFile = file;
            this.UploadFolder = uploadFolder;
            this.UploadName = uploadName;
        }

        /// <summary>
        /// 文件对象
        /// </summary>
        public IFormFile FormFile { get; private set; }

        /// <summary>
        /// 站点根目录
        /// </summary>
        public string WebRootPath { get; set; }

        /// <summary>
        /// 文件保存的文件夹路径
        /// </summary>
        public string UploadFolder { get; private set; }

        /// <summary>
        /// 保存的文件名（不包含扩展名，为空时默认自动生成）
        /// </summary>
        public string UploadName { get; private set; }
    }
}
