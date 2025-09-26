namespace YoutubeDLCS;

public class DownloadRequest
{
    public string Url { get; set; } = string.Empty;
    public string OutputDirectory { get; set; } = string.Empty;
    public string? Format { get; set; }
    public bool IsPlaylist { get; set; } = false;
}
