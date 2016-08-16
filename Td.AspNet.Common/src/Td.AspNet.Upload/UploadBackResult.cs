using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Td.AspNet.Upload
{
    /// <summary>
    /// 上传后回传的结果
    /// </summary>
    public class UploadBackResult
    {
        /// <summary>
        /// 文件标记字段名称
        /// </summary>
        public string FieldName { get; set; }

        /// <summary>
        /// 上传后的文件路径
        /// </summary>
        public string FilePath { get; set; }

        /// <summary>
        /// 消息
        /// </summary>
        public string Message { get; set; }
    }
}
