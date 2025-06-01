using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bulky.Utility.Attachments
{
    public class AttachmentService : IAttachmentService
    {
        List<string> _allowedExtensions = new List<string> { ".jpg", ".png", ".jpeg" };
        const int maxFileSize = 2_097_152;

        public string? Upload(IFormFile file, string folderName)
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

        public bool Delete(string filePath)
        {
            var fullPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\Files", filePath);
            if (!File.Exists(fullPath)) return false;
            File.Delete(fullPath);
            return true;

        }

    }
}
