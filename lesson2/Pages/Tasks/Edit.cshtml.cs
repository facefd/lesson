using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using lesson2.Models;

namespace lesson2.Pages.Tasks;

[IgnoreAntiforgeryToken]
public class EditModel : PageModel
{
    private readonly TaskDbContext _db;

    public EditModel(TaskDbContext db) => _db = db;

    [BindProperty(SupportsGet = true)]
    public int? FilteredCategoryId { get; set; }

    [BindProperty(SupportsGet = true)]
    public string SortBy { get; set; } = "date";

    [BindProperty(SupportsGet = true)]
    public new int Page { get; set; } = 1;

    public List<Category> Categories { get; set; } = new();

    [BindProperty]
    public TaskItem Task { get; set; } = new();

    public async Task<IActionResult> OnGetAsync(int id)
    {
        Categories = await _db.Categories.ToListAsync();

        var task = await _db.Tasks
            .Include(t => t.Category)
            .FirstOrDefaultAsync(t => t.Id == id);
        
        if (task == null)
        {
            TempData["Error"] = "Задача не найдена, возможно, она уже удалена";
            return RedirectToPage("/Tasks", new { categoryId = FilteredCategoryId, sortBy = SortBy, page = Page });
        }
        Task = task;
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid) return Page();
        
        if (string.IsNullOrWhiteSpace(Task.Title))
        {
            ModelState.AddModelError(nameof(Task.Title), "Название обязательно");
            return Page();
        }

        if (Task.IsDone && string.IsNullOrWhiteSpace(Task.Title.Trim()))
        {
            ModelState.AddModelError("", "Нельзя отметить выполненной задачу без названия");
            return Page();
        }

        var task = await _db.Tasks.FindAsync(Task.Id);
        if (task == null)
        {
            TempData["Error"] = "Задача не найдена";
            return RedirectToPage("/Tasks", new { categoryId = FilteredCategoryId, sortBy = SortBy, page = Page });
        }

        task.Title = Task.Title.Trim();
        task.IsDone = Task.IsDone;
        task.CategoryId = Task.CategoryId;
        task.DueDate = Task.DueDate;

        await _db.SaveChangesAsync();

        TempData["Message"] = "Задача обновлена!";
        return RedirectToPage("/Tasks", new { categoryId = FilteredCategoryId, sortBy = SortBy, page = Page });
    }
}
