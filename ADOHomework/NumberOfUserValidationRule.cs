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
    public class NumberOfUserValidationRule : ValidationRule
    {
        private int _maxNumber;

        public NumberOfUserValidationRule()
        {
            _maxNumber = 0;
        }

        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            //// Число
            //Regex regex = new Regex(@"\d*");

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
