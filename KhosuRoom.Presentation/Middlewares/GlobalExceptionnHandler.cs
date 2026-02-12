using KhosuRoom.Business.Abstarctions;
using KhosuRoom.Business.Dtos.ResultDtos;

namespace KhosuRoom.Presentation.Middlewares;

public class GlobalExceptionnHandler
{
    private readonly RequestDelegate _next;
    public GlobalExceptionnHandler(RequestDelegate next, ILogger<GlobalExceptionnHandler> logger)
    {
        _next = next;

    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            ResultDto error = new()
            {
                IsSucced = false,
                StatusCode = 500,
                Message = "Internal Server Error"
            };
            if(ex is IBaseException baseException)
            {
                error.StatusCode = baseException.StatusCode;
                error.Message = ex.Message;
            }
            await context.Response.WriteAsJsonAsync(error);
        }
    }
}
