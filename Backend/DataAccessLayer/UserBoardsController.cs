using System.Collections.Generic;
using System.Data.SQLite;
using IntroSE.Kanban.Backend.DataAccessLayer.DTOs;

namespace IntroSE.Kanban.Backend.DataAccessLayer
{
    // Y - handles all SQL for the BoardMembers table (which users belong to which boards)
    public class UserBoardsController
    {
        private DataBaseManager _dbManager;

        public UserBoardsController(DataBaseManager dbManager)
        {
            _dbManager = dbManager;
        }

        public void Insert(UserBoardsDTO dto)
        {
            using (SQLiteConnection con = _dbManager.GetConnection())
            {
                con.Open();
                string sql = "INSERT INTO BoardMembers (BoardId, UserEmail) VALUES (@BoardId, @UserEmail)";
                using (SQLiteCommand cmd = new SQLiteCommand(sql, con))
                {
                    cmd.Parameters.AddWithValue("@BoardId", dto.BoardId);
                    cmd.Parameters.AddWithValue("@UserEmail", dto.UserEmail);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public void Delete(int boardId, string userEmail)
        {
            using (SQLiteConnection con = _dbManager.GetConnection())
            {
                con.Open();
                string sql = "DELETE FROM BoardMembers WHERE BoardId = @BoardId AND UserEmail = @UserEmail";
                using (SQLiteCommand cmd = new SQLiteCommand(sql, con))
                {
                    cmd.Parameters.AddWithValue("@BoardId", boardId);
                    cmd.Parameters.AddWithValue("@UserEmail", userEmail);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public void DeleteByBoard(int boardId)
        {
            using (SQLiteConnection con = _dbManager.GetConnection())
            {
                con.Open();
                string sql = "DELETE FROM BoardMembers WHERE BoardId = @BoardId";
                using (SQLiteCommand cmd = new SQLiteCommand(sql, con))
                {
                    cmd.Parameters.AddWithValue("@BoardId", boardId);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public List<UserBoardsDTO> LoadAll()
        {
            List<UserBoardsDTO> result = new List<UserBoardsDTO>();
            using (SQLiteConnection con = _dbManager.GetConnection())
            {
                con.Open();
                string sql = "SELECT BoardId, UserEmail FROM BoardMembers";
                using (SQLiteCommand cmd = new SQLiteCommand(sql, con))
                using (SQLiteDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                        result.Add(new UserBoardsDTO(reader.GetInt32(0), reader.GetString(1)));
                }
            }
            return result;
        }
    }
}
