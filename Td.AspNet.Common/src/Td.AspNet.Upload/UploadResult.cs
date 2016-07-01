using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Td.AspNet.Upload
{
    /// <summary>
    /// 文件上传结果类
    /// </summary>
    public class UploadResult
    {
        /// <summary>
        /// 表单字段名
        /// </summary>
        public string FieldName { get; set; }

        /// <summary>
        /// 原始文件名
        /// </summary>
        public string OriginalFileName { get; set; }

        /// <summary>
        /// 文件大小
        /// </summary>
        public long FileSize { get; set; }

        /// <summary>
        /// 上传后的文件名
        /// </summary>
        public string FileName { get; set; }

        /// <summary>
        /// 文件类型
        /// </summary>
        public string ContentType { get; set; }

        /// <summary>
        /// 是否为图片
        /// </summary>
        public bool IsImage { get; set; }

        /// <summary>
        /// 上传后的文件路径
        /// </summary>
        public string FilePath { get; set; }
    }
}
