using Microsoft.AspNetCore.Http;

namespace FileManagement.Business.Services.AttachmentService
{
    public class UploadAttachmentDto
    {
        public IEnumerable<IFormFile> Attachment { get; set; }
    }
}