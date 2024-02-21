namespace EcommerceDotNetCore.Services.Media
{
    public interface IImageService
    {
        public string UploadImage(string wwwRootPath, string folderPath, string fileName, Stream imageStream);
        public void DeleteImage(string wwwRootPath, string folderPath, string fileName);
        
    }
}
