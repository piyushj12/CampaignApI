using System;
using System.ComponentModel.DataAnnotations;

namespace Campaigns.Models
{
	public class DateValidation : ValidationAttribute
	{

		private readonly string _startDatePropertyName;

		public DateValidation(string startDatePropertyName)

		{
			_startDatePropertyName = startDatePropertyName;
		}

        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
			var startDateProperty = validationContext.ObjectType.GetProperty(_startDatePropertyName);
			if(startDateProperty == null)
			{
				return new ValidationResult($"Unknown Property: {_startDatePropertyName}");
			}

			var startDate = (DateTime)startDateProperty.GetValue(validationContext.ObjectInstance);
			var endDate = (DateTime)value;

			if(startDate > endDate)
			{
				return new ValidationResult(GetErrorMessage());
			}

			return ValidationResult.Success;
        }

		public string GetErrorMessage()
		{
			return $"End Date must be greater than Start Date.";
		}
    }
}

