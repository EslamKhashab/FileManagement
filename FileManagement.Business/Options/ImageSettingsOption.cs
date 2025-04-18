namespace FileManagement.Business.Options
{
    public class ImageSettingsOption
    {
        public int MaxBytes { get; set; }

        public IEnumerable<string> AcceptedFileTypes { get; set; }
    }
}