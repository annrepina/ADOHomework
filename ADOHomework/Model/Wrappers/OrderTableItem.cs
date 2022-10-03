using ADOHomework.Model.basics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADOHomework.Model.Wrappers
{
	public class OrderTableItem : Order
	{
		public OrderTableItem() : base()
		{
			Number = 0;
			UserNumber = 0;
		}

		public OrderTableItem(int number, int userNumber, int id, int userId, int summ, DateTime dateTime) : base(id, userId, summ, dateTime)
		{
			Number = number;
			UserNumber = userNumber;
		}

		private int _number;
		private int _userNumber;

		public int Number 
		{ 
			get => _number; 
			
			set
			{
				_number = value;

				OnPropertyChanged(nameof(Number));
			}
		}

		public int UserNumber
		{
			get => _userNumber;

			set
			{
				_userNumber = value;

				OnPropertyChanged(nameof(UserNumber));
			}
		}
	}
}
