using KhosuRoom.Business.Abstarctions;

namespace KhosuRoom.Business.Exceptions;

public class BadRequestException(string message) : Exception(message), IBaseException
{
    public int StatusCode { get; set; } = 400;

}
