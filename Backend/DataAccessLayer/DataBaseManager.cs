using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;
using System.IO;

namespace IntroSE.Kanban.Backend.DataAccessLayer
{
    public class DataBaseManager
    {
        private string connectionString;
        public DataBaseManager()
        {
            string path = Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), "Kanban.db"));
            Console.WriteLine(path);
            // Y - assign to the field, not a local variable, so GetConnection() can use it
            connectionString = $"Data Source={path}; Version=3;";
        }
        public SQLiteConnection GetConnection()
        {
            return new SQLiteConnection(connectionString);
        }
        public void ClearDatabase()
        {
            // Y - GetConnection() takes no arguments; connectionString is already stored in the field
            using(SQLiteConnection connection = GetConnection())
            {
                connection.Open();
                string clearQuary = @" 
                    DELETE FROM Task;
                    DELETE FROM Board;
                    DELETE FROM Users;
                    DELETE FROM BoardMembers;";
                using (SQLiteCommand command = new SQLiteCommand(clearQuary, connection)) 
                {
                    try
                    {
                        command.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error while cleaning DataBase: {ex.Message}");
                        throw;
                    }
                }
            }
        }
        public static void Main(string[] args)
        {
            DataBaseManager manager = new DataBaseManager();
            using(SQLiteConnection connection = manager.GetConnection())
            {
                connection.Open();
                Console.WriteLine("open succefuly!");
            }
        }
       
    }
}
