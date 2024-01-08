using Domain.Entities;
using Microsoft.AspNetCore.Http;

namespace Application.Abstractions
{
    public interface ICloudinaryRepository
    {
        Task<(string imageUrl, string publicId)> Upload(IFormFile image, int width, int height);
        Task<List<(string imageUrl, string publicId)>> UploadMany(IFormFileCollection images); // This will be pretty much the same but we are iterating over the image collection, will make it more readable. Will not be used for profile pictures.
        Task<(IEnumerable<string> deleted, IEnumerable<string> notDeleted)> Destroy(List<Image> images);
    }
}
