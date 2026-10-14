using System.ComponentModel.DataAnnotations;

namespace lesson2.Validators;

// Домашнее задание: атрибут валидации — дата должна быть сегодня или в будущем
public class MustBeFutureOrTodayAttribute : ValidationAttribute
{
    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    {
        if (value is DateTime dateTime)
        {
            var today = DateTime.Today;
            if (dateTime < today)
            {
                return new ValidationResult("Дата дедлайна не может быть в прошлом");
            }
        }
        // Если значение null (nullable DateTime) — считаем валидным, если поле необязательное
        return ValidationResult.Success;
    }
}
