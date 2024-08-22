using Microsoft.AspNetCore.Mvc;
using System.Net;
using SwaggerDemo.Helper;
using SwaggerDemo.Model;
using System.Text.Json;

namespace SwaggerDemo.Controllers
{
    [Route("/")]
    public class ProcessInfoController : ControllerBase
    {
        private readonly string _defaultURL = Sys.isDevelopment ? "/swagger/index.html" : "/api/v2";

        [HttpGet]
        public IActionResult Index()
        {
            return new ContentResult
            {
                ContentType = "text/html",
                StatusCode = (int)HttpStatusCode.OK,
                Content = @"<!DOCTYPE html>
                            <html lang='en'>
                            <head>
                                <meta charset='UTF-8'>
                                <title>Redirecting...</title>
                                <script type='text/javascript'>
                                    window.location.href = '{URL}';
                                </script>
                            </head>
                            <body>
                                <p>If you are not redirected automatically, follow this <a href='{URL}'>link to the home page</a>.</p>
                            </body>
                            </html>".Replace("{URL}", _defaultURL)

            };
        }
        [HttpGet("{name}")]
        public ActionResult Get(string? name="info")
        {
            //default: info = GetCurrentProcess
            ProcessInfo psInfo = Sys.Get_CPU_Memory(name);
            string jsonString = JsonSerializer.Serialize(psInfo);
            return Ok(jsonString);
        }
        [HttpPost("{package}")]
        public ActionResult Post(string? package = "A,2:B,3")
        {
            //default package = "A,2:B,3"
            PackageInfo result = Sys.VerifyPackage(package);
            return Ok(JsonSerializer.Serialize(result));
        }
    }
}
