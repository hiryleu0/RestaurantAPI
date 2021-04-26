using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace RestaurantAPI_ASP.NET_Core.Controllers
{
    [Route("file")]
    [Authorize]
    public class FileController:ControllerBase
    {
        [HttpGet]
        public ActionResult GetFile([FromQuery] string filename)
        {
            var rootPath = Directory.GetCurrentDirectory();
            var filePath = $"{rootPath}/PrivateFiles/{filename}";
            var fileExists = System.IO.File.Exists(filePath);

            if(!fileExists)
            {
                return NotFound();
            }

            FileExtensionContentTypeProvider contentProvider = new FileExtensionContentTypeProvider();
            contentProvider.TryGetContentType(filename, out string contentType);

            var fileContents = System.IO.File.ReadAllBytes(filePath);

            return File(fileContents, contentType, filename);
        }

        [HttpPost]
        public ActionResult Upload([FromForm] IFormFile file)
        {
            if(file != null && file.Length > 0)
            {
                var rootPath = Directory.GetCurrentDirectory();
                var filePath = $"{rootPath}/PrivateFiles/{file.FileName}";
                var fileExists = System.IO.File.Exists(filePath);

                using(var stream = new FileStream(filePath, FileMode.CreateNew))
                {
                    file.CopyTo(stream);
                }

                return Ok();
            }

            return BadRequest();
        }
    }
}
