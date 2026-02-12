using KhosuRoom.Business.Abstarctions;

namespace KhosuRoom.Business.Exceptions;

public class AlreadyException(string message="Object is already exists") : Exception(message), IBaseException
{
    public int StatusCode { get; set; } = 409;

}
