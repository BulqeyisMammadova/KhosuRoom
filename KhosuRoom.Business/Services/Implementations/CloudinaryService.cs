using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using KhosuRoom.Business.Dtos.CloudinaryDtos;
using KhosuRoom.Business.Services.Abstractions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System.Net;

namespace KhosuRoom.Business.Services.Implementations;

internal class CloudinaryService : ICloudinaryService
{
    private readonly CloudinaryOptionsDto _options;
    private readonly IConfiguration _configuration;
    private readonly Cloudinary _cloudinary;

    public CloudinaryService(IConfiguration configuration)
    {
        _configuration = configuration;
        _options = configuration.GetSection("CloudinarySetting").Get<CloudinaryOptionsDto>() ?? new();

        var myAccount = new Account { ApiKey = _options.ApiKey, ApiSecret = _options.ApiSecret, Cloud = _options.CloudName };
        _cloudinary = new Cloudinary(myAccount);
        _cloudinary.Api.Secure = true;


    }

    public async Task<string> FileUploadAsync(IFormFile file)
    {
        string fileName = string.Concat(Guid.NewGuid(), file.FileName.Substring(file.FileName.LastIndexOf('.')));

        var uploadResult = new ImageUploadResult();
        if (file.Length > 0)
        {
            using var stream = file.OpenReadStream();
            var uploadParams = new ImageUploadParams
            {
                File = new FileDescription(fileName, stream),
                Folder = "ImageFolder"
            };
            uploadResult = await _cloudinary.UploadAsync(uploadParams);
        }
        string url = uploadResult.SecureUrl.ToString();

        return url;
    }
    public async Task<bool> FileDeleteAsync(string filePath)
    {
        try
        {
            string publicIdWithExtension = filePath.Substring(filePath.LastIndexOf("ImageFolder"));
            string publicId = publicIdWithExtension.Substring(0, publicIdWithExtension.LastIndexOf('.'));

            var deleteParams = new DelResParams()
            {
                PublicIds = new List<string> { publicId },
                Type = "upload",
                ResourceType = ResourceType.Image
            };
            var result = await _cloudinary.DeleteResourcesAsync(deleteParams);

            return result.StatusCode == HttpStatusCode.OK;
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            return false;
        }
    }
}