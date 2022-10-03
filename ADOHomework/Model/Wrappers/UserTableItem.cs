using ADOHomework.Model.basics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADOHomework.Model.Wrappers
{
	public class UserTableItem : User
	{
		private int _number;

		public UserTableItem() : base()
		{
			Number = 0;
		}

		public UserTableItem(int number, string name, int id, string phoneNumber) : base(name, id, phoneNumber)
		{
			Number = number;
		}

		public int Number 
		{ 
			get => _number; 
			
			set
			{
				_number = value;

				OnPropertyChanged(nameof(Number));
			}
		}
	}
}
