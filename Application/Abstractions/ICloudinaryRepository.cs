using Domain.Entities;
using Microsoft.AspNetCore.Http;

namespace Application.Abstractions
{
    public interface ICloudinaryRepository
    {
        // TODO: Fix large image uploads
        // System.Exception: File size too large. Got 12993695. Maximum is 10485760.
        Task<(string imageUrl, string publicId, string error)> Upload(IFormFile image, int width, int height);
        Task<List<(string imageUrl, string publicId, string error)>> UploadMany(IFormFileCollection images); // This will be pretty much the same but we are iterating over the image collection, will make it more readable. Will not be used for profile pictures.
        Task<(IEnumerable<string> deleted, IEnumerable<string> notDeleted)> Destroy(List<Image> images);
    }
}
