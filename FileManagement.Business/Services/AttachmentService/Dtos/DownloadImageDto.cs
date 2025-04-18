namespace FileManagement.Business.Services.AttachmentService
{
    public class DownloadImageDto
    {
        public Stream Stream { get; set; } = default!;
        public string ContentType { get; set; } = "image/webp";
        public string FileName { get; set; } = default!;
    }
}