using Application.Abstractions;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Http;

namespace Infrastructure.Repositories
{
    public class CloudinaryRepository(Cloudinary cloudinary) : ICloudinaryImage
    {
        public async Task<(string imageUrl, string publicId)> Upload(IFormFile image, int width, int height)
        {
            if (image is null)
            {
                return (null, null);
            }

            await using var stream = image.OpenReadStream();

            ImageUploadParams uploadParams = new()
            {
                File = new FileDescription(image.FileName, stream),
                UseFilename = true,
                UniqueFilename = true,
                Overwrite = true,
                Transformation = new Transformation().Quality(50).FetchFormat("webp")
            };

            if (width > 0 && height > 0)
            {
                uploadParams.Transformation.Crop("fill").Width(width).Height(height);
            }

            var uploadResult = await cloudinary.UploadAsync(uploadParams);

            if (uploadResult.Error is not null)
            {
                throw new Exception(uploadResult.Error.Message);
            }

            return (uploadResult.SecureUrl.AbsoluteUri, uploadResult.PublicId);
        }
    }
}
