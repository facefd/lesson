using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using lesson2.Models;
using static lesson2.Pages.TasksModel;

namespace lesson2.Pages;

[IgnoreAntiforgeryToken]
public class TasksModel : PageModel
{
    private readonly TaskDbContext _db;

    public TasksModel(TaskDbContext db) => _db = db;

    // Параметры из query string (Задачи 7, 9, 10, ДЗ)
    [BindProperty(SupportsGet = true)]
    public int? FilteredCategoryId { get; set; }

    [BindProperty(SupportsGet = true)]
    public string SortBy { get; set; } = "date"; // "date" или "title"

    [BindProperty(SupportsGet = true)]
    public int Page { get; set; } = 1;

    [BindProperty(SupportsGet = true)]
    public string? Search { get; set; } // ДЗ: текстовый поиск по названию

    public const int PageSize = 5; // Задача 9: пагинация

    public List<Category> Categories { get; set; } = new();
    public List<TaskItem> PaginatedTasks { get; set; } = new();
    public int TotalCount { get; set; }
    public int TotalPages { get; set; }

    [BindProperty]
    public TaskItem NewTask { get; set; } = new();

    public async Task OnGet()
    {
        // Загружаем категории для выпадающих списков (Задача 5)
        Categories = await _db.Categories.ToListAsync();

        // Задача 6 + 7: Build query с Include для загрузки Category и фильтрацией
        var query = _db.Tasks.Include(t => t.Category).AsQueryable();

        // Задача 7: Фильтрация по категории
        if (FilteredCategoryId.HasValue)
        {
            query = query.Where(t => t.CategoryId == FilteredCategoryId);
        }

        // ДЗ: Поиск по названию
        if (!string.IsNullOrWhiteSpace(Search))
        {
            query = query.Where(t => t.Title.Contains(Search));
        }

        // Подсчёт общего количества для пагинации
        TotalCount = await query.CountAsync();
        TotalPages = (int)Math.Ceiling(TotalCount / (double)PageSize);

        // Ограничиваем CurrentPage диапазоном
        if (Page < 1) Page = 1;
        if (Page > TotalPages && TotalCount > 0) Page = TotalPages;

        // Задача 10: Сортировка ПОСЛЕ фильтрации, ПЕРЕД пагинацией
        if (SortBy == "title")
        {
            query = query.OrderBy(t => t.Title);
        }
        else // default: date
        {
            query = query.OrderByDescending(t => t.CreatedAt);
        }

        // Задача 9: Пагинация после фильтрации и сортировки
        PaginatedTasks = await query
            .Skip((Page - 1) * PageSize)
            .Take(PageSize)
            .ToListAsync();
    }

    public async Task<IActionResult> OnPost()
    {
        if (!ModelState.IsValid) return Page();
        
        if (string.IsNullOrWhiteSpace(NewTask.Title))
        {
            ModelState.AddModelError(nameof(NewTask.Title), "Название обязательно");
            return Page();
        }

        var newTask = new TaskItem
        {
            Title = NewTask.Title.Trim(),
            IsDone = false,
            CategoryId = NewTask.CategoryId,
            DueDate = NewTask.DueDate,
            CreatedAt = DateTime.Now
        };
        _db.Tasks.Add(newTask);
        await _db.SaveChangesAsync();
        TempData["Message"] = "Задача успешно добавлена!";
        return RedirectToPage("/Tasks", new { 
            categoryId = FilteredCategoryId, 
            sortBy = SortBy, 
            page = Page,
            search = Search
        });
    }

    public async Task<IActionResult> OnPostDelete(int id)
    {
        var task = await _db.Tasks.FindAsync(id);
        if (task != null)
        {
            _db.Tasks.Remove(task);
            await _db.SaveChangesAsync();
            TempData["Message"] = $"Задача №{id} удалена";
        }
        return RedirectToPage("/Tasks", new { 
            categoryId = FilteredCategoryId, 
            sortBy = SortBy, 
            page = Page,
            search = Search
        });
    }
}
