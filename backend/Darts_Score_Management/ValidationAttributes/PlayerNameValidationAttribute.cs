using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace Darts_Score_Management.ValidationAttributes
{
    public class PlayerNameValidationAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var name = value as string;

            if (string.IsNullOrWhiteSpace(name))
            {
                return new ValidationResult("Name cannot be empty or consist only of whitespace.");
            }

            name = name.Trim(); 

            if (name.Length < 2)
            {
                return new ValidationResult("Name must be at least 2 characters long.");
            }

            // Regex: 
            // - Must contain at least one letter
            // - Allow max one number
            // - Disallow special characters like %$£
            // - Allow spaces, hyphens, apostrophes
            var regex = new Regex(@"^(?=.*[A-Za-z])(?!.*\d.*\d)[A-Za-z0-9\s\-\']*$");
            if (!regex.IsMatch(name))
            {
                return new ValidationResult("Name must contain at least one letter, can have max one number, and can only include letters, single number, spaces, hyphens, or apostrophes.");
            }

            // Additional check to ensure no special characters
            var specialCharsRegex = new Regex(@"[%\$£@#&\*\(\)\[\]\{\}]");
            if (specialCharsRegex.IsMatch(name))
            {
                return new ValidationResult("Name cannot contain special characters like %, $, £, @, #, &, *, (), [], {}.");
            }

            return ValidationResult.Success;
        }
    }
}

