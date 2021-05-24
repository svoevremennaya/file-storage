using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.Logging;

namespace file_storage.Controllers
{
    [ApiController]
    [Route("FileStorage")]
    public class FileStorageController : ControllerBase
    {
        private readonly string storagePath = @"C:\FileStorage";

        [HttpPut("{*path}")]
        public ActionResult Put(string path)
        {
            string fullPath = FileStorage.GetFullPath(path);

            IFormFileCollection formFiles;
            try
            {
                formFiles = Request.Form.Files;
            }
            catch
            {
                formFiles = null;
            }

            FileStorage.CreateDirectory(fullPath);
            try
            {
                if (Request.Headers.ContainsKey("Copy"))
                {
                    int statusCode = FileStorage.CopyFile(Path.Combine(storagePath, Request.Headers["Copy"]), fullPath);
                    return StatusCode(statusCode);
                }
                else if (Directory.Exists(fullPath))
                {
                    var file = formFiles[0];
                    using (FileStream fs = new FileStream(Path.Combine(fullPath, file.FileName), FileMode.Create))
                    {
                        file.CopyTo(fs);
                    }

                    return Ok();
                }
            }
            catch
            {
                return StatusCode(500);
            }

            return NotFound();
        }

        [HttpGet("{*path}")]
        public ActionResult Get(string path)
        {
            string fullPath = FileStorage.GetFullPath(path);

            if (Directory.Exists(fullPath))
            {
                try
                {
                    List<ItemInfo> catalog = FileStorage.GetFilesInCatalog(fullPath);
                    return new JsonResult(catalog);
                }
                catch
                {
                    return StatusCode(500);
                }
            }
            else if (System.IO.File.Exists(fullPath))
            {
                try
                {
                    var provider = new FileExtensionContentTypeProvider();
                    if (!provider.TryGetContentType(fullPath, out var contentType))
                    {
                        contentType = "application/unknown";
                    }

                    FileStream fileStream = new FileStream(fullPath, FileMode.Open);
                    return File(fileStream, contentType, Path.GetFileName(fullPath));
                }
                catch
                {
                    return StatusCode(500);
                }
            }

            return NotFound();
        }

        [HttpHead("{*path}")]
        public ActionResult GetHeader(string path)
        {
            string fullPath = FileStorage.GetFullPath(path);

            try
            {
                if (System.IO.File.Exists(fullPath))
                {
                    FileInfo fileInfo = new FileInfo(fullPath);
                    Response.Headers.Add("Name", fileInfo.Name);
                    Response.Headers.Add("Directory-Name", fileInfo.DirectoryName);
                    Response.Headers.Add("Size", fileInfo.Length.ToString());
                    Response.Headers.Add("Last-Write-Time", fileInfo.LastWriteTime.ToString());
                    Response.Headers.Add("Last-Access-Time", fileInfo.LastAccessTime.ToString());
                    return Ok();
                }
            }
            catch
            {
                return StatusCode(500);
            }

            return NotFound();
        }

        [HttpDelete("{*path}")]
        public ActionResult Delete(string path)
        {
            string fullPath;

            if (path == null)
            {
                return BadRequest();
            }
            else
            {
                fullPath = Path.Combine(storagePath, path);
            }

            try
            {
                if (Directory.Exists(fullPath))
                {
                    Directory.Delete(fullPath, true);
                    return Ok();
                }
                else if (System.IO.File.Exists(fullPath))
                {
                    System.IO.File.Delete(fullPath);
                    return Ok();
                }
            }
            catch
            {
                return StatusCode(500);
            }

            return NotFound();
        }
    }
}
