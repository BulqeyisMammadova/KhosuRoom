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

        var myAccount = new Account
        {
            ApiKey = _options.ApiKey,
            ApiSecret = _options.ApiSecret,
            Cloud = _options.CloudName
        };

        _cloudinary = new Cloudinary(myAccount);
        _cloudinary.Api.Secure = true;
    }

    
    public async Task<string> FileUploadAsync(IFormFile file)
    {
        string fileName = string.Concat(Guid.NewGuid(), Path.GetExtension(file.FileName));

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

        return uploadResult.SecureUrl.ToString();
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
        catch
        {
            return false;
        }
    }

   
    public async Task<string> RawUploadAsync(IFormFile file)
    {
        if (file == null || file.Length == 0)
            throw new Exception("File is empty");

        string fileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";

        using var stream = file.OpenReadStream();

        var uploadParams = new RawUploadParams
        {
            File = new FileDescription(fileName, stream),
            Folder = "AssignmentFiles"
        };

        var result = await _cloudinary.UploadAsync(uploadParams);

        if (result.StatusCode != HttpStatusCode.OK)
            throw new Exception("Raw upload failed");

        return result.SecureUrl.ToString();
    }

    public async Task<bool> RawDeleteAsync(string fileUrl)
    {
        try
        {
           
            var publicId = ExtractPublicId(fileUrl);

            var deleteParams = new DeletionParams(publicId)
            {
                ResourceType = ResourceType.Raw
            };

            var res = await _cloudinary.DestroyAsync(deleteParams);
            return res.Result == "ok";
        }
        catch
        {
            return false;
        }
    }

   
    private static string ExtractPublicId(string fileUrl)
    {
        var uri = new Uri(fileUrl);
        var segments = uri.AbsolutePath.Split('/', StringSplitOptions.RemoveEmptyEntries);

       
        var versionIndex = Array.FindIndex(segments, s =>
            s.StartsWith("v") && s.Length > 1 && s.Skip(1).All(char.IsDigit));

        if (versionIndex < 0)
            throw new Exception("Invalid cloudinary url");

        var publicIdWithExt = string.Join("/", segments.Skip(versionIndex + 1));
        var withoutExt = Path.Combine(
            Path.GetDirectoryName(publicIdWithExt) ?? "",
            Path.GetFileNameWithoutExtension(publicIdWithExt)
        ).Replace("\\", "/");

        return withoutExt;
    }
}
