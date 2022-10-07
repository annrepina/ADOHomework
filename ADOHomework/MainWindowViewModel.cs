using ADOHomework.Builders;
using ADOHomework.Model.basics;
using ADOHomework.Model.Wrappers;
using Microsoft.Data.SqlClient;
using Microsoft.IdentityModel.Tokens;
using Prism.Commands;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using System.Windows.Shell;

namespace ADOHomework
{
    /// <summary>
    /// ViewModel
    /// </summary>
    public class MainWindowViewModel : INotifyPropertyChanged
    {
        /// <summary>
        /// Конструктор по умолчанию
        /// </summary>
        public MainWindowViewModel()
        {
            OnConnectToServerCommand = new DelegateCommand(OnConnectToServerAsync);
            OnFillDatabaseCommand = new DelegateCommand(OnFillDatabaseAsync);
            OnAddNewUserCommand = new DelegateCommand(OnAddNewUserAsync);
            OnAddNewOrderCommand = new DelegateCommand(OnAddNewOrderAsync);
            OnUserDataGridDeleteKeyDownCommand = new DelegateCommand(OnUserDataGridDeleteKeyDownAsync);
            OnOrderDataGridDeleteKeyDownCommand = new DelegateCommand(OnOrderDataGridDeleteKeyDownAsync);

            _serverName = "";
            _connectionString = "";

            _isNotConnected = true;
            _hasCorrectServerName = false;
            _databaseIsNotFilled = true;
            _databaseIsFilled = false;
            _canConnect = false;
            _canAddNewUser = false;

            UserTableItems = new ObservableCollection<UserTableItem>();
            OrderTableItems = new ObservableCollection<OrderTableItem>();
            UserTableItems.CollectionChanged += OnUserTableItemsListPropertyChanged;
            OrderTableItems.CollectionChanged += OnOrderTableItemsListPropertyChanged;

            NewUserTableItem = new UserTableItem();
            NewOrderTableItem = new OrderTableItem();
            SelectedUserTableItem = new UserTableItem();
            SelectedOrderTableItem = new OrderTableItem();

            _lastUserNumber = 0;
            _lastOrderNumber = 0;
            _minOrderSum = 0;
            _maxOrderSum = 0;
            _totalOrderSum = 0;
        }

        #region Поля

        /// <summary>
        /// Имя сервера
        /// </summary>
        private string _serverName;

        /// <summary>
        /// Строка подключения
        /// </summary>
        private string _connectionString;

        /// <summary>
        /// Не подключено к серверу?
        /// </summary>
		private bool _isNotConnected;

        /// <summary>
        /// Есть корректная строка подключения к серверу?
        /// </summary>
		private bool _hasCorrectServerName;

        /// <summary>
        /// Подключено к серверу?
        /// </summary>
        private bool _isConnected;

        /// <summary>
        /// Последний номер пользователя в таблице
        /// </summary>
        private int _lastUserNumber;

        /// <summary>
        /// Последний номер заказа в таблице
        /// </summary>
        private int _lastOrderNumber;

        /// <summary>
        /// База данных не заполнена?
        /// </summary>
        private bool _databaseIsNotFilled;

        /// <summary>
        /// База данных заполнена?
        /// </summary>
        private bool _databaseIsFilled;

        /// <summary>
        /// Можно подключиться?
        /// </summary>
        private bool _canConnect;

        /// <summary>
        /// Можно добавить пользователя?
        /// </summary>
        private bool _canAddNewUser;

        /// <summary>
        /// Минимальная сумма заказа
        /// </summary>
        private int _minOrderSum;

        /// <summary>
        /// Максимальная сумма заказа
        /// </summary>
        private int _maxOrderSum;

        /// <summary>
        /// Общая сумма заказа
        /// </summary>
        private ulong _totalOrderSum;

        /// <summary>
        /// Новый пользователь в таблице
        /// </summary>
		private UserTableItem _newUserTableItem;

        /// <summary>
        /// Новый заказ в таблице
        /// </summary>
		private OrderTableItem _newOrderTableItem;

        /// <summary>
        /// Выбранный пользователь в таблице
        /// </summary>
		private UserTableItem _selectedUserTableItem;

        /// <summary>
        /// выбранный заказ в таблице
        /// </summary>
		private OrderTableItem _selectedOrderTableItem;

        #endregion Поля


		#region Константы

        /// <summary>
        /// Число пользователей по умолчанию
        /// </summary>
		private const int DefaultNumberOfUsers = 10;

        /// <summary>
        /// Число заказов по умолчанию
        /// </summary>
        private const int DefaultNumberOfOrders = 15;

        /// <summary>
        /// название БД по умолчанию
        /// </summary>
        private const string DefaultDatabaseName = "ADOHomework";

        /// <summary>
        /// Название таблицы пользователей по умолчанию
        /// </summary>
        private const string UsersTableName = "Users";

        /// <summary>
        /// Название таблицы заказов по умолчанию
        /// </summary>
        private const string OrdersTableName = "Orders";

        #endregion Константы


        #region Свойства

        /// <summary>
        /// Событие изменения свойств
        /// </summary>
        public event PropertyChangedEventHandler? PropertyChanged;

        /// <summary>
        /// Обозреваемая коллекция элементы таблицы юзеров
        /// </summary>
        public ObservableCollection<UserTableItem> UserTableItems{ get; set; }

        /// <summary>
        /// Обозреваемая коллекция элементы таблицы заказов
        /// </summary>
        public ObservableCollection<OrderTableItem> OrderTableItems{ get; set; }

        /// <summary>
        /// Новый элемент таблицы пользователей
        /// </summary>
        public UserTableItem NewUserTableItem 
        {
            get => _newUserTableItem;

			set
			{
                _newUserTableItem = value;

                OnPropertyChanged(nameof(NewUserTableItem));
			}
        }

        /// <summary>
        /// Новый элемент таблицы заказов
        /// </summary>
        public OrderTableItem NewOrderTableItem 
        {
            get => _newOrderTableItem;

            set
			{
                _newOrderTableItem = value;

                OnPropertyChanged(nameof(NewOrderTableItem));
            }
        }

        /// <summary>
        /// Выбранный элемент таблицы пользователей
        /// </summary>
        public UserTableItem SelectedUserTableItem
		{
            get => _selectedUserTableItem;

			set
			{
                _selectedUserTableItem = value;

                OnPropertyChanged(nameof(SelectedUserTableItem));
			}
        }

        /// <summary>
        /// Выбраный элемент таблицы заказов
        /// </summary>
        public OrderTableItem SelectedOrderTableItem
        {
            get => _selectedOrderTableItem;

            set
            {
                _selectedOrderTableItem = value;

                OnPropertyChanged(nameof(SelectedOrderTableItem));
            }
        }

        /// <summary>
        /// Минимальная сумма заказов
        /// </summary>
        public int MinOrderSum
        {
            get => _minOrderSum;

            set
            {
                _minOrderSum = value;

                OnPropertyChanged(nameof(MinOrderSum));
            }
        }

        /// <summary>
        /// Максимальная сумма заказов
        /// </summary>
        public int MaxOrderSum
        {
            get => _maxOrderSum;

            set
            {
                _maxOrderSum = value;

                OnPropertyChanged(nameof(MaxOrderSum));
            }
        }

        /// <summary>
        /// Общая сумма заказов
        /// </summary>
        public ulong TotalOrderSum
        {
            get => _totalOrderSum;

            set
            {
                _totalOrderSum = value;

                OnPropertyChanged(nameof(TotalOrderSum));
            }
        }

        /// <summary>
        /// Есть подкючение?
        /// </summary>
        public bool IsConnected 
        { 
            get => _isConnected; 

            set
			{
                _isConnected = value;

                OnPropertyChanged(nameof(IsConnected));
			}
        }

        /// <summary>
        /// Нет подкючения?
        /// </summary>
        public bool IsNotConnected
        {
            get => _isNotConnected;

            set
            {
                _isNotConnected = value;

                CanConnect = _isNotConnected && _hasCorrectServerName;

                OnPropertyChanged(nameof(IsNotConnected));
            }
        }

        /// <summary>
        /// Есть корректная срока подключения
        /// </summary>
        public bool HasCorrectServerName
        { 
            get => _hasCorrectServerName;

			set
			{
                _hasCorrectServerName = value;

                CanConnect = _isNotConnected && _hasCorrectServerName;

                OnPropertyChanged(nameof(HasCorrectServerName));
            }      
        }

        /// <summary>
        /// Можем подключиться?
        /// </summary>
        public bool CanConnect
        {
            get => _canConnect;

            set
            {
                _canConnect = value;

                OnPropertyChanged(nameof(CanConnect)); 
            }
        }

        /// <summary>
        /// Можем добавить нового пользователя
        /// </summary>
        public bool CanAddNewUser
		{
            get => _canAddNewUser;

			set
			{
                _canAddNewUser = value;

                OnPropertyChanged(nameof(CanAddNewUser));
			}
        }

        /// <summary>
        /// Имя сервера
        /// </summary>
        public string ServerName
		{
            get => _serverName;

            set
			{
                _serverName = value;

                _connectionString = $"Server={_serverName};Database=master;Trusted_Connection=True;Encrypt=False;MultipleActiveResultSets=True;";

                HasCorrectServerName = true;
            }
		}

        /// <summary>
        /// База данных не заполнена?
        /// </summary>
        public bool DatabaseIsNotFilled
        {
            get => _databaseIsNotFilled;
            
            set
            {
                _databaseIsNotFilled = value;

                OnPropertyChanged(nameof(DatabaseIsNotFilled));
            }
        }

        /// <summary>
        /// База данных заполнена?
        /// </summary>
        public bool DatabaseIsFilled
        {
            get => _databaseIsFilled;

            set
            {
                _databaseIsFilled = value;

                OnPropertyChanged(nameof(DatabaseIsFilled));
            }
        }

        #endregion Свойства


        #region DelegateCommands

        /// <summary>
        /// Комманда по нажатию на кнопку-заполнить БД
        /// </summary>
        public DelegateCommand OnFillDatabaseCommand { get; }

        /// <summary>
        /// Комманда по нажатию на кнопку подключиться к серверу
        /// </summary>
        public DelegateCommand OnConnectToServerCommand { get; }

        /// <summary>
        /// Комманда по нажатию на кнопку-добавить нового пользователя
        /// </summary>
        public DelegateCommand OnAddNewUserCommand { get; }

        /// <summary>
        /// Комманда по нажатию на кнопку-добавить новый заказ
        /// </summary>
        public DelegateCommand OnAddNewOrderCommand { get; }

        /// <summary>
        /// Команда по нажатию кнопки delete при выделенном пользователе в таблице
        /// </summary>
        public DelegateCommand OnUserDataGridDeleteKeyDownCommand { get; }

        /// <summary>
        /// Команда по нажатию кнопки delete при выделенном заказа в таблице
        /// </summary>
        public DelegateCommand OnOrderDataGridDeleteKeyDownCommand { get; }

        #endregion DelegateCommands


        #region Обработчики DelegateCommands

        /// <summary>
        /// Обработчик команды OnConnectToServerCommand
        /// </summary>
        private async void OnConnectToServerAsync()
        {
            try
            {
                // SqlConnection с помощью которого мы подключаеся к sql серверу 
                using (SqlConnection connection = new SqlConnection(_connectionString))
			    {
				    // Открываем подключение
				    await connection.OpenAsync();

                    // Выводим айди нашего сеанса
                    MessageBox.Show($"Connected: {connection.ClientConnectionId}");

                    IsNotConnected = false;
                    IsConnected = true;

                    string messageText = "";

                    if (HasDatabase(connection))
                        return;

                    else
					{
                        CreateDatabaseAndTables(connection);
                        CreateStoredProcedures(connection);
                        messageText = $"Database called {connection.Database} has created";
                    }

                    MessageBox.Show(messageText);
                }
            }
            catch (Exception ex)
            {
                // сбрасываем значение т.к. нет подключения
                IsNotConnected = true;

                MessageBox.Show("Server name is not correct. Please, try another one");
            }
        }

        private async void OnFillDatabaseAsync()
        {
            try
            {
                // SqlConnection с помощью которого мы подключаеся к sql серверу 
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    // Открываем подключение
                    await connection.OpenAsync();

                    string messageText = "";

                    messageText = $"Database {DefaultDatabaseName} has filled";

                    FillAndReadTables(connection);

                    UpdateParametersLabel();

                    MessageBox.Show(messageText);

                    DatabaseIsNotFilled = false;
                    DatabaseIsFilled = true;
                }
            }
            catch (Exception ex)
            {
                // сбрасываем значение т.к. нет подключения
                IsNotConnected = true;

                MessageBox.Show(ex.Message);
            }
        }

        private async void OnAddNewUserAsync()
        {
            if (NewUserTableItem != null)
            {
                if(NewUserTableItem.IsEmpty())
				{
                    MessageBox.Show("Fill all fields, please");

                    return;
				}

                try
                {
                    // SqlConnection с помощью которого мы подключаеся к sql серверу 
                    using (SqlConnection connection = new SqlConnection(_connectionString))
                    {
                        // Открываем подключение
                        await connection.OpenAsync();

                        //string messageText = "";

                        SqlCommand useCommand = new SqlCommand($"USE {DefaultDatabaseName}\n", connection);
                        useCommand.ExecuteNonQuery();

                        // создаем команду
                        SqlCommand command = new SqlCommand($"InsertInto{UsersTableName}", connection);
                        command.CommandType = System.Data.CommandType.StoredProcedure;

                        SqlParameter nameParam = new SqlParameter("@name", NewUserTableItem.Name);
                        command.Parameters.Add(nameParam);

                        SqlParameter phoneNumberParam = new SqlParameter("@phoneNumber", NewUserTableItem.PhoneNumber);
                        command.Parameters.Add(phoneNumberParam);

                        command.ExecuteNonQuery();

                        AddNewUserToList(connection);

                        string messageText = $"New user was successfully added";
                        MessageBox.Show(messageText);

                        OrderTableItem.MaxCustomerNumber = UserTableItems.Count;
                    }
                }
                catch (Exception ex)
                {
                    // сбрасываем значение т.к. нет подключения
                    IsNotConnected = true;

                    MessageBox.Show(ex.Message);
                }

            }
            else 
                return;
        }

        private async void OnAddNewOrderAsync()
		{
            if (NewOrderTableItem != null)
            {
                if (NewOrderTableItem.IsEmpty())
                {
                    MessageBox.Show("Fill all fields, please");

                    return;
                }

                try
                {
                    // SqlConnection с помощью которого мы подключаеся к sql серверу 
                    using (SqlConnection connection = new SqlConnection(_connectionString))
                    {
                        // Открываем подключение
                        await connection.OpenAsync();

                        //string messageText = "";

                        NewOrderTableItem.UserId = GetIdOfUserByNumber(NewOrderTableItem.UserNumber);

                        SqlCommand useCommand = new SqlCommand($"USE {DefaultDatabaseName}\n", connection);
                        useCommand.ExecuteNonQuery();

                        // создаем команду
                        SqlCommand command = new SqlCommand($"InsertInto{OrdersTableName}", connection);
                        command.CommandType = System.Data.CommandType.StoredProcedure;

                        SqlParameter customerIdparam = new SqlParameter("@customerId", NewOrderTableItem.UserId);
                        command.Parameters.Add(customerIdparam);

                        SqlParameter sumParam = new SqlParameter("@sum", NewOrderTableItem.Summ);
                        command.Parameters.Add(sumParam);

                        SqlParameter dateParam = new SqlParameter("@date", NewOrderTableItem.DateTime);
                        command.Parameters.Add(dateParam);

                        command.ExecuteNonQuery();

                        AddNewOrderToList(connection);

                        UpdateParametersLabel();

                        DatabaseIsFilled = true;

                        string messageText = $"New order was successfully added";
                        MessageBox.Show(messageText);
                    }
                }
                catch (Exception ex)
                {
                    // сбрасываем значение т.к. нет подключения
                    IsNotConnected = true;

                    MessageBox.Show(ex.Message);
                }

            }
            else
                return;
        }

        private async void OnUserDataGridDeleteKeyDownAsync()
		{
            if (SelectedUserTableItem != null)
            {
                //if (NewOrderTableItem.IsEmpty())
                //{
                //    MessageBox.Show("Fill all fields, please");

                //    return;
                //}

                try
                {
					foreach (var userTableItem in UserTableItems)
					{
                        if(userTableItem.Equals(SelectedUserTableItem))
						{
                            // SqlConnection с помощью которого мы подключаеся к sql серверу 
                            using (SqlConnection connection = new SqlConnection(_connectionString))
                            {
                                // Открываем подключение
                                await connection.OpenAsync();

                                //string messageText = "";

                                //NewOrderTableItem.UserId = GetIdOfUserByNumber(NewOrderTableItem.UserNumber);

                                SqlCommand useCommand = new SqlCommand($"USE {DefaultDatabaseName}\n", connection);
                                useCommand.ExecuteNonQuery();

                                // создаем команду
                                SqlCommand command = new SqlCommand($"Delete{UsersTableName}", connection);
                                command.CommandType = System.Data.CommandType.StoredProcedure;

                                SqlParameter idParam = new SqlParameter("@id", SelectedUserTableItem.Id);
                                command.Parameters.Add(idParam);

                                command.ExecuteNonQuery();

                                _lastUserNumber = 0;
                                _lastOrderNumber = 0;

                                ReadUserTableItemsFromDB(connection);
                                ReadOrderTableItemsFromDB(connection);

                                UpdateParametersLabel();

                                if(UserTableItems.Count == 0)
								{
                                    DatabaseIsFilled = false;
                                    DatabaseIsNotFilled = true;
								}

                                string messageText = $"User was successfully deleted";
                                MessageBox.Show(messageText);

                                return;
                            }
						}

                    }


                }
                catch (Exception ex)
                {
                    // сбрасываем значение т.к. нет подключения
                    IsNotConnected = true;

                    MessageBox.Show(ex.Message);
                }

            }
            else
                return;
        }

        private async void OnOrderDataGridDeleteKeyDownAsync()
		{
            if (SelectedOrderTableItem != null)
            {
                //if (NewOrderTableItem.IsEmpty())
                //{
                //    MessageBox.Show("Fill all fields, please");

                //    return;
                //}

                try
                {
                    foreach (var orderTableItem in OrderTableItems)
                    {
                        if (orderTableItem.Equals(SelectedOrderTableItem))
                        {
                            // SqlConnection с помощью которого мы подключаеся к sql серверу 
                            using (SqlConnection connection = new SqlConnection(_connectionString))
                            {
                                // Открываем подключение
                                await connection.OpenAsync();

                                //string messageText = "";

                                //NewOrderTableItem.UserId = GetIdOfUserByNumber(NewOrderTableItem.UserNumber);

                                SqlCommand useCommand = new SqlCommand($"USE {DefaultDatabaseName}\n", connection);
                                useCommand.ExecuteNonQuery();

                                // создаем команду
                                SqlCommand command = new SqlCommand($"Delete{OrdersTableName}", connection);
                                command.CommandType = System.Data.CommandType.StoredProcedure;

                                SqlParameter idParam = new SqlParameter("@id", SelectedOrderTableItem.Id);
                                command.Parameters.Add(idParam);

                                command.ExecuteNonQuery();

                                _lastOrderNumber = 0;

                                ReadOrderTableItemsFromDB(connection);

                                UpdateParametersLabel();

                                if (OrderTableItems.Count == 0)
                                {
                                    DatabaseIsFilled = false;
                                    DatabaseIsNotFilled = true;
                                }

                                string messageText = $"Order was successfully deleted";
                                MessageBox.Show(messageText);

                                return;
                            }
                        }

                    }


                }
                catch (Exception ex)
                {
                    // сбрасываем значение т.к. нет подключения
                    IsNotConnected = true;

                    MessageBox.Show(ex.Message);
                }

            }
            else
                return;
        }


        #endregion Обработчики DelegateCommands


        #region Методы-обработчики событий измениния свойств

        private void OnPropertyChanged(string propName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
        }

        /// <summary>
        /// Когда изменяется список пользователей
        /// </summary>
        /// <param name="sender">Объект вызваваший событие</param>
        /// <param name="e">Содержит данные о событии</param>
        private void OnUserTableItemsListPropertyChanged(object? sender, NotifyCollectionChangedEventArgs e)
		{
            if(e.OldItems != null)
            {
                foreach (INotifyPropertyChanged item in e.OldItems)
                {
                    item.PropertyChanged -= OnUserTableItemsPropertyChanged;
                }
            }

            if(e.NewItems != null)
            {
                foreach (INotifyPropertyChanged item in e.NewItems)
                {
                    item.PropertyChanged += OnUserTableItemsPropertyChanged;
                }
            }          
		}

        private async void OnUserTableItemsPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            ObservableCollection<UserTableItem> tempUserTableItems = new ObservableCollection<UserTableItem>();

            // SqlConnection с помощью которого мы подключаеся к sql серверу 
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                // Открываем подключение
                await connection.OpenAsync();

                ReadUserTableItemsFromDB(connection, ref tempUserTableItems);

                int numberofElements = tempUserTableItems.Count;

                try
                {
                    for(int i = 0; i < numberofElements; i++)
                    {
                        // если они чем-то отличаются
                        if (!tempUserTableItems[i].Equals(UserTableItems[i]))
                        {
                            int id = UserTableItems[i].Id;

                            string sqlExpression = $"USE {DefaultDatabaseName}\n" +
                                                   $"UPDATE {UsersTableName}\n" +
                                                   $"SET [Name] = '{UserTableItems[i].Name}', PhoneNumber = '{UserTableItems[i].PhoneNumber}'\n" +
                                                   $"WHERE Id = {id}";

                            SqlCommand sqlCommand = new SqlCommand(sqlExpression, connection);

                            sqlCommand.ExecuteNonQuery();

                            break;
                        }
                    }
                }
                catch (Exception)
                {
                    throw;
                }
            }
        }

        private void OnOrderTableItemsListPropertyChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.OldItems != null)
            {
                foreach (INotifyPropertyChanged item in e.OldItems)
                {
                    item.PropertyChanged -= OnOrderTableItemsPropertyChanged;
                }
            }

            if (e.NewItems != null)
            {
                foreach (INotifyPropertyChanged item in e.NewItems)
                {
                    item.PropertyChanged += OnOrderTableItemsPropertyChanged;
                }
            }
        }

        private async void OnOrderTableItemsPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            ObservableCollection<OrderTableItem> tempOrderTableItems = new ObservableCollection<OrderTableItem>();

            // SqlConnection с помощью которого мы подключаеся к sql серверу 
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                // Открываем подключение
                await connection.OpenAsync();

                ReadOrderTableItemsFromDB(connection, ref tempOrderTableItems);

                int numberofElements = tempOrderTableItems.Count;

                try
                {
                    for (int i = 0; i < numberofElements; i++)
                    {
                        // если они чем-то отличаются
                        if (!tempOrderTableItems[i].Equals(OrderTableItems[i]))
                        {
                            if (tempOrderTableItems[i].UserNumber != OrderTableItems[i].UserNumber)
                            {
                                OrderTableItems[i].UserId = GetIdOfUserByNumber(OrderTableItems[i].UserNumber);
                            }

                            int id = OrderTableItems[i].Id;

                            string sqlExpression = $"USE {DefaultDatabaseName}\n" +
                                                   $"UPDATE {OrdersTableName}\n" +
                                                   $"SET CustomerId = {OrderTableItems[i].UserId}, Summ = {OrderTableItems[i].Summ}, Date = '{OrderTableItems[i].DateTime}'\n" +
                                                   $"WHERE Id = {id}";

                            SqlCommand sqlCommand = new SqlCommand(sqlExpression, connection);

                            sqlCommand.ExecuteNonQuery();

                            break;
                        }
                    }
                }
                catch (Exception)
                {
                    throw;
                }
            }
        }



        #endregion Методы-обработчики событий измениния свойств


        #region Функции по созданию БД, таблиц, хранимых процедур

        /// <summary>
        /// Создать Базу данных
        /// </summary>
        /// <param name="connection">SQL соединение</param>
        private void CreateDatabase(SqlConnection connection)
        {
            // создаем запрос для созданя бд
            string sqlExpression = $"CREATE DATABASE {DefaultDatabaseName}";

            // Создаем новую комманду sql 
            // Задаем текст запроса
            // Создать БД
            SqlCommand command = new SqlCommand(sqlExpression, connection);

            //SqlParameter nameParam = new SqlParameter("@databaseName", DefaultDatabaseName);
            //command.Parameters.Add(nameParam);

            // Выполняем команду
            command.ExecuteNonQuery();
        }

        /// <summary>
        /// Создать таблицы в БД
        /// </summary>
        /// <param name="connection">SQL соединение</param>
        private void CreateTables(SqlConnection connection)
        {
            // Запрос
            string sqlExpression = $"USE ADOHomework\n" +
                $"CREATE TABLE {UsersTableName}\n" +
                "(\n" +
                    "Id int PRIMARY KEY IDENTITY NOT NULL,\n" +
                    "[Name] varchar(100) NOT NULL,\n" +
                    "PhoneNumber varchar(11) NOT NULL\n" +
                ")\n" +
                $"CREATE TABLE {OrdersTableName}\n" +
                "(\n" +
                    "Id int PRIMARY KEY IDENTITY NOT NULL,\n" +
                    $"CustomerId int,\n" +
                    "Summ int NOT NULL,\n" +
                    "[Date] datetime NOT NULL\n" +
                    "FOREIGN KEY (CustomerId) REFERENCES Users (Id) ON DELETE CASCADE\n" +
                ")";

            // Создаем новую комманду sql 
            // Задаем текст запроса
            // Создать БД
            SqlCommand command = new SqlCommand(sqlExpression, connection);

            // Выполняем команду
            command.ExecuteNonQuery();
        }

        /// <summary>
        /// Создать хранимую процедуру для вставки значений в таблицу Users
        /// </summary>
        /// <param name="connection">SQL соединение</param>
        private void CreateProcedureInsertUsers(SqlConnection connection)
        {
            string createProcedureStr =
                            $"CREATE PROCEDURE[dbo].[InsertInto{UsersTableName}]\n" +
                                "@name varchar(100),\n" +
                                "@phoneNumber varchar(11)\n" +
                            "AS\n" +
                                $"INSERT INTO {UsersTableName}([Name], PhoneNumber)\n" +
                                "VALUES(@name, @phoneNumber)\n";

            SqlCommand command = new SqlCommand(createProcedureStr, connection);

            command.ExecuteNonQuery();
        }

        private void CreateProcedureDeleteUsers(SqlConnection connection)
        {
            string createProcedureStr =
                            $"CREATE PROCEDURE[dbo].[Delete{UsersTableName}]\n" +
                                "@id int\n" +
                            "AS\n" +
                                $"DELETE {UsersTableName}\n" +
                                "WHERE Id = @id\n";

            SqlCommand command = new SqlCommand(createProcedureStr, connection);

            command.ExecuteNonQuery();
        }

        private void CreateProcedureDeleteOrders(SqlConnection connection)
        {
            string createProcedureStr =
                            $"CREATE PROCEDURE[dbo].[Delete{OrdersTableName}]\n" +
                                "@id int\n" +
                            "AS\n" +
                                $"DELETE {OrdersTableName}\n" +
                                "WHERE Id = @id\n";

            SqlCommand command = new SqlCommand(createProcedureStr, connection);

            command.ExecuteNonQuery();
        }

        /// <summary>
        /// Создать хранимую процедуру для вставки значения в таблиицу заказов
        /// </summary>
        /// <param name="connection">SQL соединение</param>
        private void CreateProcedureInsertOrders(SqlConnection connection)
        {
            string createProcedureStr =
                $"CREATE PROCEDURE[dbo].[InsertInto{OrdersTableName}]\n" +
                    "@customerId int,\n" +
                    "@sum int,\n" +
                    "@date datetime\n" +
                "AS\n" +
                    $"INSERT INTO {OrdersTableName} (CustomerId, Summ, [Date])\n" +
                    "VALUES (@customerId, @sum, @date)\n";

            SqlCommand command = new SqlCommand(createProcedureStr, connection);

            command.ExecuteNonQuery();
        }

        private void CreateDatabaseAndTables(SqlConnection connection)
		{
            CreateDatabase(connection);
            CreateTables(connection);
        }

        private void CreateStoredProcedures(SqlConnection connection)
        {
            CreateProcedureInsertUsers(connection);
            CreateProcedureInsertOrders(connection);
            CreateProcedureDeleteUsers(connection);
            CreateProcedureDeleteOrders(connection);
        }

        private void FillAndReadTables(SqlConnection connection)
        {
            FillUsersTable(connection);
            ReadUserTableItemsFromDB(connection);

            FillOrdersTable(connection);
            ReadOrderTableItemsFromDB(connection);
        }

        #endregion Функции по созданию БД, таблиц, хранимых процедур


        #region Заполнение таблиц

        /// <summary>
        /// Заполнить таблицу юзеров
        /// </summary>
        /// <param name="connection">SQL соединение</param>
        private void FillUsersTable(SqlConnection connection)
        {
            UserBuilder userBuilder = new UserBuilder();

            SqlCommand useCommand = new SqlCommand($"USE {DefaultDatabaseName}\n", connection);
            useCommand.ExecuteNonQuery();

            for (int i = 0; i < DefaultNumberOfUsers; ++i)
            {
                // создаем команду
                SqlCommand command = new SqlCommand($"InsertInto{UsersTableName}", connection);
                command.CommandType = System.Data.CommandType.StoredProcedure;

                // Создаем юзера
                userBuilder.BuildUserRandomly();
                User user = userBuilder.GetResult();

                SqlParameter nameParam = new SqlParameter("@name", user.Name);
                command.Parameters.Add(nameParam);

                SqlParameter phoneNumberParam = new SqlParameter("@phoneNumber", user.PhoneNumber);
                command.Parameters.Add(phoneNumberParam);

                command.ExecuteNonQuery();
            }
        }
        private void FillOrdersTable(SqlConnection connection)
        {
            OrderBuilder orderBuilder = new OrderBuilder();
            orderBuilder.UsersId = getUsersId();

            SqlCommand useCommand = new SqlCommand($"USE {DefaultDatabaseName}\n", connection);
            useCommand.ExecuteNonQuery();

            for (int i = 0; i < DefaultNumberOfOrders; ++i)
            {
                // создаем команду
                SqlCommand command = new SqlCommand($"InsertInto{OrdersTableName}", connection);
                command.CommandType = System.Data.CommandType.StoredProcedure;

                // Создаем заказ
                orderBuilder.BuildOrder();
                Order order = orderBuilder.GetResult();

                SqlParameter customerIdparam = new SqlParameter("@customerId", order.UserId);
                command.Parameters.Add(customerIdparam);

                SqlParameter sumParam = new SqlParameter("@sum", order.Summ);
                command.Parameters.Add(sumParam);

                SqlParameter dateParam = new SqlParameter("@date", order.DateTime);
                command.Parameters.Add(dateParam);

                command.ExecuteNonQuery();
            }
        }

        #endregion Заполнение таблиц


        #region Чтение из БД

        /// <summary>
        /// Чтение юзеров из ДБ и добавление их в список UserTableItems
        /// </summary>
        /// <param name="connection">SQL соединение</param>
        private void ReadUserTableItemsFromDB(SqlConnection connection)
        {
            UserTableItems.Clear();

            string sqlExpression = $"USE {DefaultDatabaseName}\nSELECT * FROM {UsersTableName}";

            SqlCommand sqlCommand = new SqlCommand(sqlExpression, connection);

            // из microsoft
            // Предоставляет способ чтения потока строк последовательного доступа из базы данных SQL Server
            using (SqlDataReader reader = sqlCommand.ExecuteReader())
            {
                // Read - возвращеает true, если есть что читать, если нет - false
                while (reader.Read())
                {
                    var userTableItem = new UserTableItem
                    {
                        Number = ++_lastUserNumber,
                        Id = reader.GetInt32(0),
                        Name = reader.GetString(1),
                        PhoneNumber = reader.GetString(2)
                    };

                    UserTableItems.Add(userTableItem);
                }
            }
        }

        /// <summary>
        /// Чтение юзеров из ДБ и добавление их в передаваемый список userTableItems
        /// </summary>
        /// <param name="connection">SQL соединение</param>
        /// <param name="userTableItems">Передаваемый список, в который предаются прочитанные зачения</param>
        private void ReadUserTableItemsFromDB(SqlConnection connection, ref ObservableCollection<UserTableItem> userTableItems)
        {
            if (userTableItems == null)
                userTableItems = new ObservableCollection<UserTableItem>();

            int numberCounter = 0;

            string sqlExpression = $"USE {DefaultDatabaseName}\nSELECT * FROM {UsersTableName}";

            SqlCommand sqlCommand = new SqlCommand(sqlExpression, connection);

            // из microsoft
            // Предоставляет способ чтения потока строк последовательного доступа из базы данных SQL Server
            using (SqlDataReader reader = sqlCommand.ExecuteReader())
            {
                // Read - возвращеает true, если есть что читать, если нет - false
                while (reader.Read())
                {
                    var userTableItem = new UserTableItem
                    {
                        Number = ++numberCounter,
                        Id = reader.GetInt32(0),
                        Name = reader.GetString(1),
                        PhoneNumber = reader.GetString(2)
                    };

                    userTableItems.Add(userTableItem);
                }
            }
        }

        private void ReadOrderTableItemsFromDB(SqlConnection connection)
		{
            OrderTableItems.Clear();

            OrderTableItem.MaxCustomerNumber = UserTableItems.Count;

            string sqlExpression = $"USE {DefaultDatabaseName}\nSELECT * FROM {OrdersTableName}";

            SqlCommand sqlCommand = new SqlCommand(sqlExpression, connection);

            // из microsoft
            // Предоставляет способ чтения потока строк последовательного доступа из базы данных SQL Server
            using (SqlDataReader reader = sqlCommand.ExecuteReader())
            {
                // Read - возвращеает true, если есть что читать, если нет - false
                while (reader.Read())
                {
                    var orderTableItem = new OrderTableItem
                    {
                        Number = ++_lastOrderNumber,                       
                        Id = reader.GetInt32(0),
                        UserNumber = GetNumberOfUserById(reader.GetInt32(1)),
                        UserId = reader.GetInt32(1),
                        Summ = reader.GetInt32(2),
                        DateTime = reader.GetDateTime(3),
					};

                    OrderTableItems.Add(orderTableItem);
                }
            }
        }

        private void ReadOrderTableItemsFromDB(SqlConnection connection, ref ObservableCollection<OrderTableItem> orderTableItems)
        {
            if (orderTableItems == null)
                orderTableItems = new ObservableCollection<OrderTableItem>();

            int numberCounter = 0;

            string sqlExpression = $"USE {DefaultDatabaseName}\nSELECT * FROM {OrdersTableName}";

            SqlCommand sqlCommand = new SqlCommand(sqlExpression, connection);

            // из microsoft
            // Предоставляет способ чтения потока строк последовательного доступа из базы данных SQL Server
            using (SqlDataReader reader = sqlCommand.ExecuteReader())
            {
                // Read - возвращеает true, если есть что читать, если нет - false
                while (reader.Read())
                {
                    var orderTableItem = new OrderTableItem
                    {
                        Number = ++numberCounter,
                        Id = reader.GetInt32(0),
                        UserNumber = GetNumberOfUserById(reader.GetInt32(1)),
                        UserId = reader.GetInt32(1),
                        Summ = reader.GetInt32(2),
                        DateTime = reader.GetDateTime(3),
                    };

                    orderTableItems.Add(orderTableItem);
                }
            }
        }

        #endregion Чтение из БД

        /// <summary>
        /// Передает список id юзеров 
        /// </summary>
        /// <returns></returns>
        private List<int> getUsersId()
        {
            List<int> usersId = new List<int>();

            foreach(var user in UserTableItems)
            {
                usersId.Add(user.Id);
            }

            return usersId;
        }

        /// <summary>
        /// Проверяет, существует ли текущая БД на сервере. При наличии - возвращает true, иначе false
        /// </summary>
        /// <param name="connection">SQL соединение</param>
        /// <returns></returns>
		bool HasDatabase(SqlConnection connection)
		{
            string sqlExpression = "SELECT name\n" +
                                   "FROM sys.databases;";

            SqlCommand sqlCommand = new SqlCommand(sqlExpression, connection);

            // из microsoft
            // Предоставляет способ чтения потока строк последовательного доступа из базы данных SQL Server
            using (SqlDataReader reader = sqlCommand.ExecuteReader())
            {
                // Read - возвращеает true, если есть что читать, если нет - false
                while (reader.Read())
                {
                    // Если текущая бд уже создана
                    if (reader.GetString(0) == DefaultDatabaseName)
                        return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Получить номер пользователя по Id
        /// </summary>
        /// <param name="id">id пользователя</param>
        /// <returns></returns>
        private int GetNumberOfUserById(int id)
		{
            foreach(var user in UserTableItems)
			{
                if (user.Id == id)
                    return user.Number;
			}

            return 0;
		}

        public int GetIdOfUserByNumber(int number)
        {
            foreach (var user in UserTableItems)
            {
                if (user.Number == number)
                    return user.Id;
            }

            return 0;
        }

        private void UpdateParametersLabel()
        {
            if(!OrderTableItems.IsNullOrEmpty())
            {
                var tempList = OrderTableItems.ToList().Select(order => order.Summ).ToList();

                MinOrderSum = tempList.Min();
                MaxOrderSum = tempList.Max();
                TotalOrderSum = (ulong)tempList.Sum();
            }
            else
            {
                MinOrderSum = 0;
                MaxOrderSum = 0;
                TotalOrderSum = 0;
            }
        }

        private void ResetNewUserTableItem()
        {
            NewUserTableItem = new UserTableItem();
        }

        private void ResetNewOrderTableItem()
        {
            NewOrderTableItem = new OrderTableItem();
        }

        private void AddNewUserToList(SqlConnection connection)
        {
            // Команда для чтения последнего (наибольшего) id в таблице
            SqlCommand selectIdCommand = new SqlCommand($"SELECT TOP 1 Id FROM {UsersTableName} ORDER BY Id DESC", connection);

            selectIdCommand.ExecuteNonQuery();

            int id = 0;

            // из microsoft
            // Предоставляет способ чтения потока строк последовательного доступа из базы данных SQL Server
            using (SqlDataReader reader = selectIdCommand.ExecuteReader())
            {
                // Read - возвращеает true, если есть что читать, если нет - false
                while (reader.Read())
                {
                    id = reader.GetInt32(0);
                }
            }

			NewUserTableItem.Number = ++_lastUserNumber;
            NewUserTableItem.Id = id;

			UserTableItems.Add(NewUserTableItem);

            ResetNewUserTableItem();

        }

        private void AddNewOrderToList(SqlConnection connection)
        {
            // Команда для чтения последнего (наибольшего) id в таблице
            SqlCommand selectIdCommand = new SqlCommand($"SELECT TOP 1 Id FROM {OrdersTableName} ORDER BY Id DESC", connection);

            selectIdCommand.ExecuteNonQuery();

            int id = 0;

            // из microsoft
            // Предоставляет способ чтения потока строк последовательного доступа из базы данных SQL Server
            using (SqlDataReader reader = selectIdCommand.ExecuteReader())
            {
                // Read - возвращеает true, если есть что читать, если нет - false
                while (reader.Read())
                {
                    id = reader.GetInt32(0);
                }
            }

            NewOrderTableItem.Number = ++_lastOrderNumber;
            NewOrderTableItem.Id = id;

            OrderTableItems.Add(NewOrderTableItem);

            ResetNewOrderTableItem();
        }

    }
}
