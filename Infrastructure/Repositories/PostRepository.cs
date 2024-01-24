using Application.Abstractions;
using Application.Interfaces;
using Domain.Dto;
using Domain.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class PostRepository(ApplicationDbContext context, ICloudinaryRepository cloudinary) : IPostRepository
    {
        private readonly ApplicationDbContext _context = context;
        private readonly ICloudinaryRepository _cloudinary = cloudinary;

        public async Task<string> CreatePost(string userId, string? groupId, Post post, IFormFileCollection? images)
        {
            if (groupId is null)
            {
                post.GroupId = null;
            }
            else
            {
                post.GroupId = groupId;
            }

            if (string.IsNullOrWhiteSpace(post.Content))
            {
                return string.Empty;
            }

            var newPost = new Post
            {
                UserId = userId,
                GroupId = post.GroupId,
                Created = DateTimeOffset.UtcNow,
                Content = post.Content,
                Images = post.Images
            };

            _context.Add(newPost);

            int rowsAffected = await _context.SaveChangesAsync();

            if (rowsAffected is 0)
            {
                return string.Empty;
            }

            if (images is not null)
            {
                _ = await AddImagesToPost(newPost.Id, userId, images);
            }

            return newPost.Id;
        }

        public async Task<(IEnumerable<string> deleted, IEnumerable<string> notDeleted)> DeleteImageFromPost(string postId, string userId, List<string> imageIds)
        {
            var post = await _context.FindAsync<Post>(postId);

            if (post is null || post.UserId != userId)
                return (Enumerable.Empty<string>(), Enumerable.Empty<string>());

            List<Image> imagesToDelete = await _context.Images
                .Where(i => imageIds.Contains(i.Id) && i.PostId == postId && i.UserId == userId)
                .ToListAsync();

            // where errors its a list of the publicIds that failed to be deleted (if any)
            var (deleted, notDeleted) = await _cloudinary.Destroy(imagesToDelete);

            // after deleting from cloudinary delete from db, but exclude the images that failed to be deleted from cloudinary (if any)
            List<Image> newImagesToDelete = imagesToDelete
                .Where(image => !notDeleted.Contains(image.CloudinaryPublicId))
                .ToList();

            _context.RemoveRange(newImagesToDelete);

            post.Modified = DateTimeOffset.UtcNow;

            _context.Update(post);

            int rowsAffected = await _context.SaveChangesAsync();

            if (rowsAffected is 0)
                return (Enumerable.Empty<string>(), notDeleted);

            return (deleted, notDeleted);
        }

        public async Task<bool> DeletePost(string userId, string postId)
        {
            Post post = await _context.FindAsync<Post>(postId);

            if (post is null)
            {
                return false;
            }

            if (post.UserId != userId)
            {
                return false;
            }

            _context.Remove(post);
            int rowsAffected = await _context.SaveChangesAsync();

            if (rowsAffected is 0)
            {
                return false;
            }

            return true;
        }

        public async Task<PostDto?> GetPost(string id)
        {
            return await _context.Posts
                .Where(post => post.Id == id)
                .Include(post => post.Creator)
                //.Include(post => post.Comments)
                .Select(post => new PostDto
                {
                    Id = post.Id,
                    UserId = post.UserId,
                    Created = post.Created,
                    Content = post.Content,
                    Creator = new UserDto
                    {
                        Id = post.Creator.Id,
                        UserName = post.Creator.UserName,
                        ProfilePicture = post.Creator.ProfilePicture
                    },
                    Images = post.Images.Select(image => new ImageDto
                    {
                        Id = image.Id,
                        Url = image.Url
                    }).ToList(),
                    CommentCount = post.Comments.Count
                    //Comments = post.Comments.Select(comment => new CommentDto
                    //{
                    //    Id = comment.Id,
                    //    UserId = comment.UserId,
                    //    Content = comment.Content,
                    //    Created = comment.Created,
                    //    ReplyCount = _context.Comments.Count(reply => reply.ParentId == comment.Id)
                    //}).ToList()
                }).FirstOrDefaultAsync();
        }

        public async Task<DataCountPagesDto<List<PostDto>>> GetPosts(int page, int limit)
        {
            var posts = await _context.Posts
                .Include(post => post.Creator)
                .OrderByDescending(post => post.Created)
                .Skip((page - 1) * limit)
                .Take(limit)
                .Select(post => new PostDto
                {
                    Id = post.Id,
                    UserId = post.UserId,
                    Created = post.Created,
                    Content = post.Content,
                    Creator = new UserDto
                    {
                        Id = post.Creator.Id,
                        UserName = post.Creator.UserName,
                        ProfilePicture = post.Creator.ProfilePicture
                    },
                    Images = post.Images.Select(image => new ImageDto
                    {
                        Id = image.Id,
                        Url = image.Url
                    }).Take(5).ToList(),
                    CommentCount = _context.Comments.Count(comment => comment.PostId == post.Id),
                    ImageCount = _context.Images.Count(image => image.PostId == post.Id)
                })
                .ToListAsync();

            int count = await _context.Posts.CountAsync();

            int pages = (int)Math.Ceiling(count / (double)limit);

            return new DataCountPagesDto<List<PostDto>>
            {
                Data = posts,
                Count = count,
                Pages = pages
            };
        }

        public async Task<(bool, PostDto)> UpdatePostContent(string postId, string userId, string content)
        {
            Post? post = await _context.Posts
                .Include(post => post.Creator)
                .Include(post => post.Images)
                .FirstOrDefaultAsync(post => post.Id == postId);

            if (post is null || post.UserId != userId)
                return (false, new PostDto());

            post.Content = content;
            post.Modified = DateTimeOffset.UtcNow;

            _context.Update(post);
            int rowsAffected = await _context.SaveChangesAsync();

            if (rowsAffected is 0)
                return (false, new PostDto());

            return (true, new PostDto
            {
                Id = post.Id,
                UserId = post.UserId,
                Created = post.Created,
                Content = post.Content,
                Creator = new UserDto
                {
                    Id = post.Creator.Id,
                    UserName = post.Creator.UserName,
                    ProfilePicture = post.Creator.ProfilePicture
                },
                Images = post.Images.Select(image => new ImageDto
                {
                    Id = image.Id,
                    Url = image.Url
                }).ToList(),
                CommentCount = _context.Comments.Count(comment => comment.PostId == post.Id)
            });
        }

        public async Task<IEnumerable<ImageDto>> AddImagesToPost(string postId, string userId, IFormFileCollection images)
        {
            var post = await _context.FindAsync<Post>(postId);

            if (post is null || post.UserId != userId)
                return [];

            // Get a list of the images that were uploaded to Cloudinary and add them to the post
            List<(string imageUrl, string publicId, string result)> imgs = await _cloudinary.UploadMany(images);

            if (imgs.Count == 0)
            {
                return [];
            }

            List<Image> newPostImages = imgs.Select(image => new Image
            {
                PostId = postId,
                UserId = userId,
                Url = image.imageUrl,
                CloudinaryPublicId = image.publicId,
                Created = DateTimeOffset.UtcNow
            }).ToList();

            _context.Images.AddRange(newPostImages);

            post.Modified = DateTimeOffset.UtcNow;

            _context.Update(post);

            int rowsAffected = await _context.SaveChangesAsync();

            if (rowsAffected is 0)
                return [];

            return newPostImages.Select(i => new ImageDto
            {
                Id = i.Id,
                Url = i.Url
            }).ToList();
        }
    }
}
