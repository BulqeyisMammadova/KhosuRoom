namespace KhosuRoom.Business.Services.Abstractions;

public interface IAIService
{
    Task<List<string>> GenerateSimilarTasksAsync(string assignmentTitle, string? assignmentDescription);
}
