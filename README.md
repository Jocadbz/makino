# Makino YouTubeDL Microservice

A scalable ASP.NET Core microservice for downloading YouTube videos and playlists, then uploading them to Amazon S3 for infinite storage. Configuration is managed via a simple TOML file.

## Features
- Download YouTube videos or playlists using youtube-dl
- Upload results to Amazon S3
- Asynchronous, scalable API (handles hundreds of requests)
- Configuration via `appsettings.toml` (S3 credentials, youtube-dl path)
- Returns S3 URL for downloaded content

## Setup
1. **Clone the repo**
2. **Install dependencies:**
   - .NET 9 SDK
   - youtube-dl or yt-dlp binary
   - AWS S3 bucket and credentials
3. **Configure `appsettings.toml`:**
   ```toml
   [youtubeDL]
   path = "/usr/local/bin/youtube-dl"

   [s3]
   access_key = "YOUR_AWS_ACCESS_KEY"
   secret_key = "YOUR_AWS_SECRET_KEY"
   region = "us-east-1"
   bucket = "your-s3-bucket-name"
   ```
4. **Run the service:**
   ```fish
   dotnet run
   ```

## API Usage
POST `/api/download`

**Request JSON:**
```
{
  "url": "https://youtube.com/watch?v=...",
  "outputDirectory": "downloads",
  "format": "best",
  "isPlaylist": false
}
```

**Response JSON:**
```
{
  "statusCode": 0,
  "message": "Download and upload successful.",
  "filePath": "https://your-s3-bucket.s3.us-east-1.amazonaws.com/video.mp4"
}
```

## Security
- Never commit `appsettings.toml` with real credentials.
- Use IAM roles and policies for S3 access.

## License
MIT
