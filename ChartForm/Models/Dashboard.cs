using InteractiveCenterLux.Db;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace InteractiveCenterLux.Models
{
    public struct RevenueByDate
    {
        public string Date { get; set; }
        public decimal TotalAmount { get; set; }
    }

    public class Dashboard : DbConnection
    {
        private DateTime startDate;
        private DateTime endDate;
        private int numberDays;
         
        public int NumProducts { get; set; }
        public int NumOrders { get; set; }
        public List<KeyValuePair<string, int>> TopProductsList = new List<KeyValuePair<string, int>>();
        public List<KeyValuePair<string, int>> UnderstockList = new List<KeyValuePair<string, int>>();
        public List<RevenueByDate> GrossRevenueList { get; private set; }
        public decimal TotalRevenue { get; set; }
        public decimal TotalProfit { get; set; }

        public Dashboard()
        {

        }

        private void GetNumberItems()
        {
            using (var connection = GetConnection())
            {
                connection.Open();

                using (var command = new NpgsqlCommand())
                {
                    command.Connection = connection;

                    command.CommandText = "select count(*) from product";
                    NumProducts = Convert.ToInt32(command.ExecuteScalar());

                    command.CommandText = @"select count(id) from ""order""
                                            where order_date between @fromDate and @toDate";
                    command.Parameters.Add("@fromDate", NpgsqlTypes.NpgsqlDbType.Date).Value = startDate;
                    command.Parameters.Add("@toDate", NpgsqlTypes.NpgsqlDbType.Date).Value = endDate;

                    NumOrders = Convert.ToInt32(command.ExecuteScalar());
                }
            }
        }

        private void GetOrderAnalisys()
        {
            GrossRevenueList = new List<RevenueByDate>();
            TotalProfit = 0;
            TotalRevenue = 0;

            using (var connection = GetConnection())
            {
                connection.Open();

                using (var command = new NpgsqlCommand())
                {
                    command.Connection = connection;

                    command.CommandText = @"select order_date, sum(total_amount) 
                                            from ""order"" 
                                            where order_date between @fromDate and @toDate
                                            group by order_date";
                    command.Parameters.Add("@fromDate", NpgsqlTypes.NpgsqlDbType.Date).Value = startDate;
                    command.Parameters.Add("@toDate", NpgsqlTypes.NpgsqlDbType.Date).Value = endDate;

                    var reader = command.ExecuteReader();

                    var resultTable = new List<KeyValuePair<DateTime, decimal>>();

                    while (reader.Read())
                    {
                            resultTable.Add(new KeyValuePair<DateTime, decimal>(Convert.ToDateTime(reader[0]), Convert.ToDecimal(reader[1])));
                        TotalRevenue += Convert.ToDecimal(reader[1]);
                    }

                    TotalProfit = TotalRevenue * 0.2m;

                    reader.Close();

                    //Group by hours
                    if (numberDays <= 1)
                    {
                        GrossRevenueList = (from orderList in resultTable
                                            group orderList by orderList.Key.ToString("hh tt")
                                     into order
                                            select new RevenueByDate
                                            {
                                                Date = order.Key,
                                                TotalAmount = order.Sum(amount => amount.Value)
                                            }).ToList();
                    }
                    //Group by days
                    else if(numberDays <= 30)
                    {
                        GrossRevenueList = (from orderList in resultTable
                                            group orderList by orderList.Key.ToString("dd MMM")
                                     into order
                                            select new RevenueByDate
                                            {
                                                Date = order.Key,
                                                TotalAmount = order.Sum(amount => amount.Value)
                                            }).ToList();
                    }
                    //Group by weeks
                    else if (numberDays <= 92)
                    {
                          GrossRevenueList = (from orderList in resultTable
                                    group orderList by CultureInfo.CurrentCulture.Calendar.GetWeekOfYear(
                                        orderList.Key, CalendarWeekRule.FirstDay, DayOfWeek.Monday)
                                     into order
                                    select new RevenueByDate
                                    {
                                        Date = "Week " + order.Key.ToString(),
                                        TotalAmount = order.Sum(amount => amount.Value)
                                    }).ToList();
                    }
                    //Group by months
                    else if (numberDays <= (365 * 2))
                    {
                        bool isYear = numberDays <= 365 ? true : false;
                        GrossRevenueList = (from orderList in resultTable
                                            group orderList by orderList.Key.ToString("MMM yyyy")
                                     into order
                                            select new RevenueByDate
                                            {
                                                Date = isYear? order.Key.Substring(0, order.Key.IndexOf(" ")) : order.Key,
                                                TotalAmount = order.Sum(amount => amount.Value)
                                            }).ToList();
                    }
                    //Group by year
                    else 
                    {
                        GrossRevenueList = (from orderList in resultTable
                                            group orderList by orderList.Key.ToString("yyyy")
                                    into order
                                            select new RevenueByDate
                                            {
                                                Date = order.Key,
                                                TotalAmount = order.Sum(amount => amount.Value)
                                            }).ToList();
                    }                    
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

                    command.CommandText = @"select p.name, sum(sold) as Q 
                                            from ""product"" p 
                                            inner join ""order"" o on p.id = o.product_id
                                            where o.order_date between @fromDate and @toDate
                                            group by name 
                                            order by Q desc
                                            limit (5)";
                    command.Parameters.Add("@fromDate", NpgsqlTypes.NpgsqlDbType.Date).Value = startDate;
                    command.Parameters.Add("@toDate", NpgsqlTypes.NpgsqlDbType.Date).Value = endDate;

                    var reader = command.ExecuteReader();

                    TopProductsList.Clear();

                    while (reader.Read())
                    {
                        TopProductsList.Add(new KeyValuePair<string, int>(reader[0].ToString(), Convert.ToInt32(reader[1])));
                    }                   


                    reader.Close();

                    command.CommandText = @"select p.name, sum(sold) as Q 
                                            from ""product"" p 
                                            inner join ""order"" o on p.id = o.product_id
                                            where o.order_date between @fromDate and @toDate
                                            group by name 
                                            order by Q
                                            limit (5)";
                    command.Parameters.Add("@fromDate", NpgsqlTypes.NpgsqlDbType.Date).Value = startDate;
                    command.Parameters.Add("@toDate", NpgsqlTypes.NpgsqlDbType.Date).Value = endDate;

                    reader = command.ExecuteReader();

                    UnderstockList.Clear();

                    while (reader.Read())
                    {
                        UnderstockList.Add(new KeyValuePair<string, int>(reader[0].ToString(), Convert.ToInt32(reader[1])));
                    }

                    reader.Close();
                }
            }
        }

        public bool LoadData(DateTime startDate, DateTime endDate)
        {
            endDate = new DateTime(endDate.Year, endDate.Month, endDate.Day,
                endDate.Hour, endDate.Minute, 59);
            if (this.startDate != startDate || this.endDate != endDate)
            {
                this.startDate = startDate;
                this.endDate = endDate;
                this.numberDays = (endDate - startDate).Days;

                GetNumberItems();
                GetOrderAnalisys();
                GetProductAnalisys();

                Console.WriteLine("Refreshaed data");

                return true;
            }
            else
            {
                Console.WriteLine("Data not refreshed");

                return false;
            }
        }
    }
}
