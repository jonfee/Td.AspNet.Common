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
        public IActionResult Index(IFormFile files)
        {
            UploadContext upContext = new UploadContext(files, Startup.UploadRoot, "base64", null, null, true, SaveAfterOutputType.Base64);

            var result = upContext.Save().Result;

            return Content(result.Base64Content);
        }
    }
}
