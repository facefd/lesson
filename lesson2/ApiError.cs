using System.Text.Json;

namespace lesson2;

public static class ApiError
{
    public static IResult Error(string message, string code = "UNKNOWN_ERROR") =>
        Results.Json(new { error = new { message, code } }, statusCode: 500);

    public static IResult BadRequest(string message, string code = "VALIDATION_ERROR") =>
        Results.Json(new { error = new { message, code } }, statusCode: 400);

    public static IResult NotFound(string message, string code = "RESOURCE_NOT_FOUND") =>
        Results.Json(new { error = new { message, code } }, statusCode: 404);

    public static IResult Unauthorized(string message = "Неавторизованный запрос", string code = "UNAUTHORIZED") =>
        Results.Json(new { error = new { message, code } }, statusCode: 401);

    public static IResult ValidationErrors(IEnumerable<string> errors, string code = "VALIDATION_ERROR") =>
        Results.Json(new { error = new { messages = errors, code } }, statusCode: 400);
}
