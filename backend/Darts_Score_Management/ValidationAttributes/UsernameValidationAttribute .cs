using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace Darts_Score_Management.ValidationAttributes
{
    public class UsernameValidationAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var username = value as string;

            if (string.IsNullOrWhiteSpace(username))
            {
                return new ValidationResult("Username cannot be empty or consist only of whitespace.");
            }

            username = username.Trim();

            if (username.Length < 3)
            {
                return new ValidationResult("Username must be at least 3 characters long.");
            }

            // Regex: Alphanumeric, underscore, no special characters except underscore
            var regex = new Regex(@"^[A-Za-z0-9_]+$");
            if (!regex.IsMatch(username))
            {
                return new ValidationResult("Username can only contain letters, numbers, and underscores.");
            }

            return ValidationResult.Success;
        }
    }
}

