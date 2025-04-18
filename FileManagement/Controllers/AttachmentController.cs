using FileManagement.Business.Services.AttachmentService;
using Microsoft.AspNetCore.Mvc;

namespace FileManagement.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AttachmentController : ControllerBase
    {
        private readonly IAttachmentService _attachmentService;
        private readonly ILogger<AttachmentController> _logger;

        public AttachmentController(IAttachmentService attachmentService, ILogger<AttachmentController> logger)
        {
            _attachmentService = attachmentService;
            _logger = logger;
        }

        [HttpPost("upload")]
        public async Task<IActionResult> UploadImages(IEnumerable<IFormFile> request)
        {
            var result = await _attachmentService.UploadImagesAsync(request);
            return Ok(result);
        }

        [HttpGet("download")]
        public async Task<IActionResult> DownloadImage([FromQuery] string id, [FromQuery] string size)
        {
            var file = await _attachmentService.GetResizedImageAsync(id, size);
            if (file == null) return NotFound();
            return File(file.Stream, file.ContentType, file.FileName);
        }

        [HttpGet("metadata/{id}")]
        public async Task<IActionResult> GetMetadata(string id)
        {
            var metadata = await _attachmentService.GetMetadataAsync(id);
            if (metadata == null) return NotFound();
            return Ok(metadata);
        }
    }
}