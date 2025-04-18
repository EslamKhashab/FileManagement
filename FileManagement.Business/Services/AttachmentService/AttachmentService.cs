using FileManagement.Business.Options;
using MetadataExtractor;
using MetadataExtractor.Formats.Exif;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;

namespace FileManagement.Business.Services.AttachmentService
{
    public class AttachmentService : IAttachmentService
    {
        private readonly string _basePath;
        private readonly AttachmentSettingsOption _attachmentSettings;
        private readonly ImageSettingsOption _imageSettings;

        public AttachmentService(IOptionsSnapshot<AttachmentSettingsOption> attachmentSettings, IOptionsSnapshot<ImageSettingsOption> imageSettings, IWebHostEnvironment env)
        {
            _attachmentSettings = attachmentSettings.Value;
            _imageSettings = imageSettings.Value;
            _basePath = Path.Combine(env.WebRootPath, _attachmentSettings.StorageFolder);
        }

        public async Task<List<UploadResultDto>> UploadImagesAsync(IEnumerable<IFormFile> files)
        {
            var tasks = files.Select(file => ProcessFileAsync(file));
            var results = await Task.WhenAll(tasks);
            return results.ToList();
        }

        public async Task<UploadResultDto> ProcessFileAsync(IFormFile file)
        {

            var results = new List<UploadResultDto>();

            var resultDto = new UploadResultDto { OriginalFileName = file.FileName };

            if (!ValidateFile(file))
            {

                resultDto.ErrorMessage = "Invalid file format or size.";
                results.Add(resultDto);
                return resultDto;
            }
            try
            {
                var id = Guid.NewGuid().ToString();
                var imagePath = Path.Combine(_basePath, id);
                System.IO.Directory.CreateDirectory(imagePath);

                using var image = await Image.LoadAsync(file.OpenReadStream());

                var originalWebP = Path.Combine(imagePath, "original.webp");
                await image.SaveAsWebpAsync(originalWebP);

                // not so great but it works, it's better to move for background job

                foreach (var size in _attachmentSettings.Sizes)
                {
                    using var clone = image.Clone(x => x.Resize(new ResizeOptions
                    {
                        Mode = ResizeMode.Max,
                        Size = new Size(size.Value, 0)
                    }));
                    await clone.SaveAsWebpAsync(Path.Combine(imagePath, $"{size.Key}.webp"));
                }

                var metadata = ExtractMetadata(file);
                metadata.ImageId = id;
                var metadataPath = Path.Combine(imagePath, "metadata.json");
                await File.WriteAllTextAsync(metadataPath, System.Text.Json.JsonSerializer.Serialize(metadata));
                resultDto.Id = id;
            }
            catch (Exception ex)
            {
                resultDto.ErrorMessage = $"Error processing image: {ex.Message}";
            }
            return resultDto;

        }

        public async Task<DownloadImageDto?> GetResizedImageAsync(string id, string size)
        {
            if (!_attachmentSettings.Sizes.ContainsKey(size)) return null;
            var path = Path.Combine(_basePath, id, $"{size}.webp");
            if (!File.Exists(path)) return null;

            return new DownloadImageDto
            {
                Stream = new FileStream(path, FileMode.Open, FileAccess.Read),
                FileName = $"{id}_{size}.webp"
            };
        }

        public async Task<ImageMetadataDto?> GetMetadataAsync(string id)
        {
            var path = Path.Combine(_basePath, id, "metadata.json");
            if (!File.Exists(path)) return null;

            var json = await File.ReadAllTextAsync(path);
            return System.Text.Json.JsonSerializer.Deserialize<ImageMetadataDto>(json);
        }

        private bool ValidateFile(IFormFile file)
        {
            var allowed = _imageSettings.AcceptedFileTypes;
            return file.Length <= _imageSettings.MaxBytes && allowed.Contains(file.ContentType);
        }

        private ImageMetadataDto ExtractMetadata(IFormFile file)
        {
            var result = new ImageMetadataDto();

            using var stream = file.OpenReadStream();
            var directories = ImageMetadataReader.ReadMetadata(stream);
            var subIfdDirectory = directories.OfType<ExifIfd0Directory>().FirstOrDefault();
            var gpsDirectory = directories.OfType<GpsDirectory>().FirstOrDefault();

            if (subIfdDirectory != null)
            {
                result.CameraMake = subIfdDirectory.GetDescription(ExifDirectoryBase.TagMake);
                result.CameraModel = subIfdDirectory.GetDescription(ExifDirectoryBase.TagModel);
            }

            if (gpsDirectory?.GetGeoLocation() is { } location)
            {
                result.Latitude = location.Latitude;
                result.Longitude = location.Longitude;
            }

            return result;
        }
    }
}