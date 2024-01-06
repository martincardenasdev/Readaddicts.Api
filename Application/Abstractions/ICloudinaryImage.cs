using Microsoft.AspNetCore.Http;

namespace Application.Abstractions
{
    public interface ICloudinaryImage
    {
        Task<(string imageUrl, string publicId)> Upload(IFormFile image, int width, int height);
        Task<List<(string imageUrl, string publicId)>> UploadMany(IFormFileCollection images, int width, int height);
    }
}
