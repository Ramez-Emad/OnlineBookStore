using Microsoft.AspNetCore.Http;


namespace Bulky.BL.Common.Attachments
{
    public interface IAttachmentService
    {
        string? UploadProductImage(IFormFile file);
        string? UpdateProductImage(string oldFilePath, IFormFile file);

        bool DeleteProductImage(string filePath);

    }
}
