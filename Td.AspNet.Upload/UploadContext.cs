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
        public UploadContext(IFormFile file, string webrootPath, string uploadFolder) : this(file, webrootPath, uploadFolder, null, false)
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
        public UploadContext(IFormFile file, string webrootPath, string uploadFolder, string uploadName) : this(file, webrootPath, uploadFolder, uploadName, false)
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
        /// <param name="beOverride">存在同名文件时是否覆盖</param>
        public UploadContext(IFormFile file, string webrootPath, string uploadFolder, string uploadName, bool beOverride)
        {
            this.FormFile = file;
            this.UploadFolder = uploadFolder;
            this.UploadName = uploadName;
            this.WebRootPath = webrootPath;
            this.BeOverride = BeOverride;
        }

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
        /// 保存的文件名（不包含扩展名，为空时默认自动生成）
        /// </summary>
        public string UploadName { get; private set; }

        /// <summary>
        /// 存在同名文件时覆盖
        /// </summary>
        public bool BeOverride { get; private set; }
    }
}
