using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Patterns
{
    public class AbstractFactoryDb
    {
        static void Main(string[] args)
        {
            LogEntry logEntry = new LogEntry();
            LogSaver sqlLiteSaver = new LogSaver(SqliteFactory.Instance);

            if (sqlLiteSaver != null)
            {
                Console.WriteLine("Введите текст для записи в базу:");
                logEntry.Text = Console.ReadLine();
                sqlLiteSaver.Save(new List<LogEntry> { logEntry });
            }
        }
    }

    public class LogEntry
    {
        public string? Text { get; set; }
    }


    // Fabric client
    public class LogSaver
    {
        private readonly DbProviderFactory _dbProviderFactory;

        public LogSaver(DbProviderFactory dbProviderFactory)
        {
            _dbProviderFactory = dbProviderFactory;
        }

        public void Save(IEnumerable<LogEntry> logEntries)
        {
            if (_dbProviderFactory != null)
            {
                using var dbConnection = _dbProviderFactory.CreateConnection();
                if (dbConnection != null)
                {
                    SetDbConnectionString(dbConnection);
                    using var dbCommand = _dbProviderFactory.CreateCommand();
                    if (dbCommand != null)
                    {
                        SetCommandArguments(logEntries);
                        dbCommand.ExecuteNonQuery();
                    }
                }
            }
        }

        public IEnumerable<LogEntry> Read()
        {
            if (_dbProviderFactory != null)
            {
                using var dbConnection = _dbProviderFactory.CreateConnection();
                if (dbConnection != null)
                {
                    SetDbConnectionString(dbConnection);
                    using var dbCommand = _dbProviderFactory.CreateCommand();
                    if (dbCommand != null)
                    {
                        var dbReader =  dbCommand.ExecuteReader();
                    }
                }
            }

            return null;
        }

        private void SetDbConnectionString(DbConnection? dbConnection) { }
        private void SetCommandArguments(IEnumerable<LogEntry> logEntries) { }
    }
}
