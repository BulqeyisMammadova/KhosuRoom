using Microsoft.AspNetCore.Http;

namespace KhosuRoom.Business.Helpers;

public static class FileValidationHelper
{
    public static readonly string[] AllowedExtensions =
        [".pdf", ".doc", ".docx","txt", ".png", ".jpg", ".jpeg"];

    public const long MaxFileSizeBytes = 10 * 1024 * 1024; 
    public const int MaxFiles = 10;

    public static bool IsAllowed(IFormFile file)
    {
        var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
        return AllowedExtensions.Contains(ext) && file.Length > 0 && file.Length <= MaxFileSizeBytes;
    }
}