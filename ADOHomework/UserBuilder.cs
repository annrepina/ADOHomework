using ADOHomework.Model.basics;
using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADOHomework
{
	public class UserBuilder : Builder<User>
    {
        public UserBuilder()
        {
            _random = new Random(); 
        }

        public const long MinNumber = 1000000000;
        public const long MaxNumber = 9999999999;

        private static string[] userNames = { "John", "Jane", "Ann", "Karen", "William", "Tom", "Lilly", "Keira", "Violet", "Andrew" };

        private static string[] userLastNames = {"Johnson", "Dean", "Kelly", "Peacock", "Kash", "Haig", "Dickinson", "Finch", "Gill", "Mercer" };

        private void SetNameRandomly()
        {
            int nameindex = _random.Next(userNames.Length - 1);
            int lastnameindex = _random.Next(userNames.Length - 1);

            string name = userNames[nameindex] + userLastNames[lastnameindex];

            _element.Name = name;
        }

        private void SetPhoneNumberRandomly()
        {
            string phoneNumber = "8";

            long number = _random.NextInt64(MinNumber, MaxNumber);

            phoneNumber = phoneNumber + number.ToString();

            _element.PhoneNumber = phoneNumber;
        }

        public void BuildUserRandomly()
        {
            SetNameRandomly();
            SetPhoneNumberRandomly();
        }

        private Random _random;
    }
}
