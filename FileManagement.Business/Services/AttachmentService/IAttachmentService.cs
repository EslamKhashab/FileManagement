using Microsoft.AspNetCore.Http;

namespace FileManagement.Business.Services.AttachmentService
{
    public interface IAttachmentService
    {
        Task<List<UploadResultDto>> UploadImagesAsync(IEnumerable<IFormFile> request);

        Task<DownloadImageDto?> GetResizedImageAsync(string id, string size);

        Task<ImageMetadataDto?> GetMetadataAsync(string id);
    }
}