

namespace EventHub.Core.EventValidation
{
    using System;
    using System.ComponentModel.DataAnnotations;
    public class DateGreaterThanAttribute : ValidationAttribute
    {
        private readonly string _comparisonProperty;


        public DateGreaterThanAttribute(string comparisonProperty)
        {
            _comparisonProperty = comparisonProperty;
        }

        protected override ValidationResult? IsValid(object value, ValidationContext validationContext)
        {
            if (value == null) return ValidationResult.Success; // Required validation will handle this

            var property = validationContext.ObjectType.GetProperty(_comparisonProperty);

            if(property == null)
            {
                return new ValidationResult($"Property {_comparisonProperty} not found");

            }

            var comprasionValue = property.GetValue(validationContext.ObjectInstance);

            if (value is DateTime endDate && comprasionValue is DateTime startDate)
            {
                if (endDate < startDate)
                {
                    return new ValidationResult(ErrorMessage ?? "End date must be greater than start date");
                }
            }

               


            return ValidationResult.Success;
        }
    }
}
