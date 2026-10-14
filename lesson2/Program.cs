using System.Diagnostics;
using System.Collections.Concurrent;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using lesson2.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorPages();
builder.Services.AddDbContext<TaskDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("TaskDb")));

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<TaskDbContext>();
    db.Database.EnsureCreated();
}

var requestTimestamps = new ConcurrentDictionary<string, Queue<DateTime>>();

app.Use(async (context, next) =>
{
    var clientIp = context.Connection.RemoteIpAddress?.ToString() ?? "unknown";
    if (clientIp != "::1" && clientIp != "127.0.0.1")
    {
        var now = DateTime.Now;
        var timestamps = requestTimestamps.GetOrAdd(clientIp, _ => new Queue<DateTime>());
        while (timestamps.Count > 0 && now.Subtract(timestamps.Peek()).TotalSeconds > 10)
            timestamps.Dequeue();
        if (timestamps.Count >= 5)
        {
            context.Response.StatusCode = 429;
            await context.Response.WriteAsync(ApiError.Error("Слишком много запросов. Попробуйте позже.").Value);
            return;
        }
        timestamps.Enqueue(now);
    }
    await next();
});

app.Use(async (context, next) =>
{
    var maintenanceHeader = context.Request.Headers["X-Maintenance"].FirstOrDefault();
    if (maintenanceHeader == "true")
    {
        var path = context.Request.Path.Value ?? "";
        if (!path.StartsWith("/css") && !path.StartsWith("/js") && !path.EndsWith(".html"))
        {
            context.Response.StatusCode = 503;
            await context.Response.WriteAsync(ApiError.Error("Сервер временно недоступен, ведутся технические работы.").Value);
            return;
        }
    }
    await next();
});

app.UseExceptionHandler(errorApp =>
{
    errorApp.Run(async context =>
    {
        var exception = context.Features.Get<IExceptionHandlerFeature>()?.Error;
        Console.WriteLine($"[ERROR] {DateTime.Now:HH:mm:ss} Exception: {exception?.Message}");
        if (exception?.StackTrace != null)
            Console.WriteLine(exception.StackTrace);
        if (context.Request.Path.StartsWithSegments("/api"))
        {
            context.Response.StatusCode = 500;
            context.Response.ContentType = "application/json; charset=utf-8";
            await context.Response.WriteAsync(ApiError.Error("Внутренняя ошибка сервера").Value);
        }
        else
        {
            context.Response.Redirect("/Error", permanent: false);
        }
    });
});

app.Use(async (context, next) =>
{
    var startTime = DateTime.Now;
    Console.WriteLine($"[{startTime:HH:mm:ss}] {context.Request.Method} {context.Request.Path}");
    await next();
    var endTime = DateTime.Now;
    var duration = (endTime - startTime).TotalMilliseconds;
    Console.WriteLine($"[{endTime:HH:mm:ss}] ← {context.Response.StatusCode} ({Math.Round(duration)} ms)");
});

app.Use(async (context, next) =>
{
    if (context.Request.Path.StartsWithSegments("/api/tasks") &&
        (context.Request.Method == "POST" || context.Request.Method == "DELETE"))
    {
        var apiKey = context.Request.Headers["X-Api-Key"].FirstOrDefault();
        if (apiKey != "lesson2-secret")
        {
            context.Response.StatusCode = 401;
            await context.Response.WriteAsync(ApiError.Unauthorized().Value);
            return;
        }
    }
    await next();
});

app.UseDefaultFiles();
app.UseStaticFiles();

// ========== API ЭНДПОИНТЫ ==========
app.MapGet("/api/hello", () => "Привет! Сервер работает.");

app.MapGet("/api/server-info", () =>
    Results.Json(new
    {
        serverName = "Lesson1.WebServer",
        version = "1.0",
        currentTime = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss")
    }));

app.MapGet("/api/sum", (string? a, string? b) => {
    if (double.TryParse(a, out double numA) && double.TryParse(b, out double numB))
        return Results.Json(new { a = numA, b = numB, result = numA + numB });
    return ApiError.BadRequest("Некорректные параметры");
});

app.MapPost("/api/echo", ([Microsoft.AspNetCore.Mvc.FromBody] EchoRequest? req) => {
    var text = req?.Text ?? "";
    return Results.Json(new { original = text, uppercase = text.ToUpper(), length = text.Length });
});

app.MapGet("/api/divide", (string? a, string? b) => {
    if (!double.TryParse(a, out double numA) || !double.TryParse(b, out double numB))
        return ApiError.BadRequest("Некорректные параметры");
    if (numB == 0)
        return ApiError.BadRequest("Деление на ноль невозможно");
    return Results.Json(new { result = numA / numB });
});

// Задача 2: GET /api/tasks — возвращаем TaskDto без циклов
app.MapGet("/api/tasks", async (TaskDbContext db, int? categoryId, string sortBy = "date", int page = 1, int pageSize = 5, string? search = null) =>
{
    var query = db.Tasks
        .Include(t => t.Category)
        .AsQueryable();

    // Фильтрация по категории
    if (categoryId.HasValue)
        query = query.Where(t => t.CategoryId == categoryId);

    // Поиск по названию (ДЗ занятия 8)
    if (!string.IsNullOrWhiteSpace(search))
        query = query.Where(t => t.Title.Contains(search));

    // Сортировка
    if (sortBy == "title")
        query = query.OrderBy(t => t.Title);
    else
        query = query.OrderByDescending(t => t.CreatedAt);

    // Пагинация (Задача 8)
    var totalCount = await query.CountAsync();
    var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);
    var tasks = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();

    var taskDtos = tasks.Select(t => new TaskDto(
        t.Id,
        t.Title,
        t.IsDone,
        t.CreatedAt,
        t.Category?.Name,
        t.DueDate
    )).ToArray();

    return Results.Ok(new PaginatedResponse<TaskDto>(taskDtos, page, pageSize, totalCount, totalPages));
});

// Задача 2: GET /api/tasks/{id}
app.MapGet("/api/tasks/{id:int}", async (int id, TaskDbContext db) =>
{
    var task = await db.Tasks
        .Include(t => t.Category)
        .FirstOrDefaultAsync(t => t.Id == id);

    if (task == null)
        return ApiError.NotFound("Задача не найдена");

    var dto = new TaskDto(
        task.Id,
        task.Title,
        task.IsDone,
        task.CreatedAt,
        task.Category?.Name,
        task.DueDate
    );

    return Results.Ok(dto);
});

// Задача 3: POST /api/tasks — используем CreateTaskDto для защиты от overposting
app.MapPost("/api/tasks", async ([Microsoft.AspNetCore.Mvc.FromBody] CreateTaskDto? req, TaskDbContext db) =>
{
    if (req == null)
        return ApiError.BadRequest("Некорректный запрос");

    // Валидация через DataAnnotations (занятие 7)
    var tempTask = new TaskItem { Title = req.Title.Trim(), CategoryId = req.CategoryId };
    var results = new List<ValidationResult>();
    bool isValid = Validator.TryValidateObject(tempTask, new ValidationContext(tempTask), results, true);

    if (!isValid)
    {
        var errorMessages = results.Select(r => r.ErrorMessage).Where(e => e != null).ToArray();
        return ApiError.ValidationErrors(errorMessages);
    }

    var newTask = new TaskItem
    {
        Title = tempTask.Title,
        IsDone = false, // Игнорируем клиентский isDone
        CategoryId = tempTask.CategoryId,
        DueDate = null,
        CreatedAt = DateTime.Now
    };
    db.Tasks.Add(newTask);
    await db.SaveChangesAsync();

    // Задача 7: Results.Created с Location header
    var dto = new TaskDto(
        newTask.Id,
        newTask.Title,
        newTask.IsDone,
        newTask.CreatedAt,
        newTask.Category?.Name,
        newTask.DueDate
    );
    return Results.Created($"/api/tasks/{newTask.Id}", dto);
});

// Задача 4: PUT /api/tasks/{id} — UpdateTaskDto
app.MapPut("/api/tasks/{id:int}", async (int id, [Microsoft.AspNetCore.Mvc.FromBody] UpdateTaskDto? req, TaskDbContext db) =>
{
    if (req == null)
        return ApiError.BadRequest("Некорректный запрос");

    var task = await db.Tasks.FindAsync(id);
    if (task == null)
        return ApiError.NotFound("Задача не найдена");

    // Обновляем только разрешённые поля
    task.Title = req.Title.Trim();
    task.IsDone = req.IsDone;
    task.CategoryId = req.CategoryId;

    await db.SaveChangesAsync();

    var dto = new TaskDto(
        task.Id,
        task.Title,
        task.IsDone,
        task.CreatedAt,
        task.Category?.Name,
        task.DueDate
    );

    return Results.Ok(dto);
});

// Задача 5: GET /api/categories
app.MapGet("/api/categories", async (TaskDbContext db) =>
{
    var categories = await db.Categories
        .Include(c => c.Tasks)
        .Select(c => new CategoryDto(c.Id, c.Name, c.Tasks.Count))
        .ToListAsync();

    return Results.Ok(categories);
});

app.MapDelete("/api/tasks/{id:int}", async (int id, TaskDbContext db) =>
{
    var task = await db.Tasks.FindAsync(id);
    if (task == null)
        return ApiError.NotFound("Задача не найдена");

    db.Tasks.Remove(task);
    await db.SaveChangesAsync();

    // Задача 7: 204 No Content вместо 200 с телом
    return Results.NoContent();
});

app.MapGet("/api/crash-test", () => {
    throw new Exception("Тестовая ошибка");
});

app.MapRazorPages();

app.Run();

public class EchoRequest { public string Text { get; set; } = ""; }
