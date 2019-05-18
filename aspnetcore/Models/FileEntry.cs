namespace TeslaCamBrowser.Models
{
    public class FileEntry
    {
        public string Time { get; set; }
        public string CombinedFile { get; set; }
        public string PreviewFile { get; set; }
        public string TimelapseFile { get; set; }
        public string MappedPreview { get; set; }
        public string MappedCombined { get; set; }
        public string MappedTimelapse { get; set; }

        public bool DeleteFile { get; set; }
        public string FileId { get; set; }
    }
}
