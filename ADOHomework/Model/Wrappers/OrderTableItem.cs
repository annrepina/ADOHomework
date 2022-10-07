using ADOHomework.Model.basics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADOHomework.Model.Wrappers
{
	/// <summary>
	/// Заказ, в качестве элемента таблицы 
	/// </summary>
	public class OrderTableItem : Order
	{

        #region Конструкторы

		/// <summary>
		/// Конструктор по умолчанию
		/// </summary>
		public OrderTableItem() : base()
		{
			Number = 0;
			UserNumber = 0;
        }

		/// <summary>
		/// Конструктор с параметрами
		/// </summary>
		/// <param name="number">Номер заказа в таблице</param>
		/// <param name="userNumber">Номер пользователя - покупателя в таблице</param>
		/// <param name="id">Идентификатор заказа</param>
		/// <param name="userId">Идентификатор пользователя - покупателя</param>
		/// <param name="summ">Сумма заказа</param>
		/// <param name="dateTime">Дата заказа</param>
		public OrderTableItem(int number, int userNumber, int id, int userId, int summ, DateTime dateTime) : base(id, userId, summ, dateTime)
		{
			Number = number;
			UserNumber = userNumber;
		}

        #endregion Конструкторы


        #region Поля

		/// <summary>
		/// Номер заказа в таблице
		/// </summary>
        private int _number;

        /// <summary>
        /// Номер пользователя - покупателя в таблице
        /// </summary>
        private int _userNumber;

		/// <summary>
		/// Максимальный номер пользователя в таблице
		/// </summary>
        private static int _maxCustomerNumber;

        #endregion Поля


        #region Свойства

        /// <summary>
        /// Свойство - Максимальный номер пользователя в таблице
        /// </summary>
        public static int MaxCustomerNumber
		{
			get => _maxCustomerNumber;

			set
			{
				_maxCustomerNumber = value;
            }
        }

        /// <summary>
        /// Свойство - Номер заказа в таблице
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

        /// <summary>
        /// Свойство - Максимальный номер пользователя в таблице
        /// </summary>
        public int UserNumber
		{
			get => _userNumber;

			set
			{
				_userNumber = value;

				OnPropertyChanged(nameof(UserNumber));
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
            if (obj == null)
                return false;

            if (obj is OrderTableItem item)
                return UserNumber == item.UserNumber && Id == item.Id && UserId == item.UserId && Summ == item.Summ && DateTime == item.DateTime;

            return false;
        }

		/// <summary>
		/// Заказ пустой?
		/// </summary>
		/// <returns></returns>
		public bool IsEmpty()
		{
			return UserNumber == 0 || Summ == 0 || DateTime == DateTime.MinValue;
		}
	}
}
