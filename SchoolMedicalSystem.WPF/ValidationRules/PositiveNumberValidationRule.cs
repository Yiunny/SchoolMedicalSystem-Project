using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace SchoolMedicalSystem.WPF.ValidationRules
{
    public class PositiveNumberValidationRule : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            if (double.TryParse(value as string, out double number))
            {
                if (number > 0)
                {
                    return ValidationResult.ValidResult;
                }
                return new ValidationResult(false, "Giá trị phải là một số dương.");
            }
            return new ValidationResult(false, "Vui lòng chỉ nhập số.");
        }
    }
}
