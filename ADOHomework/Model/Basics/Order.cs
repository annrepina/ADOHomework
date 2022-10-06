using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADOHomework.Model.basics
{
	public class Order : INotifyPropertyChanged
	{
		public Order()
		{
			Id = 0;
			Summ = 0;
			UserId = 0;
			DateTime = DateTime.MinValue;
		}

		public Order(int id, int userId, int summ, DateTime dateTime)
		{
			Id = id;
			UserId = userId;
			Summ = summ;
			DateTime = dateTime;
		}

		public int Id
		{
			get => _id;

			set
			{
				_id = value;

				OnPropertyChanged(nameof(Id));
			}
		}

		public int UserId
		{
			get => _userId;

			set
			{
				_userId = value;

				OnPropertyChanged(nameof(UserId));
			}
		}

		public int Summ
		{
			get => _summ;

			set
			{
				_summ = value;

				OnPropertyChanged(nameof(Summ));
			}
		}

		public DateTime DateTime
		{
			get => _dateTime;

			set
			{
				_dateTime = value;

				OnPropertyChanged(nameof(DateTime));
			}
		}

		//private List<int> _usersId;
		private int _id;
		private int _userId;
		private int _summ;
		private DateTime _dateTime;

		public event PropertyChangedEventHandler? PropertyChanged;

		protected void OnPropertyChanged(string propName)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
		}

		//public void AddId(int id)
		//      {
		//          _usersId.Add(id);
		//      }
	}
}
