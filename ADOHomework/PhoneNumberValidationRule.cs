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
    /// Правило проверки номера телефона пользователя на коректность
    /// </summary>
    public class PhoneNumberValidationRule : ValidationRule
    {
        /// <summary>
        /// Константа максимальной большой по значению номер телефона в виде long
        /// </summary>
        public const long MaxNumber = 89999999999;

        /// <summary>
        /// Проверяет номер телефона пользователя на коректность
        /// </summary>
        /// <param name="value">Проверяемое значение</param>
        /// <param name="cultureInfo">Информация о культуре</param>
        /// <returns></returns>
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            long phoneNumber = 0L;

            bool success = false;

            Regex regex = new Regex(@"^8\d{10}");

			try
			{			
				if (value != null && value.ToString().Length > 0 && Int64.Parse(value.ToString()) <= MaxNumber)
					success = Int64.TryParse((string)value, out phoneNumber);
			}
			catch (Exception e)
			{
                MessageBox.Show("Illegal characters or too long number");
				return new ValidationResult(false, "Illegal characters or too long number");
			}

			if (regex.IsMatch(phoneNumber.ToString()))
            {
                return ValidationResult.ValidResult;
            }
            else
            {
                MessageBox.Show("Phone number is not correct. It has to begin from '8' and has 11 digits.");
                return new ValidationResult(false, "Phone number is not correct. It has to begin from '8' and has 11 digits.");
            }
        }
    }
}
