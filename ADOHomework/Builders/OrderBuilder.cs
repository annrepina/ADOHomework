using ADOHomework.Model.basics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADOHomework.Builders
{
    /// <summary>
    /// Строитель заказов - Создает заказы
    /// </summary>
    public class OrderBuilder : Builder<Order>
    {
        /// <summary>
        /// Конструктор по умолчанию
        /// </summary>
        public OrderBuilder()
        {
            _random = new Random();
            _maxBoundForIdRandom = 1;
            _usersId = new List<int>();
        }

        /// <summary>
        /// Константа - минимальная сумма заказа для создания заказа
        /// </summary>
        public const int MinSumm = 5000;

        /// <summary>
        /// Константа - максимальная сумма заказа для создания заказа
        /// </summary>
        public const int MaxSumm = 99999;

        /// <summary>
        /// Максимальная граница для рандомного Id
        /// </summary>
        private int _maxBoundForIdRandom;
        
        /// <summary>
        /// Экземпляр рандома
        /// </summary>
        private Random _random;

        /// <summary>
        /// Список доступных айди юзеров
        /// </summary>
        private List<int> _usersId;

        /// <summary>
        /// Свойство - Список доступных айди юзеров
        /// </summary>
        public List<int> UsersId
        {
            set
            {
                _usersId = value;

                _maxBoundForIdRandom = _usersId.Count - 1;
            }
        }

        /// <summary>
        /// Создать заказ
        /// </summary>
        public void BuildOrder()
        {
            SetUserIdRandomly();
            SetSummRandomly();
            SetDateTimeRandomly();
        }

        /// <summary>
        /// Задать Id пользователя рандомно
        /// Не актуально, т.к. айди будет автоматически создаваться базой данных при добавлении элемента в таблицу
        /// А потом из бд читаться и добавляться в список
        /// </summary>
        private void SetUserIdRandomly()
        {
            if (_maxBoundForIdRandom < 0)
                _maxBoundForIdRandom = _usersId.Count - 1;

            int indexOfUserId = _random.Next(0, _maxBoundForIdRandom);

            _element.UserId = _usersId[indexOfUserId];

            ServiceFunctions.Swap(_usersId, indexOfUserId, _maxBoundForIdRandom);

            --_maxBoundForIdRandom;
        }

        /// <summary>
        /// Задать сумму заказа рандомно
        /// </summary>
        private void SetSummRandomly()
        {
            int summ = _random.Next(MinSumm, MaxSumm);

            _element.Summ = summ;
        }

        /// <summary>
        /// Задать дату рандомно
        /// </summary>
        private void SetDateTimeRandomly()
        {
            DateTime start = new DateTime(2010, 1, 1);

            int range = (DateTime.Now - start).Days;

            DateTime res = start.AddDays(_random.Next(range));

            _element.DateTime = res;
        }
    }
}
