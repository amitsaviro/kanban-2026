using System;
using System.Collections.Generic;
using System.Data.SQLite;
using IntroSE.Kanban.Backend.DataAccessLayer.DTOs;

namespace IntroSE.Kanban.Backend.DataAccessLayer
{
    // Y - handles all SQL for the Task table; tasks are keyed by (Id, BoardId) together
    public class TaskController
    {
        private DataBaseManager _dbManager;

        public TaskController(DataBaseManager dbManager)
        {
            _dbManager = dbManager;
        }

        public void Insert(TaskDTO dto)
        {
            using (SQLiteConnection con = _dbManager.GetConnection())
            {
                con.Open();
                string sql = @"INSERT INTO Task (Id, BoardId, ColumnOrdinal, Title, Description, DueDate, CreationTime, AssigneeEmail)
                               VALUES (@Id, @BoardId, @ColumnOrdinal, @Title, @Description, @DueDate, @CreationTime, @AssigneeEmail)";
                using (SQLiteCommand cmd = new SQLiteCommand(sql, con))
                {
                    cmd.Parameters.AddWithValue("@Id", dto.Id);
                    cmd.Parameters.AddWithValue("@BoardId", dto.BoardId);
                    cmd.Parameters.AddWithValue("@ColumnOrdinal", dto.ColumnOrdinal);
                    cmd.Parameters.AddWithValue("@Title", dto.Title);
                    cmd.Parameters.AddWithValue("@Description", (object)dto.Description ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@DueDate", dto.DueDate);
                    cmd.Parameters.AddWithValue("@CreationTime", dto.CreationTime);
                    cmd.Parameters.AddWithValue("@AssigneeEmail", (object)dto.AssigneeEmail ?? DBNull.Value);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public void UpdateColumnOrdinal(int boardId, int taskId, int columnOrdinal)
        {
            Update(boardId, taskId, "ColumnOrdinal", columnOrdinal);
        }

        public void UpdateTitle(int boardId, int taskId, string title)
        {
            Update(boardId, taskId, "Title", title);
        }

        public void UpdateDescription(int boardId, int taskId, string description)
        {
            Update(boardId, taskId, "Description", (object)description ?? DBNull.Value);
        }

        public void UpdateDueDate(int boardId, int taskId, string dueDate)
        {
            Update(boardId, taskId, "DueDate", dueDate);
        }

        public void UpdateAssignee(int boardId, int taskId, string email)
        {
            Update(boardId, taskId, "AssigneeEmail", (object)email ?? DBNull.Value);
        }

        public void DeleteByBoard(int boardId)
        {
            using (SQLiteConnection con = _dbManager.GetConnection())
            {
                con.Open();
                string sql = "DELETE FROM Task WHERE BoardId = @BoardId";
                using (SQLiteCommand cmd = new SQLiteCommand(sql, con))
                {
                    cmd.Parameters.AddWithValue("@BoardId", boardId);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public List<TaskDTO> LoadAll()
        {
            List<TaskDTO> result = new List<TaskDTO>();
            using (SQLiteConnection con = _dbManager.GetConnection())
            {
                con.Open();
                string sql = "SELECT Id, BoardId, ColumnOrdinal, Title, Description, DueDate, CreationTime, AssigneeEmail FROM Task";
                using (SQLiteCommand cmd = new SQLiteCommand(sql, con))
                using (SQLiteDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        result.Add(new TaskDTO(
                            reader.GetInt32(0),
                            reader.GetInt32(1),
                            reader.GetInt32(2),
                            reader.GetString(3),
                            reader.IsDBNull(4) ? null : reader.GetString(4),
                            reader.GetString(5),
                            reader.GetString(6),
                            reader.IsDBNull(7) ? null : reader.GetString(7)));
                    }
                }
            }
            return result;
        }

        // Y - column name comes from our own code (not user input) so string interpolation is safe here
        private void Update(int boardId, int taskId, string column, object value)
        {
            using (SQLiteConnection con = _dbManager.GetConnection())
            {
                con.Open();
                string sql = $"UPDATE Task SET {column} = @Value WHERE Id = @TaskId AND BoardId = @BoardId";
                using (SQLiteCommand cmd = new SQLiteCommand(sql, con))
                {
                    cmd.Parameters.AddWithValue("@Value", value);
                    cmd.Parameters.AddWithValue("@TaskId", taskId);
                    cmd.Parameters.AddWithValue("@BoardId", boardId);
                    cmd.ExecuteNonQuery();
                }
            }
        }
    }
}
