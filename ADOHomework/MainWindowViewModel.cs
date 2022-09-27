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

        //public string DataBaseName 
        //{ 
        //    get => _dataBaseName;

        //    set 
        //    {
        //        _dataBaseName = value;

        //        HasDataBaseName = true;
        //    }
        //}

   //     public bool HasDataBaseName 
   //     { 
   //         get => _hasDataBaseName; 

   //         set
			//{
   //             _hasDataBaseName = value;

   //             CanTryToConnect = _hasCorrectConnectionString && _hasDataBaseName;

   //             OnPropertyChanged(nameof(HasDataBaseName));
			//}
   //     }

  //      public bool CanTryToConnect
		//{
  //          get => _canTryToConnect;

  //          set
		//	{
  //              _canTryToConnect = value;

  //              OnPropertyChanged(nameof(CanTryToConnect));
		//	}
		//}

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
                    "[Date] date NOT NULL\n" +
                ")";

            // Создаем новую комманду sql 
            // Задаем текст запроса
            // Создать БД
            SqlCommand command = new SqlCommand(sqlExpression, connection);

            // Выполняем команду
            await command.ExecuteNonQueryAsync();
        }

        private async void FillTablesByDefault()
        {
            string 
        }
            

    }
}
