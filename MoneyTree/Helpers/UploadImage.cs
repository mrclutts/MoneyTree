using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Web;

namespace MoneyTree.Helpers
{
        public static class ImageUploadValidator
        {
            public static bool IsWebFriendlyImage(HttpPostedFileBase file)
            {
                //check for actual object
                if (file == null)
                    return false;
                //check size - file must be less than 2MB and greater than 1kb
                if (file.ContentLength > 2 * 1024 * 1024 || file.ContentLength < 1024)
                    return false;
                try
                {
                    var ext = Path.GetExtension(file.FileName).ToLower();
                    if (file.ContentType.Contains("image"))
                    {
                        using (var img = Image.FromStream(file.InputStream))
                        {
                            return ImageFormat.Jpeg.Equals(img.RawFormat) ||
                                ImageFormat.Png.Equals(img.RawFormat) ||
                                ImageFormat.Gif.Equals(img.RawFormat);
                        }
                    }
                    else {
                        if ((ext == ".docx") || (ext == ".doc") || (ext == ".pdf"))
                        {
                            return true;
                        };
                        return false;
                    }


                }
                catch (Exception e)
                {
                    return false;
                }
            }
        }
    }
