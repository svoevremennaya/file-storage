using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace file_storage.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class FileStorageController : ControllerBase
    {
        private readonly string storagePath = @"C:\FileStorage";

        [HttpPut("{*path}")]
        public ActionResult Put(string path)
        {
            string fullPath;

            if (path == null)
            {
                fullPath = storagePath;
            }
            else
            {
                fullPath = Path.Combine(storagePath, path);
            }

            try
            {
                if (Request.Headers.ContainsKey("Copy"))
                {
                    int statusCode = FileStorage.CopyFile(Request.Headers["Copy"], fullPath);
                    return StatusCode(statusCode);
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
            string fullPath;

            if (path == null)
            {
                fullPath = storagePath;
            }
            else
            {
                fullPath = Path.Combine(storagePath, path);
            }

            if (Directory.Exists(fullPath))
            {
                try
                {
                    List<string> catalog = FileStorage.GetFilesInCatalog(fullPath);
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
                    FileStream fileStream = new FileStream(fullPath, FileMode.Open);
                    return File(fileStream, Path.GetFileName(fullPath));
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
            string fullPath;

            if (path == null)
            {
                fullPath = storagePath;
            }
            else
            {
                fullPath = Path.Combine(storagePath, path);
            }

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
