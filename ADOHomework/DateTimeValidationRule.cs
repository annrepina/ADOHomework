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
    public class DateTimeValidationRule : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            if(value != null && value.ToString().Length > 0)
            {
                DateTime date;

                string[] dateFormats = { "MM/dd/yyyy HH:mm:ss", "M/dd/yyyy HH:mm:ss", "M/d/yyyy HH:mm:ss", "MM/d/yyyy HH:mm:ss" };

                string dateStr = value.ToString();

                string timeOfDay = "";

                if (dateStr.Contains(" AM"))
                    timeOfDay = " AM";
                else if(dateStr.Contains(" PM"))
                    timeOfDay = " PM";

                int index = dateStr.IndexOf(timeOfDay);

                // удаляю обозначение суток
                dateStr = dateStr.Remove(index);

                try
                {
                    date = DateTime.ParseExact(dateStr, dateFormats, CultureInfo.InvariantCulture);
                    return ValidationResult.ValidResult;
                }
                catch (Exception e)
                {
                    MessageBox.Show("Date is incorrect. Please enter mm/dd/yyyy hh:mm:ss AM/PM");
                    return new ValidationResult(false, e.Message);
                }
            }
            else
            {
                MessageBox.Show("Date is null or empty");
                return new ValidationResult(false, "Date is null or empty");
            }
        }
    }
}
