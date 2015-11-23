using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Td.AspNet.WebApi
{
    public class FileContent
    {
        public string FileName { get; set; }

        public Stream FileSteam { get; set; }
    }
}
