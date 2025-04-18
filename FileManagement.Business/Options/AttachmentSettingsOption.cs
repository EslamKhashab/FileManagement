namespace FileManagement.Business.Options
{
    public class AttachmentSettingsOption
    {
        public string StorageFolder { get; set; }

        public Dictionary<string, int> Sizes { get; set; }
    }
}