namespace lesson2.Models;

// Задача 2: DTO для чтения задачи — только read-only поля без навигационных свойств
// Чтобы избежать циклической сериализации (см. Задачу 1) возвращаем categoryName текстом, а не объект Category
public record TaskDto(
    int Id, 
    string Title, 
    bool IsDone, 
    DateTime CreatedAt, 
    string? CategoryName,
    DateTime? DueDate = null
);

// Задача 3: DTO для создания задачи — клиент может передать только то, что ему разрешено
// IsDone и CreatedAt НЕ включены — сервер сам их установит
public record CreateTaskDto(string Title, int? CategoryId);

// Задача 4: DTO для обновления задачи
public record UpdateTaskDto(string Title, bool IsDone, int? CategoryId);

// Задача 5: DTO для категории
public record CategoryDto(int Id, string Name, int TaskCount);

// ДЗ Урока 8 + Задача 8: DTO для пагинированного ответа
public record PaginatedResponse<T>(
    T[] Items, 
    int Page, 
    int PageSize, 
    int TotalCount, 
    int TotalPages
);
