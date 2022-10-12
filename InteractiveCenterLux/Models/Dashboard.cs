using Arction.WinForms.Charting;
using InteractiveCenterLux.Db;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InteractiveCenterLux.Models
{
    public class Dashboard : DbConnection
    {
        // Properties
        public int NumProducts { get; set; }
        public List<KeyValuePair<string, int>> TopProductsList { get; set; }
        public List<KeyValuePair<string, int>> UnderstockList { get; set; }

        private void GetNumberItems()
        {
            using (var connection = GetConnection())
            {
                connection.Open();

                using (var command = new NpgsqlCommand())
                {
                    command.Connection = connection;

                    command.CommandText = "select count(*) from product";
                    NumProducts = (int)command.ExecuteScalar();
                }
            }
        }

        private void GetProductAnalisys()
        {
            using (var connection = GetConnection())
            {
                connection.Open();

                using (var command = new NpgsqlCommand())
                {
                    command.Connection = connection;

                    command.CommandText = @"select top 5 p.name, sum(sold) as Q 
                                            from product p 
                                            group by name 
                                            order by Q desc";

                    var reader = command.ExecuteReader();

                    while (reader.Read())
                    {
                        TopProductsList.Add(new KeyValuePair<string, int>(reader[0].ToString(), (int)reader[1]));
                    }

                    reader.Close();



                    command.CommandText = @"select top 5 p.name, sum(sold) as Q 
                                            from product p 
                                            group by name 
                                            order by Q";

                    reader = command.ExecuteReader();

                    while (reader.Read())
                    {
                        UnderstockList.Add(new KeyValuePair<string, int>(reader[0].ToString(), (int)reader[1]));
                    }

                    reader.Close();
                }
            }
        }

        public bool LoadData()
        {
            GetProductAnalisys();

            return true;
        }
    }
}
