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
        public MainWindowViewModel()
        {
            OnConnectToServerCommand = new DelegateCommand(OnConnectToServerAsync);
            OnFillDatabaseCommand = new DelegateCommand(OnFillDatabaseAsync);
            OnAddNewUserCommand = new DelegateCommand(OnAddNewUserAsync);
            _serverName = "";
            _connectionString = "";
            _isNotConnected = true;
            _hasCorrectServerName = false;
            _databaseIsNotFilled = true;
            _databaseIsFilled = false;
            UserTableItems = new ObservableCollection<UserTableItem>();
            OrderTableItems = new ObservableCollection<OrderTableItem>();
            UserTableItems.CollectionChanged += OnUserTableItemsListPropertyChanged;
            OrderTableItems.CollectionChanged += OnOrderTableItemsListPropertyChanged;

            _lastUserNumber = 0;
            _lastOrderNumber = 0;
            _minOrderSum = 0;
            _maxOrderSum = 0;
            _totalOrderSum = 0;

            NewUserTableItem = new UserTableItem();
            NewOrderTableitem = new OrderTableItem();

        }

        private string _serverName;
        private string _connectionString;
		private bool _isNotConnected;
		private bool _hasCorrectServerName;
        private bool _isConnected;
        /// <summary>
        /// Последний номер пользователя в таблице
        /// </summary>
        private int _lastUserNumber;

        /// <summary>
        /// Последний номер заказа в таблице
        /// </summary>
        private int _lastOrderNumber;
        private bool _databaseIsNotFilled;
        private bool _databaseIsFilled;
        private bool _canConnect;
        private int _minOrderSum;
        private int _maxOrderSum;
        private ulong _totalOrderSum;



        #region Константы

        private const int DefaultNumberOfUsers = 10;
        private const int DefaultNumberOfOrders = 15;

        private const string DefaultDatabaseName = "ADOHomework";
        private const string UsersTableName = "Users";
        private const string OrdersTableName = "Orders";

        #endregion Константы

        #region Свойства

        public event PropertyChangedEventHandler? PropertyChanged;

        public ObservableCollection<UserTableItem> UserTableItems{ get; set; }

        public ObservableCollection<OrderTableItem> OrderTableItems{ get; set; }

        public UserTableItem NewUserTableItem { get; set; }
        public OrderTableItem NewOrderTableitem { get; set; }

        public int MinOrderSum
        {
            get => _minOrderSum;

            set
            {
                _minOrderSum = value;

                OnPropertyChanged(nameof(MinOrderSum));
            }
        }

        public int MaxOrderSum
        {
            get => _maxOrderSum;

            set
            {
                _maxOrderSum = value;

                OnPropertyChanged(nameof(MaxOrderSum));
            }
        }

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

                //CanTryToConnect = _hasCorrectConnectionString && _hasDataBaseName;

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
        /// Стока подключения
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

        public bool DatabaseIsNotFilled
        {
            get => _databaseIsNotFilled;
            
            set
            {
                _databaseIsNotFilled = value;

                OnPropertyChanged(nameof(DatabaseIsNotFilled));
            }
        }

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

        public DelegateCommand OnFillDatabaseCommand { get; }

        public DelegateCommand OnConnectToServerCommand { get; }

        public DelegateCommand OnAddNewUserCommand { get; }

        #endregion DelegateCommands


        #region Обработчики DelegateCommands

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

                MessageBox.Show(ex.Message);
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
            if(NewUserTableItem)
            NewUserTableItem = new UserTableItem()
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
                    $"CustomerId int REFERENCES {UsersTableName} (Id) NOT NULL,\n" +
                    "Summ int NOT NULL,\n" +
                    "[Date] datetime NOT NULL\n" +
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
    }
}
