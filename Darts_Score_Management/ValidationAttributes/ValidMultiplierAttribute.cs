//using System.ComponentModel.DataAnnotations;

//namespace Darts_Score_Management.ValidationAttributes
//{
//    public class ValidMultiplierAttribute : ValidationAttribute
//    {
//        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
//        {
//            if (value is int multiplier)
//            {
//                if (multiplier >= 1 && multiplier <= 3)
//                    return ValidationResult.Success;

//                return new ValidationResult("Invalid multiplier. Must be between 1-3.");
//            }
//            return new ValidationResult("Multiplier must be an integer.");
//        }
//    }
//}
