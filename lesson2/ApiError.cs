using System.Text.Json;

namespace lesson2;

// Задача 6: Единый формат ошибок для всего API
public static class ApiError
{
    public static Results.JsonHttpResult Error(string message, string code = "UNKNOWN_ERROR") =>
        Results.Json(new { error = new { message, code } }, statusCode: 500);

    public static Results.JsonHttpResult BadRequest(string message, string code = "VALIDATION_ERROR") =>
        Results.Json(new { error = new { message, code } }, statusCode: 400);

    public static Results.JsonHttpResult NotFound(string message, string code = "RESOURCE_NOT_FOUND") =>
        Results.Json(new { error = new { message, code } }, statusCode: 404);

    public static Results.JsonHttpResult Unauthorized(string message = "Неавторизованный запрос", string code = "UNAUTHORIZED") =>
        Results.Json(new { error = new { message, code } }, statusCode: 401);

    // Для валидации из занятия 7 (список ошибок)
    public static Results.JsonHttpResult ValidationErrors(IEnumerable<string> errors, string code = "VALIDATION_ERROR") =>
        Results.Json(new { error = new { messages = errors, code } }, statusCode: 400);
}
