using ADOHomework.Model.basics;
using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADOHomework.Builders
{
    /// <summary>
    /// Строитель юзеров - создает юзеры
    /// </summary>
    public class UserBuilder : Builder<User>
    {
        /// <summary>
        /// Конструктор по умолчанию
        /// </summary>
        public UserBuilder()
        {
            _random = new Random();
        }

        /// <summary>
        /// Константа - минимальный номер телефона (его часть после '8')
        /// </summary>
        public const long MinNumber = 1000000000;

        /// <summary>
        /// Константа - максимальный номер телефона (его часть после '8')
        /// </summary>
        public const long MaxNumber = 9999999999;

        /// <summary>
        /// Список имен пользователей для рандомизации
        /// </summary>
        private static string[] userNames = { "John", "Jane", "Ann", "Karen", "William", "Tom", "Lilly", "Keira", "Violet", "Andrew" };

        /// <summary>
        /// Список фамилий пользователей для рандомизации
        /// </summary>
        private static string[] userLastNames = { "Johnson", "Dean", "Kelly", "Peacock", "Kash", "Haig", "Dickinson", "Finch", "Gill", "Mercer" };

        /// <summary>
        /// Задать имя пользователя рандомно
        /// </summary>
        private void SetNameRandomly()
        {
            int nameindex = _random.Next(userNames.Length - 1);
            int lastnameindex = _random.Next(userNames.Length - 1);

            string name = userNames[nameindex] + ' ' + userLastNames[lastnameindex];

            _element.Name = name;
        }

        /// <summary>
        /// Задать телефон пользователя рандомно
        /// </summary>
        private void SetPhoneNumberRandomly()
        {
            string phoneNumber = "8";

            long number = _random.NextInt64(MinNumber, MaxNumber);

            phoneNumber = phoneNumber + number.ToString();

            _element.PhoneNumber = phoneNumber;
        }

        /// <summary>
        /// Создать пользователя
        /// </summary>
        public void BuildUserRandomly()
        {
            SetNameRandomly();
            SetPhoneNumberRandomly();
        }

        /// <summary>
        /// Экземпляр рандома
        /// </summary>
        private Random _random;
    }
}
