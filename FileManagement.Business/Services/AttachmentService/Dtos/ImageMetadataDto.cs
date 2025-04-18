namespace FileManagement.Business.Services.AttachmentService
{
    public class ImageMetadataDto
    {
        public string ImageId { get; set; }

        public string? CameraMake { get; set; }

        public string? CameraModel { get; set; }

        public double? Latitude { get; set; }

        public double? Longitude { get; set; }
    }
}