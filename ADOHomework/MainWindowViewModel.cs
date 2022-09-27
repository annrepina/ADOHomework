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
        //private const string _dataBaseName = "ADOHomework";
        //private bool _hasDataBaseName;
        //private bool _canTryToConnect;
        //private UserBuilder _userBuilder;
 

        private const int DefaultNumberOfUsers = 10;

        public event PropertyChangedEventHandler? PropertyChanged;

        public ObservableCollection<User> Users{ get; set; }



        /// <summary>
        /// Есть подкючение?
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
            //OnClickCommand = new DelegateCommand(OnClickAsync);
            OnFillDBCommand = new DelegateCommand(OnFillDBAsync);
            _serverName = "";
            _isNotConnected = true;
            _hasCorrectServerName = false;
            //_dataBaseName = "ADOHomework";
            //_hasDataBaseName = false;
            //_canTryToConnect = false;
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

                    // При уда
                    IsNotConnected = false;

				    // Выводим айди нашего сеанса
				    MessageBox.Show($"Connected: {connection.ClientConnectionId}");

                    //string sqlExpression = "USE ADOHomework\n INSERT INTO Users VALUES(@name, @phonember)";

                    CreateDatabase(connection);

                    CreateTables(connection);

                    CreateProcedureInsertUsers(connection);

                    FillUsersTable(connection);

                    //ReadUsersFromDB(connection);

                    string sqlExpression = "SELECT * FROM Users";

                    SqlCommand sqlCommand = new SqlCommand(sqlExpression, connection);

                    // из microsoft
                    // Предоставляет способ чтения потока строк последовательного доступа из базы данных SQL Server
                    using (SqlDataReader reader = await sqlCommand.ExecuteReaderAsync())
                    {
                        // ReadAsync - возвращеает true, если есть что читать, если нет - false
                        while (await reader.ReadAsync())
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

                    var a = Users.Count;

                    //// Создаем новую комманду sql 
                    //// Задаем текст запроса
                    //// Создать БД
                    //SqlCommand command = new SqlCommand(sqlExpression, connection);

                    ////////command.Parameters.AddWithValue("@name", "Andrew");
                    ////////            command.Parameters.AddWithValue("@phonember", "89006788778");

                    //////// Создаем sql параметр
                    ////SqlParameter databaseNameParam = new SqlParameter("@ADOHomework", System.Data.SqlDbType.Variant);
                    ////databaseNameParam.Value = "ADOHomework";

                    ////command.Parameters.Add(databaseNameParam);

                    //// Выполняем команду
                    //await command.ExecuteNonQueryAsync();

                    MessageBox.Show($"Database called {connection.Database} created");




                    // Заполнение таблицы значениями по умолчанию

                    //            // из microsoft
                    //            // Предоставляет способ чтения потока строк последовательного доступа из базы данных SQL Server
                    //            using (SqlDataReader reader = await command.ExecuteReaderAsync())
                    //{
                    //                // ReadAsync - возвращеает true, если есть что читать, если нет - false
                    //                while (await reader.ReadAsync())
                    //                {
                    //                    var user = new User
                    //                    {
                    //                        Id = reader.GetInt32(0),
                    //                        Name = reader.GetString(1),
                    //                        PhoneNumber = reader.GetString(2)
                    //                    };

                    //                    Users.Add(user);
                    //                }
                    //}

                    //SqlCommand sqlCommand = new SqlCommand();
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
        private async void CreateDatabase(SqlConnection connection)
        {
            // создаем запрос для созданя бд
            string sqlExpression = $"CREATE DATABASE ADOHomework";

            // Создаем новую комманду sql 
            // Задаем текст запроса
            // Создать БД
            SqlCommand command = new SqlCommand(sqlExpression, connection);

            // Выполняем команду
            await command.ExecuteNonQueryAsync();
        }

        /// <summary>
        /// Создать таблицы в БД
        /// </summary>
        /// <param name="connection">SQL соединение</param>
        private async void CreateTables(SqlConnection connection)
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
            await command.ExecuteNonQueryAsync();
        }

        /// <summary>
        /// Создать хранимую процедуру для вставки значений в таблицу Users
        /// </summary>
        /// <param name="connection">SQL соединение</param>
        private async void CreateProcedureInsertUsers(SqlConnection connection)
        {
            string createProcedureStr =
                            "CREATE PROCEDURE[dbo].[InsertIntoUsers]\n" +
                                "@name varchar(100),\n" +
                                "@phoneNumber varchar(11)\n" +
                            "AS\n" +
                                "INSERT INTO Users([Name], PhoneNumber)\n" +
                                "VALUES(@name, @phoneNumber)\n";

            SqlCommand command = new SqlCommand(createProcedureStr, connection);

            await command.ExecuteNonQueryAsync();
        }

        /// <summary>
        /// Заполнить таблицу юзеров
        /// </summary>
        private async void FillUsersTable(SqlConnection connection)
        {
            // Создаем транзакцию
            SqlTransaction transaction = connection.BeginTransaction();

            // Создаем список команд
            List<SqlCommand> commands = new List<SqlCommand>();

            UserBuilder userBuilder = new UserBuilder();

            for (int i = 0; i < DefaultNumberOfUsers; ++i)
            {
                // создаем команду
                SqlCommand command = new SqlCommand("InsertIntoUsers", connection);
                command.Transaction = transaction;
                command.CommandType = System.Data.CommandType.StoredProcedure;

                // Создаем юзера
                userBuilder.BuildUserRandomly();
                User user = userBuilder.GetResult();

                SqlParameter nameParam = new SqlParameter("@name", user.Name);
                command.Parameters.Add(nameParam);

                SqlParameter phoneNumberParam = new SqlParameter("@phoneNumber", user.PhoneNumber);
                command.Parameters.Add(phoneNumberParam);

                commands.Add(command);
            }

            try
            {
                for(int i = 0; i < DefaultNumberOfUsers; ++i)
                {
                    var success = await commands[i].ExecuteNonQueryAsync();
                }

                await transaction.CommitAsync();
            }

            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);

                await transaction.RollbackAsync();
            }
        }

        private async void ReadUsersFromDB(SqlConnection connection)
        {
            string sqlExpression = "SELECT * FROM Users";

            SqlCommand sqlCommand = new SqlCommand(sqlExpression, connection);

            // из microsoft
            // Предоставляет способ чтения потока строк последовательного доступа из базы данных SQL Server
            using (SqlDataReader reader = await sqlCommand.ExecuteReaderAsync())
            {
                // ReadAsync - возвращеает true, если есть что читать, если нет - false
                while (await reader.ReadAsync())
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


    }
}
