using System.ComponentModel.DataAnnotations;

namespace Darts_Score_Management.ValidationAttributes
{
    public class ValidDartSegmentAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value is int segment)
            {
                if ((segment >= 1 && segment <= 20) || segment == 25)
                    return ValidationResult.Success;

                return new ValidationResult("Invalid dart segment. Must be between 1-20 or 25 for bullseye.");
            }
            return new ValidationResult("Segment must be an integer.");
        }
    }
}
