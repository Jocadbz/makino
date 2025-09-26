using Amazon.S3;
using Amazon.S3.Transfer;

namespace YoutubeDLCS;

public static class S3Helper
{
    public static async Task<string> UploadFileAsync(string filePath, S3Config config)
    {
        var s3Client = new AmazonS3Client(config.AccessKey, config.SecretKey, Amazon.RegionEndpoint.GetBySystemName(config.Region));
        var fileName = Path.GetFileName(filePath);
        var uploadRequest = new TransferUtilityUploadRequest
        {
            FilePath = filePath,
            BucketName = config.Bucket,
            Key = fileName,
            CannedACL = S3CannedACL.PublicRead
        };
        var transferUtility = new TransferUtility(s3Client);
        await transferUtility.UploadAsync(uploadRequest);
        return $"https://{config.Bucket}.s3.{config.Region}.amazonaws.com/{fileName}";
    }
}
