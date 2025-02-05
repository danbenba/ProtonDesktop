namespace ProtonDesktop
{
    public class DownloadInfo
    {
        public string FileName { get; set; } = "";
        public string ResultFilePath { get; set; } = "";
        public long TotalBytes { get; set; } = 0;
        public long BytesReceived { get; set; } = 0;
        public string Status { get; set; } = "En cours"; // "En cours", "Terminé", "Annulé", "Erreur"...
    }
}
