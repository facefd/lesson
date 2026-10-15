using System;
using System.ComponentModel.DataAnnotations;
using lesson2.Validators;

namespace lesson2.Models
{
    public class TaskItem
    {
        public int Id { get; set; }
        
        [Required(ErrorMessage = "Название обязательно")]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "От 3 до 100 символов")]
        [NoForbiddenWords]
        public string Title { get; set; } = "";

        public bool IsDone { get; set; } = false;

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public int? CategoryId { get; set; }
        public Category? Category { get; set; }

        [Display(Name = "Дедлайн")]
        [DataType(DataType.Date)]
        [MustBeFutureOrToday(ErrorMessage = "Дата дедлайна не может быть в прошлом")]
        public DateTime? DueDate { get; set; }

        public override string ToString() => $"Task({Id}): '{Title}' (done: {IsDone}, cat: {CategoryId ?? -1}, due: {DueDate?.ToString("dd.MM.yyyy") ?? "—"})";
    }
}
