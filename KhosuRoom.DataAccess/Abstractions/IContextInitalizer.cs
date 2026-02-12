namespace KhosuRoom.DataAccess.Abstractions;

public interface IContextInitalizer
{
    Task InitDatabaseAsync();
}
