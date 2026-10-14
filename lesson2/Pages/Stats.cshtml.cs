using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using lesson2.Models;

public class StatsModel : PageModel
{
    private readonly TaskDbContext _db;

    public StatsModel(TaskDbContext db) => _db = db;

    public int TotalCount { get; set; }
    public int DoneCount { get; set; }
    public int UndoneCount { get; set; }

    // Задача 8: Группировка задач по категориям
    public List<CategoryStatViewModel> ByCategory { get; set; } = new();

    public async Task OnGet()
    {
        // Общая статистика
        TotalCount = await _db.Tasks.CountAsync();
        DoneCount = await _db.Tasks.CountAsync(t => t.IsDone);
        UndoneCount = TotalCount - DoneCount;

        // Задача 8: Группировка по категориям
        ByCategory = await _db.Tasks
            .Where(t => t.Category != null) // только задачи с категорией
            .GroupBy(t => new { t.Category.Name, t.CategoryId })
            .Select(g => new CategoryStatViewModel
            {
                CategoryName = g.Key.Name,
                CategoryId = g.Key.CategoryId,
                Count = g.Count(),
                DoneCount = g.Count(t => t.IsDone),
                UndoneCount = g.Count(t => !t.IsDone)
            })
            .OrderByDescending(x => x.Count)
            .ToListAsync();
    }

    public IActionResult OnPost()
    {
        TempData["Message"] = "Статистика обновлена вручную";
        return RedirectToPage("/Stats");
    }

    public class CategoryStatViewModel
    {
        public string CategoryName { get; set; } = "";
        public int CategoryId { get; set; }
        public int Count { get; set; }
        public int DoneCount { get; set; }
        public int UndoneCount { get; set; }
    }
}
