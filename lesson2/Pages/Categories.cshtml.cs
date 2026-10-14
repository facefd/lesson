using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using lesson2.Models;

namespace lesson2.Pages;

[IgnoreAntiforgeryToken]
public class CategoriesModel : PageModel
{
    private readonly TaskDbContext _db;

    public CategoriesModel(TaskDbContext db) => _db = db;

    [BindProperty(SupportsGet = true)]
    public List<CategoryItemViewModel> Categories { get; set; } = new();

    [BindProperty]
    public Category NewCategory { get; set; } = new();

    // Нагрузка всех категорий с количеством связанных задач
    public async Task OnGet()
    {
        Categories = await _db.Categories
            .Include(c => c.Tasks)
            .Select(c => new CategoryItemViewModel
            {
                Id = c.Id,
                Name = c.Name,
                TaskCount = c.Tasks.Count
            })
            .ToListAsync();
    }

    // Добавление новой категории
    public async Task<IActionResult> OnPost()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        var category = new Category
        {
            Name = NewCategory.Name.Trim(),
            Tasks = new List<TaskItem>()
        };
        _db.Categories.Add(category);
        await _db.SaveChangesAsync();

        TempData["Message"] = $"Категория «{category.Name}» создана!";
        return RedirectToPage();
    }

    // Удаление категории (Задача 11)
    public async Task<IActionResult> OnPostDelete(int id)
    {
        var category = await _db.Categories.FindAsync(id);
        if (category == null)
        {
            TempData["Error"] = "Категория не найдена";
            return RedirectToPage();
        }

        string categoryName = category.Name;
        
        // При удалении задачи получают CategoryId = null (благодаря OnDelete(DeleteBehavior.SetNull))
        _db.Categories.Remove(category);
        await _db.SaveChangesAsync();

        TempData["Message"] = $"Категория «{categoryName}» удалена. Связанные задачи стали без категории.";
        return RedirectToPage();
    }

    // ViewModel для отображения категории с количеством задач
    public class CategoryItemViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public int TaskCount { get; set; }
    }
}
