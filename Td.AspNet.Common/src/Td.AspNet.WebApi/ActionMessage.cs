using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Mvc;
using Microsoft.Net.Http.Headers;

namespace Td.AspNet.WebApi
{
    public class ActionMessage
    {
        public static IActionResult Ok(ErrorMessage message)
        {
            var result = new HttpOkObjectResult("api");
            result.Value = Newtonsoft.Json.JsonConvert.SerializeObject(message);
            result.StatusCode = 200;
            result.ContentTypes.Add(new MediaTypeHeaderValue("application/json"));
            return result;
        }
    }
}
