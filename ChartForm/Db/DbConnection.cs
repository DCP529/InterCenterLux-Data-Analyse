using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InteractiveCenterLux.Db
{
    public abstract class DbConnection
    {
        private readonly string _connectionString;

        public DbConnection()
        {
            _connectionString = "Host=localhost;" +
            "Port=5433;Database=LuxDatabase;Username=postgres;Password=super200;";
        }

        protected NpgsqlConnection GetConnection()
        {
            return new NpgsqlConnection(_connectionString);
        }
    }
}
