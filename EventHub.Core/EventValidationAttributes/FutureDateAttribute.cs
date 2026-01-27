using System.ComponentModel.DataAnnotations;

namespace EventHub.Core.EventValidation
{
    public class FutureDateAttribute : ValidationAttribute
    {
        public FutureDateAttribute()
          : base("Date must grater than start date")
        {
        }
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {

            if (value == null) return ValidationResult.Success; // Required validation will handle this

            if (value is not DateTime dateValue)
                return new ValidationResult("Invalid date");

            if (dateValue <= DateTime.Now)
                return new ValidationResult(ErrorMessage ?? "Date must be in the future");



            return ValidationResult.Success;
        }
    }
}
