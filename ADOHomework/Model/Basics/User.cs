using System.ComponentModel;

namespace ADOHomework.Model.basics
{
	public class User : INotifyPropertyChanged
	{
		public User()
		{
			Name = "";
			Id = 0;
			PhoneNumber = "";
		}

		public User(string name, int id, string phoneNumber)
		{
			Name = name;
			Id = id;
			PhoneNumber = phoneNumber;
		}

		private string _name;
		private int _id;
		private string _phoneNumber;

		public int Id
		{
			get => _id;

			set
			{
				_id = value;

				OnPropertyChanged(nameof(Id));
			}
		}

		public string Name
		{
			get => _name;

			set
			{
				_name = value;

				OnPropertyChanged(nameof(Name));
			}
		}

		public string PhoneNumber
		{
			get => _phoneNumber;

			set
			{
				_phoneNumber = value;

				OnPropertyChanged(nameof(PhoneNumber));
			}
		}

		public event PropertyChangedEventHandler? PropertyChanged;

		protected void OnPropertyChanged(string propName)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
		}
	}
}