using Microsoft.AspNetCore.Mvc;
using YoutubeDLCS;

[ApiController]
[Route("api/[controller]")]
public class DownloadController : ControllerBase
{
    private readonly AppConfig _config;
    private readonly youtubeDL _youtubeDL;

    public DownloadController()
    {
        _config = ConfigHelper.LoadConfig("appsettings.toml");
        _youtubeDL = new youtubeDL(_config.YoutubeDL.Path);
    }

    private void ScheduleFileDeletion(string filePath)
    {
        int minutes = _config.Cleanup.DeleteAfterMinutes;
        if (minutes > 0)
        {
            Task.Run(async () =>
            {
                await Task.Delay(TimeSpan.FromMinutes(minutes));
                try
                {
                    if (System.IO.File.Exists(filePath))
                        System.IO.File.Delete(filePath);
                }
                catch { /* ignore errors */ }
            });
        }
    }

    [HttpPost]
    public async Task<ActionResult<DownloadResponse>> Download([FromBody] DownloadRequest request)
    {
        try
        {
            int exitCode;
            if (request.IsPlaylist)
            {
                exitCode = await _youtubeDL.DownloadPlaylistAsync(request.Url, request.OutputDirectory, request.Format);
            }
            else
            {
                exitCode = await _youtubeDL.DownloadAsync(request.Url, request.OutputDirectory, request.Format);
            }

            if (exitCode == 0)
            {
                var dir = request.OutputDirectory;
                var files = Directory.GetFiles(dir);
                string? filePath = files.Length > 0 ? files[0] : null;
                if (filePath != null)
                {
                    ScheduleFileDeletion(filePath);
                    if (!string.IsNullOrWhiteSpace(_config.S3.Bucket))
                    {
                        var s3Url = await S3Helper.UploadFileAsync(filePath, _config.S3);
                        return Ok(new DownloadResponse
                        {
                            StatusCode = 0,
                            Message = "Download and upload successful.",
                            FilePath = s3Url
                        });
                    }
                    else
                    {
                        return Ok(new DownloadResponse
                        {
                            StatusCode = 0,
                            Message = "Download successful (local only).",
                            FilePath = filePath
                        });
                    }
                }
                else
                {
                    return StatusCode(500, new DownloadResponse
                    {
                        StatusCode = -2,
                        Message = "File not found after download.",
                        FilePath = null
                    });
                }
            }
            else
            {
                return StatusCode(500, new DownloadResponse
                {
                    StatusCode = exitCode,
                    Message = "Download failed.",
                    FilePath = null
                });
            }
        }
        catch (Exception ex)
        {
            return StatusCode(500, new DownloadResponse
            {
                StatusCode = -1,
                Message = ex.Message,
                FilePath = null
            });
        }
    }
}
