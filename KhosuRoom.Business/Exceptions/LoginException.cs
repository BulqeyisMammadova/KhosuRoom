using KhosuRoom.Business.Abstarctions;

namespace KhosuRoom.Business.Exceptions;

public class LoginException(string message = "User not found") : Exception(message), IBaseException
{
    public int StatusCode { get; set; } = 404;

}
public class ForbiddenException(string message = "User not authorize") : Exception(message), IBaseException
{
    public int StatusCode { get; set; } = 403;

}





