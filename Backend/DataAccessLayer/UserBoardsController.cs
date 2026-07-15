using System.Collections.Generic;
using Microsoft.Data.Sqlite;
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
            using (SqliteConnection con = _dbManager.GetConnection())
            {
                con.Open();
                string sql = "INSERT INTO BoardMembers (BoardId, UserEmail) VALUES (@BoardId, @UserEmail)";
                using (SqliteCommand cmd = new SqliteCommand(sql, con))
                {
                    cmd.Parameters.AddWithValue("@BoardId", dto.BoardId);
                    cmd.Parameters.AddWithValue("@UserEmail", dto.UserEmail);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public void Delete(int boardId, string userEmail)
        {
            using (SqliteConnection con = _dbManager.GetConnection())
            {
                con.Open();
                string sql = "DELETE FROM BoardMembers WHERE BoardId = @BoardId AND UserEmail = @UserEmail";
                using (SqliteCommand cmd = new SqliteCommand(sql, con))
                {
                    cmd.Parameters.AddWithValue("@BoardId", boardId);
                    cmd.Parameters.AddWithValue("@UserEmail", userEmail);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public void DeleteByBoard(int boardId)
        {
            using (SqliteConnection con = _dbManager.GetConnection())
            {
                con.Open();
                string sql = "DELETE FROM BoardMembers WHERE BoardId = @BoardId";
                using (SqliteCommand cmd = new SqliteCommand(sql, con))
                {
                    cmd.Parameters.AddWithValue("@BoardId", boardId);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public List<UserBoardsDTO> LoadAll()
        {
            List<UserBoardsDTO> result = new List<UserBoardsDTO>();
            using (SqliteConnection con = _dbManager.GetConnection())
            {
                con.Open();
                string sql = "SELECT BoardId, UserEmail FROM BoardMembers";
                using (SqliteCommand cmd = new SqliteCommand(sql, con))
                using (SqliteDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                        result.Add(new UserBoardsDTO(reader.GetInt32(0), reader.GetString(1)));
                }
            }
            return result;
        }
    }
}
