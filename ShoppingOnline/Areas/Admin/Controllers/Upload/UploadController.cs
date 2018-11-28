using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
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
            this._env = env;
        }

        [HttpPost]
        public async Task UploadImageForCKEditor(IList<IFormFile> upload, string CKEditorFuncNum, string CKEditor, string langCode)
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
                await HttpContext.Response.WriteAsync("<script>window.parent.CKEDITOR.tools.callFunction(" + CKEditorFuncNum + ", '" + Path.Combine(imageFolder, filename).Replace(@"\", @"/") + "');</script>");
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

                //ResizeAndSaveImage(file.OpenReadStream(), filePath, Int32.Parse(width), Int32.Parse(height));

                return new OkObjectResult(Path.Combine(imageFolder, filename).Replace(@"\", @"/"));
            }
        }

        [HttpPost]
        public IActionResult Thumbnail(string x, string y, string h, string w, string source)
        {
            var sourceFile = _env.WebRootPath + source;

            var decicmalX = decimal.Parse(x, CultureInfo.InvariantCulture);
            var decicmalY = decimal.Parse(y, CultureInfo.InvariantCulture);
            var decicmalW = decimal.Parse(w, CultureInfo.InvariantCulture);
            var decicmalH = decimal.Parse(h, CultureInfo.InvariantCulture);

            var intX = (int)(Math.Round(decicmalX));
            var intY = (int)(Math.Round(decicmalY));
            var intW = (int)(Math.Round(decicmalW));
            var intH = (int)(Math.Round(decicmalH));

            var img = Bitmap.FromFile(sourceFile);
            var bmp = new Bitmap(intW, intH, img.PixelFormat);
            var graphic = Graphics.FromImage(bmp);
            graphic.DrawImage(img, new Rectangle(0, 0, intW, intH), new Rectangle(intX, intY, intW, intH), GraphicsUnit.Pixel);

            ImageFormat imgFormat = img.RawFormat;

            string fileName = Guid.NewGuid().ToString();
            string folderName = _env.WebRootPath + $@"\uploaded\images\{DateTime.Now.ToString("yyyyMMdd")}\thumbnail";

            if (!Directory.Exists(folderName))
            {
                Directory.CreateDirectory(folderName);
            }

            string src = folderName + $@"\" + fileName + ".png";
            string path = $@"\uploaded\images\{DateTime.Now.ToString("yyyyMMdd")}\thumbnail\{fileName}.png";

            img.Dispose();
            bmp.Save(Path.Combine(src), imgFormat);

            Stream writingStream = new FileStream(src, FileMode.Open);

            ResizeAndSaveImage(writingStream, src, 265, 265);

            return new ObjectResult(path);
        }

        [HttpPost]
        public IActionResult Details(string x, string y, string h, string w, string source)
        {
            var sourceFile = _env.WebRootPath + source;

            var decicmalX = decimal.Parse(x, CultureInfo.InvariantCulture);
            var decicmalY = decimal.Parse(y, CultureInfo.InvariantCulture);
            var decicmalW = decimal.Parse(w, CultureInfo.InvariantCulture);
            var decicmalH = decimal.Parse(h, CultureInfo.InvariantCulture);

            var intX = (int)(Math.Round(decicmalX));
            var intY = (int)(Math.Round(decicmalY));
            var intW = (int)(Math.Round(decicmalW));
            var intH = (int)(Math.Round(decicmalH));

            var img = Bitmap.FromFile(sourceFile);
            var bmp = new Bitmap(intW, intH, img.PixelFormat);
            var graphic = Graphics.FromImage(bmp);
            graphic.DrawImage(img, new Rectangle(0, 0, intW, intH), new Rectangle(intX, intY, intW, intH), GraphicsUnit.Pixel);

            ImageFormat imgFormat = img.RawFormat;

            string fileName = Guid.NewGuid().ToString();
            string folderName = _env.WebRootPath + $@"\uploaded\images\{DateTime.Now.ToString("yyyyMMdd")}\details";

            if (!Directory.Exists(folderName))
            {
                Directory.CreateDirectory(folderName);
            }

            string src = folderName + $@"\" + fileName + ".png";
            string path = $@"\uploaded\images\{DateTime.Now.ToString("yyyyMMdd")}\details\{fileName}.png";

            img.Dispose();
            bmp.Save(Path.Combine(src), imgFormat);


            Stream writingStream = new FileStream(src, FileMode.Open);

            ResizeAndSaveImage(writingStream, src, 473, 473);

            return new ObjectResult(path);
        }


        [HttpPost]
        public IActionResult UploadAdvertisement()
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
                string extension = Path.GetExtension(file.FileName);
                var filename = Guid.NewGuid().ToString() + extension;
                var imageFolder = $@"\uploaded\images\ads\{now.ToString("yyyyMMdd")}";

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

                return new OkObjectResult(Path.Combine(imageFolder, filename).Replace(@"\", @"/"));
            }
        }

        [HttpPost]
        public IActionResult UploadSlide()
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
                string extension = Path.GetExtension(file.FileName);
                var filename = Guid.NewGuid().ToString() + extension;
                var imageFolder = $@"\uploaded\images\slide\{now.ToString("yyyyMMdd")}";

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

                return new OkObjectResult(Path.Combine(imageFolder, filename).Replace(@"\", @"/"));
            }
        }

        private void ResizeAndSaveImage(Stream stream, string filePath, int width, int height)
        {
            using (Image<Rgba32> image = SixLabors.ImageSharp.Image.Load(stream))
            {
                image.Mutate(x => x.Resize(width, height));
                stream.Dispose();
                image.Save(filePath);
                image.Dispose();
            }
        }
    }
}