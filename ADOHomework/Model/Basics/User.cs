using System.ComponentModel;

namespace ADOHomework.Model.basics
{
	/// <summary>
	/// Пользователь
	/// </summary>
	public class User : INotifyPropertyChanged
	{

        #region Конструкторы 

		/// <summary>
		/// Конструктор по умолчанию
		/// </summary>
        public User()
		{
			Name = "";
			Id = 0;
			PhoneNumber = "";
		}

		/// <summary>
		/// Конструктор с полями
		/// </summary>
		/// <param name="name">Имя пользователя</param>
		/// <param name="id">Идентификатор пользователя</param>
		/// <param name="phoneNumber">Номер телефона</param>
		public User(string name, int id, string phoneNumber)
		{
			Name = name;
			Id = id;
			PhoneNumber = phoneNumber;
		}

        #endregion Конструкторы


        #region Поля

		/// <summary>
		/// Имя пользователя
		/// </summary>
        private string _name;

		/// <summary>
		/// Идентификатор пользователя
		/// </summary>
		private int _id;

		/// <summary>
		/// Номер телефона пользоватля
		/// </summary>
		private string _phoneNumber;

        #endregion Поля


        #region Свойства

        /// <summary>
        /// Свойство - Идентификатор пользователя
        /// </summary>
        public int Id
		{
			get => _id;

			set
			{
				_id = value;

				OnPropertyChanged(nameof(Id));
			}
		}

        /// <summary>
        /// Свойство - Имя пользователя
        /// </summary>
        public string Name
		{
			get => _name;

			set
			{
				_name = value;

				OnPropertyChanged(nameof(Name));
			}
		}

        /// <summary>
        /// Свойство - Номер телефона пользоватля
        /// </summary>
        public string PhoneNumber
		{
			get => _phoneNumber;

			set
			{
				_phoneNumber = value;

				OnPropertyChanged(nameof(PhoneNumber));
			}
		}

        #endregion Свойства


        #region Реализация INotifyPropertyChanged

        /// <summary>
        /// Событие изменения свойств
        /// </summary>
        public event PropertyChangedEventHandler? PropertyChanged;

        /// <summary>
        /// Метод, вызывающий событие PropertyChanged
        /// </summary>
        /// <param name="propName">Имя измененного свойства</param>
        protected void OnPropertyChanged(string propName)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
		}

        #endregion Реализация INotifyPropertyChanged

	}
}