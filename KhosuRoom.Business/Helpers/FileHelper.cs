using Microsoft.AspNetCore.Http;

namespace KhosuRoom.Business.Helpers;

public static class FileHelper
{
    public static bool CheckSize(this IFormFile file, int size)
    {
        return file.Length <= size * 2 * 1024 * 1024;
    }
    public static bool CheckType(this IFormFile file, string type = "image")
    {
        return file.ContentType.Contains(type);
    }


}
