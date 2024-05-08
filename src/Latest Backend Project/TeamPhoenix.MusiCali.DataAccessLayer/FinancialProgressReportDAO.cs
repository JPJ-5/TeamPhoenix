using Amazon;
using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TeamPhoenix.MusiCali.DataAccessLayer.Models;

namespace TeamPhoenix.MusiCali.DataAccessLayer
{
    public class FinancialProgressReportDAO
    {
        private string connectionString;
        private readonly IConfiguration configuration;
        public FinancialProgressReportDAO(IConfiguration configuration)
        {
            this.configuration = configuration;
            this.connectionString = configuration.GetConnectionString("ConnectionString")!;
        }

        public HashSet<FinancialInfoModel> FetchYearlyReport(string userHash)
        {
            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    HashSet<FinancialInfoModel> infos = new HashSet<FinancialInfoModel>();
                    // Open the database connection
                    connection.Open();
                    string sql = "SELECT YEAR(SaleDate) AS Year, SUM(Profit) AS TotalProfit, SUM(Revenue) AS TotalRevenue, COUNT(ReceiptID) AS NumberOfSales "
                            + "FROM CraftReceipt "
                            + "WHERE CreatorHash = @CreatorHash GROUP BY YEAR(SaleDate) AND PendingSale = @PendingSale";

                    // Create a SqlCommand to execute the SQL
                    using (MySqlCommand command = new MySqlCommand(sql, connection))
                    {
                        // Add the UserId parameter to the command
                        command.Parameters.AddWithValue("@CreatorHash", userHash);
                        command.Parameters.AddWithValue("@PendingSale", false)

                        using (MySqlDataReader reader = command.ExecuteReader())
                        {
                            // Read each record
                            while (reader.Read())
                            {
                                infos.Add(new FinancialInfoModel
                                {
                                    financialYear = reader.GetInt32(reader.GetOrdinal("Year")),
                                    financialProfit = reader.GetDecimal(reader.GetOrdinal("TotalProfit")),
                                    financialRevenue = reader.GetDecimal(reader.GetOrdinal("TotalRevenue")),
                                    sales = reader.GetInt32(reader.GetOrdinal("NumberOfSales"))
                                });
                            }
                        }
                        return infos;
                    }
                }
            }
            catch(SqlException ex)
            {
                Console.WriteLine(ex.Message);
                return new HashSet<FinancialInfoModel>();
            }
            catch(Exception ex1)
            {
                Console.WriteLine(ex1.Message);
                return new HashSet<FinancialInfoModel>();
            }
        }

        public HashSet<FinancialInfoModel> FetchQuarterlyReport(string userHash)
        {
            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    HashSet<FinancialInfoModel> infos = new HashSet<FinancialInfoModel>();
                    // Open the database connection
                    connection.Open();
                    
                    string sql = "SELECT YEAR(SaleDate) AS Year, QUARTER(SaleDate) AS Quarter, SUM(Profit) AS TotalProfit, SUM(Revenue) AS TotalRevenue, COUNT(ReceiptID) AS NumberOfSales "
                            + "FROM CraftReceipt "
                            + "WHERE CreatorHash = @CreatorHash " +
                            "GROUP BY YEAR(SaleDate), QUARTER(SaleDate)";

                    // Create a SqlCommand to execute the SQL
                    using (MySqlCommand command = new MySqlCommand(sql, connection))
                    {
                        // Add the UserId parameter to the command
                        command.Parameters.AddWithValue("@CreatorHash", userHash);

                        using (MySqlDataReader reader = command.ExecuteReader())
                        {
                            // Read each record
                            while (reader.Read())
                            {
                                infos.Add(new FinancialInfoModel
                                {
                                    financialYear = reader.GetInt32(reader.GetOrdinal("Year")),
                                    financialQuater = reader.GetInt32(reader.GetOrdinal("Quarter")),
                                    financialProfit = reader.GetDecimal(reader.GetOrdinal("TotalProfit")),
                                    financialRevenue = reader.GetDecimal(reader.GetOrdinal("TotalRevenue")),
                                    sales = reader.GetInt32(reader.GetOrdinal("NumberOfSales"))
                                });
                            }
                        }
                        return infos;
                    }
                }
            }
            catch (SqlException ex)
            {
                Console.WriteLine(ex.Message);
                return new HashSet<FinancialInfoModel>();
            }
            catch (Exception ex1)
            {
                Console.WriteLine(ex1.Message);
                return new HashSet<FinancialInfoModel>();
            }
        }

        public HashSet<FinancialInfoModel> FetchMonthlyReport(string userHash)
        {
            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    HashSet<FinancialInfoModel> infos = new HashSet<FinancialInfoModel>();
                    // Open the database connection
                    connection.Open();
                    string sql = "SELECT YEAR(SaleDate) AS Year, MONTH(SaleDate) AS Month, SUM(Profit) AS TotalProfit, SUM(Revenue) AS TotalRevenue, COUNT(ReceiptID) AS NumberOfSales "
                            + "FROM CraftReceipt "
                            + "WHERE CreatorHash = @CreatorHash "
                            + "GROUP BY YEAR(SaleDate), MONTH(SaleDate)";

                    // Create a SqlCommand to execute the SQL
                    using (MySqlCommand command = new MySqlCommand(sql, connection))
                    {
                        // Add the UserId parameter to the command
                        command.Parameters.AddWithValue("@CreatorHash", userHash);

                        using (MySqlDataReader reader = command.ExecuteReader())
                        {
                            // Read each record
                            while (reader.Read())
                            {
                                infos.Add(new FinancialInfoModel
                                {
                                    financialYear = reader.GetInt32(reader.GetOrdinal("Year")),
                                    financialMonth = reader.GetInt32(reader.GetOrdinal("Month")),
                                    financialProfit = reader.GetDecimal(reader.GetOrdinal("TotalProfit")),
                                    financialRevenue = reader.GetDecimal(reader.GetOrdinal("TotalRevenue")),
                                    sales = reader.GetInt32(reader.GetOrdinal("NumberOfSales"))
                                });
                            }
                        }
                        return infos;
                    }
                }
            }
            catch (SqlException ex)
            {
                Console.WriteLine(ex.Message);
                return new HashSet<FinancialInfoModel>();
            }
            catch (Exception ex1)
            {
                Console.WriteLine(ex1.Message);
                return new HashSet<FinancialInfoModel>();
            }
        }
    }
}
