using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace ADOHomework
{
    public class SummValidationRule : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            bool success = false;

            int res = 0;

            // если строка не пустая
            if (value != null && value.ToString().Length > 0)
            {
                success = Int32.TryParse(value.ToString(), out res);
            }

            else
            {
                MessageBox.Show("Summ is null or empty");
                return new ValidationResult(false, "Summ is null or empty");
            }


            if (success)
            {
                if (res > 0)
                {
                    return ValidationResult.ValidResult;
                }
                else
                {
                    MessageBox.Show("Summ has to be positive");
                    return new ValidationResult(false, "Summ has to be positive");
                }
            }


            else
            {
                MessageBox.Show("Input value has illegal characters or too long");
                return new ValidationResult(false, "Input value has illegal characters");
            }

        }
    }
}
