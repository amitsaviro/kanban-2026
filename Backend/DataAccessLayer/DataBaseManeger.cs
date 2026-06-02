using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;
using System.IO;

namespace IntroSE.Kanban.Backend.DataAccessLayer
{
    public class DataBaseManeger
    {
        private string connectionString;
        public DataBaseManeger() 
        { 
            string path = Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), "Kanban.db"));
            Console.WriteLine(path);
            string connectionString = $"Data Source={path}; Version=3;";
        }
        public SQLiteConnection GetConnection()
        {
            return new SQLiteConnection(connectionString);
        }
        public void ClearDatabase()
        {
            using(SQLiteConnection connection = GetConnection(connectionString))
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
            DataBaseManeger maneger = new DataBaseManeger();
            using(SQLiteConnection connection = maneger.GetConnection())
            {
                connection.Open();
                Console.WriteLine("open succefuly!");
            }
        }
       
    }
}
