namespace lesson2.Models;

public record TaskDto(
    int Id, 
    string Title, 
    bool IsDone, 
    DateTime CreatedAt, 
    string? CategoryName,
    DateTime? DueDate = null
);

public record CreateTaskDto(string Title, int? CategoryId);

public record UpdateTaskDto(string Title, bool IsDone, int? CategoryId);

public record CategoryDto(int Id, string Name, int TaskCount);

public record PaginatedResponse<T>(
    T[] Items, 
    int Page, 
    int PageSize, 
    int TotalCount, 
    int TotalPages
);
