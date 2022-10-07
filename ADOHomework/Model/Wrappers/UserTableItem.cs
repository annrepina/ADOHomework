using ADOHomework.Model.basics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADOHomework.Model.Wrappers
{
    /// <summary>
    /// Пользователь, в качестве элемента таблицы 
    /// </summary>
    public class UserTableItem : User
	{

        #region Конструкторы

        /// <summary>
        /// Конструктор по умолчанию
        /// </summary>
        public UserTableItem() : base()
		{
			Number = 0;
		}

		/// <summary>
		/// Конструктор с параметрами
		/// </summary>
		/// <param name="number">Номер пользователя в таблице</param>
		/// <param name="name">Имя пользователя</param>
		/// <param name="id">Идентификатор пользователя</param>
		/// <param name="phoneNumber">Номер телефона пользователя</param>
		public UserTableItem(int number, string name, int id, string phoneNumber) : base(name, id, phoneNumber)
		{
			Number = number;
		}

        #endregion Конструкторы


        #region Поля

        /// <summary>
        /// Номер пользователя в таблице
        /// </summary>
        private int _number;

        #endregion Поля

        #region Свойства

        /// <summary>
        /// Номер пользователя в таблице
        /// </summary>
        public int Number 
		{ 
			get => _number; 
			
			set
			{
				_number = value;

				OnPropertyChanged(nameof(Number));
			}
		}

        #endregion Свойства

        /// <summary>
        /// Перегрузка Equals
        /// </summary>
        /// <param name="obj">Объект, с которым сравнивают текущий экземпляр класса</param>
        /// <returns></returns>
        public override bool Equals(object? obj)
		{
			if(obj == null)
				return false;

			if(obj is UserTableItem item)
				return Name == item.Name && Id == item.Id && PhoneNumber == item.PhoneNumber;

			return false;
		}

		/// <summary>
		/// Пользователь пустой?
		/// </summary>
		/// <returns></returns>
		public bool IsEmpty()
		{
			if(Name == "" || PhoneNumber == "")
				return true;

			return false;
		}
	}
}
