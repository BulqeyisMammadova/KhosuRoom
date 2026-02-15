using Microsoft.AspNetCore.Http;

namespace KhosuRoom.Business.Services.Abstractions;

public interface ICloudinaryService
{
    Task<string> FileUploadAsync(IFormFile file);
    Task<bool> FileDeleteAsync(string filePath);
}
