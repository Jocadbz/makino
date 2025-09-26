using Tomlyn;
using Tomlyn.Model;

namespace YoutubeDLCS;

public class AppConfig
{
    public YoutubeDLConfig YoutubeDL { get; set; } = new();
    public S3Config S3 { get; set; } = new();
}

public class YoutubeDLConfig
{
    public string Path { get; set; } = string.Empty;
}

public class S3Config
{
    public string AccessKey { get; set; } = string.Empty;
    public string SecretKey { get; set; } = string.Empty;
    public string Region { get; set; } = string.Empty;
    public string Bucket { get; set; } = string.Empty;
}

public static class ConfigHelper
{
    public static AppConfig LoadConfig(string filePath)
    {
        var model = Toml.ToModel(File.ReadAllText(filePath)) as TomlTable;
        var config = new AppConfig();
        if (model != null)
        {
            var youtubeDL = model["youtubeDL"] as TomlTable;
            if (youtubeDL != null)
                config.YoutubeDL.Path = youtubeDL["path"]?.ToString() ?? "";

            var s3 = model["s3"] as TomlTable;
            if (s3 != null)
            {
                config.S3.AccessKey = s3["access_key"]?.ToString() ?? "";
                config.S3.SecretKey = s3["secret_key"]?.ToString() ?? "";
                config.S3.Region = s3["region"]?.ToString() ?? "";
                config.S3.Bucket = s3["bucket"]?.ToString() ?? "";
            }
        }
        return config;
    }
}
