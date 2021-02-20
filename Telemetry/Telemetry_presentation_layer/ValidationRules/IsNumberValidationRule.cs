using System.Globalization;
using System.Windows.Controls;

namespace Telemetry_presentation_layer.ValidationRules
{
    public class IsNumberValidationRule : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            if (string.IsNullOrWhiteSpace((value ?? "").ToString()))
            {
                return new ValidationResult(false, "Field is required.");
            }
            else
            {
                if (int.TryParse(value.ToString(), out int result))
                {
                    return ValidationResult.ValidResult;
                }
                else
                {
                    return new ValidationResult(false, "Number is required.");
                }
            }
        }
    }
}