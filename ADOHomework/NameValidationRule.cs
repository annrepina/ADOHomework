using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace ADOHomework
{
    public class NameValidationRule : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            // Имя  фамилия с большой буквы и между ними пробел, например Ann Repina
            Regex regex = new Regex(@"^[A-Z]{1}[a-z]*\s[A-Z]{1}[a-z]*");

            if (value != null && value.ToString().Length > 0)
            {
                if (regex.IsMatch(value.ToString()))
                {
                    return ValidationResult.ValidResult;
                }

                else
                {
                    MessageBox.Show("Name has to contain firstname and lastname, which start with upper case. Name can't contain digits");
                    return new ValidationResult(false, "Name has to contain firstname and lastname, which start with upper case.");
                }
            }

            else
            {
                MessageBox.Show("Name is null or empty");
                return new ValidationResult(false, "Name is null or empty");
            }

        }
    }
}
