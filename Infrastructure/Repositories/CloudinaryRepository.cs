using Application.Abstractions;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Domain.Entities;
using Microsoft.AspNetCore.Http;

namespace Infrastructure.Repositories
{
    public class CloudinaryRepository(Cloudinary cloudinary) : ICloudinaryImage
    {
        public async Task<(IEnumerable<string> deleted, IEnumerable<string> notDeleted)> Destroy(List<Image> images)
        {
            // Try to delete an image one by one and add the publicId to the corresponding list based on the result
            ICollection<string> failedDeletions = [];
            ICollection<string> successfulDeletions = [];   

            foreach (var image in images)
            {
                DeletionParams deletionParams = new(image.CloudinaryPublicId);

                var delete = await cloudinary.DestroyAsync(deletionParams);

                if (delete.Result == "ok")
                {
                    successfulDeletions.Add(image.CloudinaryPublicId);
                }
                else
                {
                    failedDeletions.Add(image.CloudinaryPublicId);
                }
            }

            return (successfulDeletions, failedDeletions);
        }

        public async Task<(string imageUrl, string publicId)> Upload(IFormFile image, int width, int height)
        {
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

        public async Task<List<(string imageUrl, string publicId)>> UploadMany(IFormFileCollection images)
        {
            List<(string imageUrl, string publicId)> imageUrls = [];

            foreach (var image in images)
            {
                await using Stream stream = image.OpenReadStream();

                ImageUploadParams uploadParams = new()
                {
                    File = new FileDescription(image.FileName, stream),
                    UseFilename = true,
                    UniqueFilename = true,
                    Overwrite = true,
                    Transformation = new Transformation().Quality(50).FetchFormat("webp")
                };

                ImageUploadResult uploadResult= await cloudinary.UploadAsync(uploadParams);

                if (uploadResult.Error is not null)
                {
                    throw new Exception(uploadResult.Error.Message);
                }

                imageUrls.Add((uploadResult.SecureUrl.AbsoluteUri, uploadResult.PublicId));
            }

            return imageUrls;
        }
    }
}
