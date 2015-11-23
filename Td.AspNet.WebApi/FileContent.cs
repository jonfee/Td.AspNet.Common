using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Td.AspNet.WebApi
{
    public class FileContent
    {
        /// <summary>
        /// 表单字段名
        /// </summary>
        public string FieldName { get; set; }

        /// <summary>
        /// 文件名
        /// </summary>
        public string FileName { get; set; }

        /// <summary>
        /// 文件流
        /// </summary>
        public Stream FileStream { get; set; }
    }
}
