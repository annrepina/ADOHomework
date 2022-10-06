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
		public static int MaxCustomerNumber
		{
			get => _maxCustomerNumber;

			set
			{
				_maxCustomerNumber = value;

				//thisOnPropertyChanged(nameof(MaxCustomerNumber));

            }

        }

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
		private static int _maxCustomerNumber;

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

		public override bool Equals(object? obj)
		{
            if (obj == null)
                return false;

            if (obj is OrderTableItem item)
                return UserNumber == item.UserNumber && Id == item.Id && UserId == item.UserId && Summ == item.Summ && DateTime == item.DateTime;

            return false;
        }

		public bool IsEmpty()
		{
			return UserNumber == 0 || Summ == 0 || DateTime == DateTime.MinValue;
		}
	}
}
