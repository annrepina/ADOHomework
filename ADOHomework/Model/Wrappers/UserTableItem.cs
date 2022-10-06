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

		public override bool Equals(object? obj)
		{
			if(obj == null)
				return false;

			if(obj is UserTableItem item)
				return Name == item.Name && Id == item.Id && PhoneNumber == item.PhoneNumber;

			return false;
		}

		public bool isEmpty()
		{
			if(Id == 0 && Name == "" && PhoneNumber == "" && Number == 0)
				return true;

			return false;
		}
	}
}
