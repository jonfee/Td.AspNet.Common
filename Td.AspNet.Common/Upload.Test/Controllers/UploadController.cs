using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.Net.Http.Headers;
using Td.AspNet.Upload;

// For more information on enabling MVC for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace Upload.Test.Controllers
{
    public class UploadController : Controller
    {
        // GET: /<controller>/
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Index(int a)
        {
            var files = HttpContext.Request.Form.Files;

            UploadContext upContext = new UploadContext("http://localhost:27102/api/uploadify", files, null, 0, "/upload/test", false, "abc", null, false, 100L, 720, 720, false);

            var result = upContext.Save().Result;

            string paths = string.Join(",", result.Select(p => p.FilePath).ToArray());

            return Content(paths);
        }
    }
}
