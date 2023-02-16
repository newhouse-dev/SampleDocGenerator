namespace Newhouse.SampleDocGenerator.Models
{
    public class AppSettings
    {
        public string OutputLocation { get; set; }
        public int NumberOfFiles { get; set; }
        public int FileSizeKB { get; set; }
        public string[] FileTypes { get; set; }
    }
}

