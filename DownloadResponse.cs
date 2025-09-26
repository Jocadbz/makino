namespace YoutubeDLCS;

public class DownloadResponse
{
    public int StatusCode { get; set; }
    public string Message { get; set; } = string.Empty;
    public string? FilePath { get; set; }
}
