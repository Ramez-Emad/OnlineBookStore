using Bulky.DataAccess.Entities;
using Microsoft.AspNetCore.Http;

namespace Bulky.BL.Common.Attachments
{
    public class AttachmentService : IAttachmentService
    {

        List<string> _allowedExtensions = new List<string> { ".jpg", ".png", ".jpeg" };
        const int maxFileSize = 2_097_152;
        const string productsImagesBaseFolderPath = "Images\\Products";

        private string? Upload(IFormFile file, string folderName)
        {
            var extension = Path.GetExtension(file.FileName);

            if (!_allowedExtensions.Contains(extension)) return null;

            if (file.Length == 0 || file.Length > maxFileSize) return null;

            var folderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\Files", folderName);

            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }

            var fileName = $"{Guid.NewGuid()}_{extension}";

            var filePath = Path.Combine(folderPath, fileName);

            using var FileStream = new FileStream(filePath, FileMode.Create);

            file.CopyTo(FileStream);

            return fileName;
        }

        private bool Delete(string filePath)
        {
            var fullPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\Files", filePath);
            if (!File.Exists(fullPath)) return false;
            File.Delete(fullPath);
            return true;

        }

        public string? UploadProductImage(IFormFile file)
        {
            return Upload(file, productsImagesBaseFolderPath);
        }

        public string? UpdateProductImage(string oldFilePath , IFormFile file)
        {
            string path = Path.Combine(productsImagesBaseFolderPath, oldFilePath);

            Delete(path);

            return UploadProductImage(file);
        }

        public bool DeleteProductImage(string filePath)
        {
            if(filePath == "PlaceHolder.png")
                return false;
            string path = Path.Combine(productsImagesBaseFolderPath, filePath);
            return Delete(path);
        }
    }
}
