using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ShoppingOnline.WebApplication.Areas.Admin.Controllers.Base;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

namespace ShoppingOnline.WebApplication.Areas.Admin.Controllers.Upload
{
    public class UploadController : BaseController
    {
        private IHostingEnvironment _env;

        public UploadController(IHostingEnvironment env)
        {
            _env = env;
        }

        [HttpPost]
        public async Task UploadImageForCKEditor(IList<IFormFile> upload, string CKEditorFuncNum, string CKEditor,
            string langCode)
        {
            DateTime now = DateTime.Now;
            if (upload.Count == 0)
            {
                await HttpContext.Response.WriteAsync("Your picture");
            }
            else
            {
                var file = upload[0];

                string extension = Path.GetExtension(file.FileName);
                var filename = Guid.NewGuid().ToString() + extension;

                var imageFolder = $@"\uploaded\images\{now.ToString("yyyyMMdd")}";

                string folder = _env.WebRootPath + imageFolder;

                if (!Directory.Exists(folder))
                {
                    Directory.CreateDirectory(folder);
                }

                string filePath = Path.Combine(folder, filename);
                using (FileStream fs = System.IO.File.Create(filePath))
                {
                    file.CopyTo(fs);
                    fs.Flush();
                }

                await HttpContext.Response.WriteAsync("<script>window.parent.CKEDITOR.tools.callFunction(" +
                                                      CKEditorFuncNum + ", '" +
                                                      Path.Combine(imageFolder, filename).Replace(@"\", @"/") +
                                                      "');</script>");
            }
        }

        /// <summary>
        /// Upload image for form
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public IActionResult UploadImage()
        {
            DateTime now = DateTime.Now;
            var files = Request.Form.Files;
            if (files.Count == 0)
            {
                return new BadRequestObjectResult(files);
            }
            else
            {
                var file = files[0];
                //var filename = ContentDispositionHeaderValue
                //                    .Parse(file.ContentDisposition)
                //                    .FileName
                //                    .Trim('"');
                string extension = Path.GetExtension(file.FileName);
                var filename = Guid.NewGuid().ToString() + extension;

                var imageFolder = $@"\uploaded\images\{now.ToString("yyyyMMdd")}";

                string folder = _env.WebRootPath + imageFolder;

                if (!Directory.Exists(folder))
                {
                    Directory.CreateDirectory(folder);
                }

                string filePath = Path.Combine(folder, filename);
//                using (FileStream fs = System.IO.File.Create(filePath))
//                {
//                    file.CopyTo(fs);
//                    fs.Flush();
//                }

                ResizeAndSaveImage(file.OpenReadStream(), filePath);

                return new OkObjectResult(Path.Combine(imageFolder, filename).Replace(@"\", @"/"));
            }
        }

        private void ResizeAndSaveImage(Stream stream, string filePath)
        {
            using (Image<Rgba32> image = Image.Load(stream))
            {
                image.Mutate(x => x.Pad(600, 600).BackgroundColor(Rgba32.White).Opacity(1));
                image.Save(filePath); // Automatic encoder selected based on extension.
            }
        }
    }
}