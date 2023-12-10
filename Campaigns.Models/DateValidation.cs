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
            if (startDateProperty == null)
            {
                return new ValidationResult($"Unknown Property: {_startDatePropertyName}");
            }

            var startDateValue = startDateProperty.GetValue(validationContext.ObjectInstance);

            // Check if the startDate is a nullable DateTime and if it has a value
            if (startDateValue == null || !(startDateValue is DateTime startDate))
            {
                // Handle the case where startDate is null or not a DateTime
                return ValidationResult.Success; // Or return an error if StartDate is required
            }

            // Check if the value (endDate) is a nullable DateTime and if it has a value
            if (value == null || !(value is DateTime endDate))
            {
                // Handle the case where endDate is null or not a DateTime
                return ValidationResult.Success; // Or return an error if EndDate is required
            }

            if (startDate > endDate)
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

