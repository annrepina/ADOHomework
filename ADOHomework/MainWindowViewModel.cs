using Microsoft.Data.SqlClient;
using Prism.Commands;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ADOHomework
{
    /// <summary>
    /// ViewModel
    /// </summary>
    public class MainWindowViewModel : INotifyPropertyChanged
    {
		private string _serverName;
		private bool _isNotConnected;
		private bool _hasCorrectServerName;
        private bool _isConnected;

        //private const string _dataBaseName = "ADOHomework";
        //private bool _hasDataBaseName;
        //private bool _canTryToConnect;
        //private UserBuilder _userBuilder;


        public const int DefaultNumberOfUsers = 10;
        public const int DefaultNumberOfOrders = 15;

        public event PropertyChangedEventHandler? PropertyChanged;

        public ObservableCollection<User> Users{ get; set; }



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

                //CanTryToConnect = _hasCorrectConnectionString && _hasDataBaseName;

                OnPropertyChanged(nameof(HasCorrectServerName));
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

                HasCorrectServerName = true;
            }
		}

        public DelegateCommand OnFillDBCommand { get; }

        //public DelegateCommand OnClickCommand { get; }

        
        public MainWindowViewModel()
        {
            OnFillDBCommand = new DelegateCommand(OnFillDBAsync);
            _serverName = "";
            _isNotConnected = true;
            _hasCorrectServerName = false;
            Users = new ObservableCollection<User>();
            //_userBuilder = new UserBuilder();
        }

        private async void OnFillDBAsync()
        {
            // Строка подключения
            //_connectionString = "Server=DESKTOP-BHIRPIK\\MYSUPERDB;Database=master;Trusted_Connection=True;Encrypt=False";
            string _connectionString = $"Server={_serverName};Database=master;Trusted_Connection=True;Encrypt=False;MultipleActiveResultSets=True;";

            try
            {
               // Класс с помощью которого мы подключаеся к sql серверу 
                using (SqlConnection connection = new SqlConnection(_connectionString))
			    {
				    // Открываем подключение
				    await connection.OpenAsync();

                    IsNotConnected = false;
                    IsConnected = true;

				    // Выводим айди нашего сеанса
				    MessageBox.Show($"Connected: {connection.ClientConnectionId}");

                    CreateDatabase(connection);

                    CreateTables(connection);

                    CreateProcedureInsertUsers(connection);
                    CreateProcedureInsertOrders(connection);

                    FillUsersTable(connection);
                    ReadUsersFromDB(connection);

                    FillOrdersTable(connection);

                    MessageBox.Show($"Database called {connection.Database} created");

                    var a = Users.Count;
                }
            }
            catch (Exception ex)
            {
                // сбрасываем значение т.к. нет подключения
                IsNotConnected = true;

                MessageBox.Show(ex.Message);
            }
        }

        private void OnPropertyChanged(string propName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
        }

        /// <summary>
        /// Создать Базу данных
        /// </summary>
        /// <param name="connection">SQL соединение</param>
        private void CreateDatabase(SqlConnection connection)
        {
            // создаем запрос для созданя бд
            string sqlExpression = $"CREATE DATABASE ADOHomework";

            // Создаем новую комманду sql 
            // Задаем текст запроса
            // Создать БД
            SqlCommand command = new SqlCommand(sqlExpression, connection);

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
            string sqlExpression = "USE ADOHomework\n" +
                "CREATE TABLE Users\n" +
                "(\n" +
                    "Id int PRIMARY KEY IDENTITY NOT NULL,\n" +
                    "[Name] varchar(100) NOT NULL,\n" +
                    "PhoneNumber varchar(11) UNIQUE NOT NULL\n" +
                ")\n" +
                "CREATE TABLE Orders\n" +
                "(\n" +
                    "Id int PRIMARY KEY IDENTITY NOT NULL,\n" +
                    "CustomerId int REFERENCES Users (Id) NOT NULL,\n" +
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
                            "CREATE PROCEDURE[dbo].[InsertIntoUsers]\n" +
                                "@name varchar(100),\n" +
                                "@phoneNumber varchar(11)\n" +
                            "AS\n" +
                                "INSERT INTO Users([Name], PhoneNumber)\n" +
                                "VALUES(@name, @phoneNumber)\n";

            SqlCommand command = new SqlCommand(createProcedureStr, connection);

            command.ExecuteNonQuery();
        }

        /// <summary>
        /// Заполнить таблицу юзеров
        /// </summary>
        /// <param name="connection">SQL соединение</param>
        private void FillUsersTable(SqlConnection connection)
        {
            UserBuilder userBuilder = new UserBuilder();

            for (int i = 0; i < DefaultNumberOfUsers; ++i)
            {
                // создаем команду
                SqlCommand command = new SqlCommand("InsertIntoUsers", connection);
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

        /// <summary>
        /// Чтение юзеров из ДБ
        /// </summary>
        /// <param name="connection"></param>
        private void ReadUsersFromDB(SqlConnection connection)
        {
            string sqlExpression = "SELECT * FROM Users";

            SqlCommand sqlCommand = new SqlCommand(sqlExpression, connection);

            // из microsoft
            // Предоставляет способ чтения потока строк последовательного доступа из базы данных SQL Server
            using (SqlDataReader reader = sqlCommand.ExecuteReader())
            {
                // ReadAsync - возвращеает true, если есть что читать, если нет - false
                while (reader.Read())
                {
                    var user = new User
                    {
                        Id = reader.GetInt32(0),
                        Name = reader.GetString(1),
                        PhoneNumber = reader.GetString(2)
                    };

                    Users.Add(user);
                }
            }
        }

        /// <summary>
        /// Создать хранимую процедуру для вставки значения в таблиицу заказов
        /// </summary>
        /// <param name="connection">SQL соединение</param>
        private void CreateProcedureInsertOrders(SqlConnection connection)
        {
            string createProcedureStr =
                "CREATE PROCEDURE[dbo].[InsertIntoOrders]\n" +
                    "@customerId int,\n" +
                    "@sum int,\n" +
                    "@date datetime\n" +
                "AS\n" +
                    "INSERT INTO Orders (CustomerId, Summ, [Date])\n" +
                    "VALUES (@customerId, @sum, @date)\n";

            SqlCommand command = new SqlCommand(createProcedureStr, connection);

            command.ExecuteNonQuery();
        }

        private void FillOrdersTable(SqlConnection connection)
        {
            OrderBuilder orderBuilder = new OrderBuilder();
            orderBuilder.UsersId = getUsersId();

            for (int i = 0; i < DefaultNumberOfOrders; ++i)
            {
                // создаем команду
                SqlCommand command = new SqlCommand("InsertIntoOrders", connection);
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
        
        /// <summary>
        /// Передает список id юзеров 
        /// </summary>
        /// <returns></returns>
        private List<int> getUsersId()
        {
            List<int> usersId = new List<int>();

            foreach(var user in Users)
            {
                usersId.Add(user.Id);
            }

            return usersId;
        }
    }
}
