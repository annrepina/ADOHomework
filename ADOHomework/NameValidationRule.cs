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
    /// <summary>
    /// Правило проверки имени пользователя на коректность
    /// </summary>
    public class NameValidationRule : ValidationRule
    {
        /// <summary>
        /// Проверяет имя пользователя на корректность
        /// </summary>
        /// <param name="value">Проверяемое значение</param>
        /// <param name="cultureInfo">Информация о культуре</param>
        /// <returns></returns>
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            // Имя  фамилия с большой буквы и между ними пробел, например Ann Repina
            Regex regex = new Regex(@"^[A-Z]{1}[a-z]*\s[A-Z]{1}[a-z]*$");

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
