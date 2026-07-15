using System.Collections.Generic;
using Microsoft.Data.Sqlite;
using IntroSE.Kanban.Backend.DataAccessLayer.DTOs;

namespace IntroSE.Kanban.Backend.DataAccessLayer
{
    // Y - handles all SQL for the Column table; note "Lim" is the DB column name ("Limit" is a reserved SQL keyword)
    public class ColumnController
    {
        private DataBaseManager _dbManager;

        public ColumnController(DataBaseManager dbManager)
        {
            _dbManager = dbManager;
        }

        public void Insert(ColumnDTO dto)
        {
            using (SqliteConnection con = _dbManager.GetConnection())
            {
                con.Open();
                string sql = "INSERT INTO Column (BoardId, Ordinal, Lim) VALUES (@BoardId, @Ordinal, @Lim)";
                using (SqliteCommand cmd = new SqliteCommand(sql, con))
                {
                    cmd.Parameters.AddWithValue("@BoardId", dto.BoardId);
                    cmd.Parameters.AddWithValue("@Ordinal", dto.Ordinal);
                    cmd.Parameters.AddWithValue("@Lim", dto.Limit);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public void UpdateLimit(int boardId, int ordinal, int limit)
        {
            using (SqliteConnection con = _dbManager.GetConnection())
            {
                con.Open();
                string sql = "UPDATE Column SET Lim = @Lim WHERE BoardId = @BoardId AND Ordinal = @Ordinal";
                using (SqliteCommand cmd = new SqliteCommand(sql, con))
                {
                    cmd.Parameters.AddWithValue("@Lim", limit);
                    cmd.Parameters.AddWithValue("@BoardId", boardId);
                    cmd.Parameters.AddWithValue("@Ordinal", ordinal);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public void DeleteByBoard(int boardId)
        {
            using (SqliteConnection con = _dbManager.GetConnection())
            {
                con.Open();
                string sql = "DELETE FROM Column WHERE BoardId = @BoardId";
                using (SqliteCommand cmd = new SqliteCommand(sql, con))
                {
                    cmd.Parameters.AddWithValue("@BoardId", boardId);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public List<ColumnDTO> LoadAll()
        {
            List<ColumnDTO> result = new List<ColumnDTO>();
            using (SqliteConnection con = _dbManager.GetConnection())
            {
                con.Open();
                string sql = "SELECT BoardId, Ordinal, Lim FROM Column";
                using (SqliteCommand cmd = new SqliteCommand(sql, con))
                using (SqliteDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                        result.Add(new ColumnDTO(reader.GetInt32(0), reader.GetInt32(1), reader.GetInt32(2)));
                }
            }
            return result;
        }
    }
}
