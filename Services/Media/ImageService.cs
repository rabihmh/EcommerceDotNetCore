using System;
using System.IO;

namespace EcommerceDotNetCore.Services.Media
{
    public class ImageService : IImageService
    {
        public string UploadImage(string wwwRootPath, string folderPath, string fileName, Stream imageStream)
        {
            try
            {
                string uploadsFolder = Path.Combine(wwwRootPath, folderPath);
                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                string uniqueFileName = Guid.NewGuid().ToString() + "_" + fileName;
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    imageStream.CopyTo(fileStream);
                }

                return Path.Combine(folderPath, uniqueFileName);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public void DeleteImage(string wwwRootPath, string folderPath, string fileName)
        {
            try
            {
                string filePath = Path.Combine(wwwRootPath, folderPath, fileName);

                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

    }
}