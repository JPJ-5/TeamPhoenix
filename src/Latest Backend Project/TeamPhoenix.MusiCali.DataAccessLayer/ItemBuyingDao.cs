using Amazon.S3;
using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using TeamPhoenix.MusiCali.DataAccessLayer.Models;

namespace TeamPhoenix.MusiCali.DataAccessLayer
{
    public class ItemBuyingDAO
    {
        private readonly string connectionString;
        private readonly IConfiguration configuration;

        public ItemBuyingDAO(IConfiguration configuration)
        {
            this.configuration = configuration;
            this.connectionString = configuration.GetConnectionString("ConnectionString")!;

        }


        public async Task<bool> InsertRecieptTable(CraftReceiptModel receipt)
        {
            try
            {
                using (var connection = new MySqlConnection(connectionString))
                {
                    await connection.OpenAsync(); // Open the MySQL connection.
                    string commandText = @"
                    INSERT INTO CraftReceipt (CreatorHash, BuyerHash, SKU, SellPrice, OfferPrice, Profit, Revenue, Quantity, SaleDate, PendingSale)
                    VALUES (@CreatorHash, @BuyerHash, @SKU, @SellPrice, @OfferPrice, @Profit, @Revenue, @Quantity, @SaleDate, @PendingSale);";

                    using (var command = new MySqlCommand(commandText, connection))
                    {
                        // Assign parameters to prevent SQL Injection.
                        command.Parameters.AddWithValue("@CreatorHash", receipt.CreatorHash);
                        command.Parameters.AddWithValue("@BuyerHash", receipt.BuyerHash);
                        command.Parameters.AddWithValue("@SKU", receipt.SKU);
                        command.Parameters.AddWithValue("@SellPrice", receipt.SellPrice);
                        command.Parameters.AddWithValue("@OfferPrice", receipt.OfferPrice);
                        command.Parameters.AddWithValue("@Profit", receipt.Profit);
                        command.Parameters.AddWithValue("@Revenue", receipt.Revenue);
                        command.Parameters.AddWithValue("@Quantity", receipt.Quantity);
                        command.Parameters.AddWithValue("@SaleDate", receipt.SaleDate);
                        command.Parameters.AddWithValue("@PendingSale", receipt.PendingSale);

                        command.ExecuteNonQuery();
                        Console.WriteLine("Data inserted successfully!");
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error occurred: {ex.Message}");
                // Rethrow the exception after logging it
                return false;
            }
        }

        public string GetEmailByUserHash(string userHash)
        {
            string? email = null;
            try
            {
                using (var connection = new MySqlConnection(connectionString))
                {
                    connection.Open(); // Open the MySQL connection.
                    string commandText = "SELECT Email FROM UserAccount WHERE UserHash = @UserHash;";
                    using (var command = new MySqlCommand(commandText, connection))
                    {
                        command.Parameters.AddWithValue("@UserHash", userHash);
                        object result = command.ExecuteScalar();
                        if (result != null)
                        {
                            email = result.ToString();
                            Console.WriteLine($"Email found: {email}");
                        }
                        else
                        {
                            Console.WriteLine("No email found for the provided userHash.");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error occurred: {ex.Message}");
            }
            return email!;
        }

        public string GetUserHashBySku(string sku)
        {
            string? userHash = null;
            try
            {
                using (var connection = new MySqlConnection(connectionString))
                {
                    connection.Open(); // Open the MySQL connection.
                    string commandText = "SELECT CreatorHash FROM CraftItem WHERE SKU = @SKU;";
                    using (var command = new MySqlCommand(commandText, connection))
                    {
                        command.Parameters.AddWithValue("@SKU", sku);
                        object result = command.ExecuteScalar();
                        if (result != null)
                        {
                            userHash = result.ToString();
                            Console.WriteLine($"userHash found: {userHash}");
                        }
                        else
                        {
                            Console.WriteLine("No userHash found for the provided sku.");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error occurred: {ex.Message}");
            }
            return userHash!;
        }

    }
}