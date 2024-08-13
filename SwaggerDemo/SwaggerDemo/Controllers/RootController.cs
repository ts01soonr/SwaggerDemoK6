using Microsoft.AspNetCore.Mvc;
using System.Net;
using SwaggerDemo.Helper;

namespace SwaggerDemo.Controllers
{
    [Route("/")]
    public class RootController : ControllerBase
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
    }
}
