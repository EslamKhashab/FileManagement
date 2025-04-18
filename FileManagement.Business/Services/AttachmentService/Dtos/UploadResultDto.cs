namespace FileManagement.Business.Services.AttachmentService
{
    public class UploadResultDto
    {
        public string Id { get; set; }

        public string OriginalFileName { get; set; }

        public string? ErrorMessage { get; set; }
    }
}