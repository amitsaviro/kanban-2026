using System;
using Microsoft.Data.Sqlite;
using System.IO;

namespace IntroSE.Kanban.Backend.DataAccessLayer
{
    /// <summary>
    /// Manages the SQLite database connection and schema.
    /// All controllers receive an instance of this class to get connections.
    /// </summary>
    public class DataBaseManager
    {
        private string _connectionString;

        public DataBaseManager()
        {
            // Y - relative path so kanban.db is always next to the compiled dll (Requirement 5b.6)
            string path = Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), "kanban.db"));
            _connectionString = $"Data Source={path}";
        }

        /// <summary>Returns an unopened SQLite connection. Callers must open and dispose it.</summary>
        public SqliteConnection GetConnection()
        {
            return new SqliteConnection(_connectionString);
        }

        /// <summary>
        /// Creates all tables if they do not already exist.
        /// Safe to call on every startup — IF NOT EXISTS makes it idempotent.
        /// </summary>
        public void CreateSchema()
        {
            using (SqliteConnection con = GetConnection())
            {
                con.Open();
                // Y - one big CREATE TABLE IF NOT EXISTS block; runs as a single batch
                string sql = @"
                    CREATE TABLE IF NOT EXISTS Users (
                        Email    TEXT PRIMARY KEY,
                        Password TEXT NOT NULL
                    );
                    CREATE TABLE IF NOT EXISTS Board (
                        Id          INTEGER PRIMARY KEY,
                        Name        TEXT    NOT NULL,
                        OwnerEmail  TEXT    NOT NULL,
                        NextTaskId  INTEGER NOT NULL DEFAULT 0
                    );
                    CREATE TABLE IF NOT EXISTS BoardMembers (
                        BoardId   INTEGER NOT NULL,
                        UserEmail TEXT    NOT NULL,
                        PRIMARY KEY (BoardId, UserEmail)
                    );
                    CREATE TABLE IF NOT EXISTS Column (
                        BoardId INTEGER NOT NULL,
                        Ordinal INTEGER NOT NULL,
                        Lim     INTEGER NOT NULL DEFAULT -1,
                        PRIMARY KEY (BoardId, Ordinal)
                    );
                    CREATE TABLE IF NOT EXISTS Task (
                        Id            INTEGER NOT NULL,
                        BoardId       INTEGER NOT NULL,
                        ColumnOrdinal INTEGER NOT NULL,
                        Title         TEXT    NOT NULL,
                        Description   TEXT,
                        DueDate       TEXT    NOT NULL,
                        CreationTime  TEXT    NOT NULL,
                        AssigneeEmail TEXT,
                        PRIMARY KEY (Id, BoardId)
                    );";
                using (SqliteCommand cmd = new SqliteCommand(sql, con))
                    cmd.ExecuteNonQuery();
            }
        }

        /// <summary>Deletes all rows from every table but keeps the schema intact.</summary>
        public void ClearDatabase()
        {
            using (SqliteConnection con = GetConnection())
            {
                con.Open();
                // Y - delete child tables before parent tables to respect foreign-key order
                string sql = @"
                    DELETE FROM Task;
                    DELETE FROM Column;
                    DELETE FROM BoardMembers;
                    DELETE FROM Board;
                    DELETE FROM Users;";
                using (SqliteCommand cmd = new SqliteCommand(sql, con))
                    cmd.ExecuteNonQuery();
            }
        }
    }
}
