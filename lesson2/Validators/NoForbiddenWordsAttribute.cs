using System.ComponentModel.DataAnnotations;

namespace lesson2.Validators;

public class NoForbiddenWordsAttribute : ValidationAttribute
{
    private readonly string[] _forbiddenWords = { "тест", "тестовый", "check" };

    protected override ValidationResult IsValid(object? value, ValidationContext validationContext)
    {
        if (value is string str && !string.IsNullOrWhiteSpace(str))
        {
            var lower = str.ToLower();
            foreach (var word in _forbiddenWords)
            {
                if (lower.Contains(word))
                {
                    return new ValidationResult($"Название не должно содержать слово '{word}'");
                }
            }
        }
        return ValidationResult.Success!;
    }
}
