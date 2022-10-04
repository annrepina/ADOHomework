using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace ADOHomework
{
    public class PhoneNumberValidationRule : ValidationRule
    {
        public const long MaxNumber = 89999999999;

		public PhoneNumberValidationRule()
		{
            _errorMessage = "";

        }

        private string _errorMessage;
        public string ErrorMessage
        {
            get { return _errorMessage; }
            set { _errorMessage = value; }
        }

        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            long phoneNumber = 0L;

            bool success = false;

            Regex regex = new Regex(@"^8\d{10}");

			//string str = value.ToString();

			//Model.Wrappers.UserTableItem userTableItem = (Model.Wrappers.UserTableItem)value;

			try
			{
				// если строка не пустая
				if (value != null && value.ToString().Length > 0)
					success = Int64.TryParse((string)value, out phoneNumber);
			}
			catch (Exception e)
			{
				return new ValidationResult(false, $"Illegal characters or {e.Message}");
			}

			if (regex.IsMatch(phoneNumber.ToString()))
            {
                //return new ValidationResult(true, regex.Match(phoneNumber.ToString()).ToString());

                return ValidationResult.ValidResult;
            }
            else
                return new ValidationResult(false, "Phone number is not correct. It has to begin from '8' and has 11 digits.");
        }
    }
}
