using System.Collections.Generic;
using Microsoft.Data.Sqlite;
using IntroSE.Kanban.Backend.DataAccessLayer.DTOs;

namespace IntroSE.Kanban.Backend.DataAccessLayer
{
    // Y - handles all SQL for the Board table
    public class BoardController
    {
        private DataBaseManager _dbManager;

        public BoardController(DataBaseManager dbManager)
        {
            _dbManager = dbManager;
        }

        public void Insert(BoardDTO dto)
        {
            using (SqliteConnection con = _dbManager.GetConnection())
            {
                con.Open();
                string sql = "INSERT INTO Board (Id, Name, OwnerEmail, NextTaskId) VALUES (@Id, @Name, @OwnerEmail, @NextTaskId)";
                using (SqliteCommand cmd = new SqliteCommand(sql, con))
                {
                    cmd.Parameters.AddWithValue("@Id", dto.Id);
                    cmd.Parameters.AddWithValue("@Name", dto.Name);
                    cmd.Parameters.AddWithValue("@OwnerEmail", dto.OwnerEmail);
                    cmd.Parameters.AddWithValue("@NextTaskId", dto.NextTaskId);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        // Y - called every time a task is added so the board's task-ID counter is durable across restarts
        public void UpdateNextTaskId(int boardId, int nextTaskId)
        {
            using (SqliteConnection con = _dbManager.GetConnection())
            {
                con.Open();
                string sql = "UPDATE Board SET NextTaskId = @NextTaskId WHERE Id = @Id";
                using (SqliteCommand cmd = new SqliteCommand(sql, con))
                {
                    cmd.Parameters.AddWithValue("@NextTaskId", nextTaskId);
                    cmd.Parameters.AddWithValue("@Id", boardId);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public void UpdateOwner(int boardId, string newOwnerEmail)
        {
            using (SqliteConnection con = _dbManager.GetConnection())
            {
                con.Open();
                string sql = "UPDATE Board SET OwnerEmail = @OwnerEmail WHERE Id = @Id";
                using (SqliteCommand cmd = new SqliteCommand(sql, con))
                {
                    cmd.Parameters.AddWithValue("@OwnerEmail", newOwnerEmail);
                    cmd.Parameters.AddWithValue("@Id", boardId);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public void Delete(int boardId)
        {
            using (SqliteConnection con = _dbManager.GetConnection())
            {
                con.Open();
                string sql = "DELETE FROM Board WHERE Id = @Id";
                using (SqliteCommand cmd = new SqliteCommand(sql, con))
                {
                    cmd.Parameters.AddWithValue("@Id", boardId);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public List<BoardDTO> LoadAll()
        {
            List<BoardDTO> result = new List<BoardDTO>();
            using (SqliteConnection con = _dbManager.GetConnection())
            {
                con.Open();
                string sql = "SELECT Id, Name, OwnerEmail, NextTaskId FROM Board";
                using (SqliteCommand cmd = new SqliteCommand(sql, con))
                using (SqliteDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                        result.Add(new BoardDTO(
                            reader.GetInt32(0),
                            reader.GetString(1),
                            reader.GetString(2),
                            reader.GetInt32(3)));
                }
            }
            return result;
        }
    }
}
