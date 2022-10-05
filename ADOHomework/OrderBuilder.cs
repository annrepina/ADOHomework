using ADOHomework.Model.basics;
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
            _maxBoundForIdRandom = 1;
            _usersId = new List<int>();
        }

        public const int MinSumm = 5000;
        public const int MaxSumm = 99999;

        //public const int MinYear = 2010;
        //public const int MaxYear = 2013;

        //public const int MinMonth = 1;
        //public const int MaxMonth = 12;
        //public const int MinDay = 2013;
        //public const int MaxDay = 2013;

        private int _maxBoundForIdRandom;

        private Random _random;
        private List<int> _usersId;

        public List<int> UsersId 
        { 
            set
            {
                _usersId = value;

                _maxBoundForIdRandom = _usersId.Count - 1;
            }
        }

        public void BuildOrder()
        {
            SetUserIdRandomly();
            SetSummRandomly();
            SetDateTimeRandomly();
        }

        private void SetUserIdRandomly()
        {
            if(_maxBoundForIdRandom < 0)
                _maxBoundForIdRandom = _usersId.Count - 1;

            int indexOfUserId = _random.Next(0, _maxBoundForIdRandom);

            _element.UserId = _usersId[indexOfUserId];

            ServiceFunctions.Swap(_usersId, indexOfUserId, _maxBoundForIdRandom);

            --_maxBoundForIdRandom;
        }

        private void SetSummRandomly()
        {
            int summ = _random.Next(MinSumm, MaxSumm);

            _element.Summ = summ;
        }

        private void SetDateTimeRandomly()
        {
            DateTime start = new DateTime(2010, 1, 1);

            int range = (DateTime.Now - start).Days;

            DateTime res = start.AddDays(_random.Next(range)); 

            _element.DateTime = res;

            //DateTime date 
        }

        
    }
}
