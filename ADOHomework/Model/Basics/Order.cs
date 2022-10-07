using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADOHomework.Model.basics
{
	/// <summary>
	/// Заказ
	/// </summary>
	public class Order : INotifyPropertyChanged
	{

        #region Конструкторы

        /// <summary>
        /// Конструктор по умолчанию
        /// </summary>
        public Order()
		{
			Id = 0;
			Summ = 0;
			UserId = 0;
			DateTime = DateTime.MinValue;
		}

		/// <summary>
		/// Конструктор с параметрами
		/// </summary>
		/// <param name="id">Идентификатор</param>
		/// <param name="userId">Идентификатор пользователя (покупателя)</param>
		/// <param name="summ">Сумма заказа</param>
		/// <param name="dateTime">Дата заказа</param>
		public Order(int id, int userId, int summ, DateTime dateTime)
		{
			Id = id;
			UserId = userId;
			Summ = summ;
			DateTime = dateTime;
		}

        #endregion Конструкторы


        #region Поля

        /// <summary>
        /// Идентификатор
        /// </summary>
        private int _id;

		/// <summary>
		/// Идентификатор пользователя - покупателя
		/// </summary>
		private int _userId;

		/// <summary>
		/// Сумма заказа
		/// </summary>
		private int _summ;

		/// <summary>
		/// Дата заказа
		/// </summary>
		private DateTime _dateTime;

        #endregion Поля


        #region Свойства

        /// <summary>
        /// Свойство - идентификатор
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
		/// Свойство - идентификатор пользователя - покупателя
		/// </summary>
		public int UserId
		{
			get => _userId;

			set
			{
				_userId = value;

				OnPropertyChanged(nameof(UserId));
			}
		}

		/// <summary>
		/// Свойство - Сумма заказа
		/// </summary>
		public int Summ
		{
			get => _summ;

			set
			{
				_summ = value;

				OnPropertyChanged(nameof(Summ));
			}
		}

		/// <summary>
		/// Свойство - Дата заказа
		/// </summary>
		public DateTime DateTime
		{
			get => _dateTime;

			set
			{
				_dateTime = value;

				OnPropertyChanged(nameof(DateTime));
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
