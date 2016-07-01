using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Td.AspNet.Upload
{
    public static class FileSave
    {
        public static Task<UploadResult> SaveAsAsync(this Stream stream, string filepath)
        {
            using (FileStream fs = new FileStream(filepath, FileMode.Create))
            {
                byte[] bytes = new byte[stream.Length];
                int numBytesRead = 0;
                int numBytesToRead = (int)stream.Length;
                stream.Position = 0;
                while (numBytesToRead > 0)
                {
                    int n = stream.Read(bytes, numBytesRead, Math.Min(numBytesToRead, int.MaxValue));
                    if (n <= 0)
                    {
                        break;
                    }
                    fs.Write(bytes, numBytesRead, n);
                    numBytesRead += n;
                    numBytesToRead -= n;
                }
                fs.Dispose();
                return null;
            }
        }
    }
}
