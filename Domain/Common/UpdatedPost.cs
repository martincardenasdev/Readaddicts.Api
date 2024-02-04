using Domain.Dto;

namespace Domain.Common
{
    public class UpdatedPost
    {
        public string? NewContent { get; set; }
        public List<ImageDto>? AddedImages { get; set; }
        public List<string>? RemovedImages { get; set; }
    }
}
