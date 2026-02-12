namespace KhosuRoom.Business.Dtos.ResultDtos;

public class ResultDto
{
    public ResultDto()
    {
        Message = "Successfully";
        IsSucced = true;
        StatusCode = 200;
    }
    public ResultDto(string message)
    {
        Message = message;
    }

    public ResultDto(bool success, string message)
    {
        Message = message;
        IsSucced = success;
    }
    public ResultDto(int statusCode, bool success, string message)
    {
        StatusCode = statusCode;
        IsSucced = success;
        Message = message;
    }
    public ResultDto(int statusCode, string message)
    {
        StatusCode = statusCode;
        Message = message;
    }

    public int StatusCode { get; set; } = 200;
    public bool IsSucced { get; set; } = true;
    public string Message { get; set; } = "Successfully";
}
public class ResultDto<T> : ResultDto
{
    public T? Data { get; set; }

    public ResultDto(T data) : base()
    {
        Data = data;
    }

    public ResultDto() : base()
    {
    }
}