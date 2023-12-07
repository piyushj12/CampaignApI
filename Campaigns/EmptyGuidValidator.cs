using System;
using System.ComponentModel.DataAnnotations;

namespace Campaigns.Api
{
	public class EmptyGuidValidator: ValidationAttribute
	{
		public EmptyGuidValidator()
		{
		}

        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if(!(value is Guid guidValue) || (guidValue == Guid.Empty))
            {
                return new ValidationResult(" Guid cannot be emplty");
            }

            return ValidationResult.Success;

        }
    }
}

