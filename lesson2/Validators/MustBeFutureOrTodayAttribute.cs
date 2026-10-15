using System.ComponentModel.DataAnnotations;

namespace lesson2.Validators;

public class MustBeFutureOrTodayAttribute : ValidationAttribute
{
    protected override ValidationResult IsValid(object? value, ValidationContext validationContext)
    {
        if (value is DateTime dateTime)
        {
            if (dateTime < DateTime.Today)
            {
                return new ValidationResult("Дата дедлайна не может быть в прошлом");
            }
        }
        return ValidationResult.Success!;
    }
}
