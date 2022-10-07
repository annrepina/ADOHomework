using ADOHomework.Model.Wrappers;
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
    /// Правило проверки номера пользователя в таблице на коректность
    /// </summary>
    public class NumberOfUserValidationRule : ValidationRule
    {
        /// <summary>
        /// Максимальный номер пользователя в таблице
        /// </summary>
        private int _maxNumber;

        /// <summary>
        /// Конструктор по умолчанию
        /// </summary>
        public NumberOfUserValidationRule()
        {
            _maxNumber = 0;
        }

        /// <summary>
        /// Проверяет номер пользователя в таблице на коректность
        /// </summary>
        /// <param name="value">Проверяемое значение</param>
        /// <param name="cultureInfo">Информация о культуре</param>
        /// <returns></returns>
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
                MessageBox.Show("Number is null or empty");
                return new ValidationResult(false, "Number is null or empty");
            }

            _maxNumber = OrderTableItem.MaxCustomerNumber;

            if (success)
            {
                if(res > 0 && res <= _maxNumber)
                {
                    return ValidationResult.ValidResult;
                }
                else
                {
                    MessageBox.Show("Number has to be positive and match to users' numbers");
                    return new ValidationResult(false, "Number has to be positive");
                }
            }

            else
            {
                MessageBox.Show("Input value has illegal characters");
                return new ValidationResult(false, "Input value has illegal characters");
            }
        }
    }
}
