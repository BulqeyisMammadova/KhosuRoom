using KhosuRoom.Business.Abstarctions;

namespace KhosuRoom.Business.Exceptions;

public class NotFoundExceptions(string message="Object is not found") : Exception(message), IBaseException
{
    public int StatusCode { get; set; } = 404;

}
