using System.Collections.Generic;
using System.Data.SQLite;
using IntroSE.Kanban.Backend.DataAccessLayer.DTOs;

namespace IntroSE.Kanban.Backend.DataAccessLayer
{
    // Y - handles all SQL for the Users table: insert a new user, or load all users from DB
    public class UserController
    {
        private DataBaseManager _dbManager;

        public UserController(DataBaseManager dbManager)
        {
            _dbManager = dbManager;
        }

        // Y - parameterized query prevents SQL injection; always use @Param syntax with SQLite
        public void Insert(UserDTO dto)
        {
            using (SQLiteConnection con = _dbManager.GetConnection())
            {
                con.Open();
                string sql = "INSERT INTO Users (Email, Password) VALUES (@Email, @Password)";
                using (SQLiteCommand cmd = new SQLiteCommand(sql, con))
                {
                    cmd.Parameters.AddWithValue("@Email", dto.Email);
                    cmd.Parameters.AddWithValue("@Password", dto.Password);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        // Y - reads every row and wraps each in a DTO; the facade reconstructs User BL objects from these
        public List<UserDTO> LoadAll()
        {
            List<UserDTO> result = new List<UserDTO>();
            using (SQLiteConnection con = _dbManager.GetConnection())
            {
                con.Open();
                string sql = "SELECT Email, Password FROM Users";
                using (SQLiteCommand cmd = new SQLiteCommand(sql, con))
                using (SQLiteDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                        result.Add(new UserDTO(reader.GetString(0), reader.GetString(1)));
                }
            }
            return result;
        }
    }
}
