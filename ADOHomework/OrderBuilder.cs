using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADOHomework
{
    /// <summary>
    /// Создает заказы
    /// </summary>
    public class OrderBuilder : Builder<Order>
    {
        public OrderBuilder()
        {
            _random = new Random();
        }

        public const int minSumm = 5000;
        public const int maxSumm = 99999;

        public const int minYear = 2010;
        public const int maxYear = 2013;

        public const int minMonth = 1;
        public const int maxMonth = 12;
        public const int minDay = 2013;
        public const int maxDay = 2013;


        private Random _random;

        public void BuildOrder(List<int> usersId)
        {
            SetUserIdRandomly(usersId);
            SetSummRandomly();
            SetDateTimeRandomly();
        }

        private void SetUserIdRandomly(List<int> usersId)
        {
            int maxBound = usersId.Count();

            int userId = _random.Next(1, maxBound);

            _element.Id = userId;
        }

        private void SetSummRandomly()
        {
            int summ = _random.Next(minSumm, maxSumm);

            _element.Summ = summ;
        }

        private void SetDateTimeRandomly()
        {
            DateTime start = new DateTime(2010, 1, 1);

            int range = (DateTime.Now - start).Days;

            DateTime res = start.AddDays(range); 

            _element.DateTime = res;
        }
    }
}
