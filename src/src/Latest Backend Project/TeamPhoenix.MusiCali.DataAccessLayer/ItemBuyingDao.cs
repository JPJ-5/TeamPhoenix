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
        private readonly ItemPaginationDAO itemPaginationDAO;

        public ItemBuyingDAO(IConfiguration configuration, IAmazonS3 s3Client)
        {
            this.configuration = configuration;
            this.connectionString = configuration.GetConnectionString("ConnectionString")!;
            itemPaginationDAO = new ItemPaginationDAO(s3Client ,this.configuration);
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

        public async Task<bool> AcceptPendingSale(CraftReceiptModel receipt)
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                await connection.OpenAsync();
                using (var transaction = await connection.BeginTransactionAsync())
                {
                    try
                    {
                        // Update stock available in CraftItem
                        string updateStockQuery = @"UPDATE CraftItem
                                            SET StockAvailable = StockAvailable - @Quantity
                                            WHERE SKU = @SKU;";
                        using (var updateStockCommand = new MySqlCommand(updateStockQuery, connection, (MySqlTransaction)transaction))
                        {
                            updateStockCommand.Parameters.AddWithValue("@SKU", receipt.SKU);
                            updateStockCommand.Parameters.AddWithValue("@Quantity", receipt.Quantity);
                            await updateStockCommand.ExecuteNonQueryAsync();
                        }

                        // Update PendingSale to false in CraftReceipt
                        string updateReceiptQuery = @"UPDATE CraftReceipt
                                              SET PendingSale = 0
                                              WHERE ReceiptID = @ReceiptID;";
                        using (var updateReceiptCommand = new MySqlCommand(updateReceiptQuery, connection, (MySqlTransaction)transaction))
                        {
                            updateReceiptCommand.Parameters.AddWithValue("@ReceiptID", receipt.ReceiptID);
                            await updateReceiptCommand.ExecuteNonQueryAsync();
                        }

                        // Commit the transaction
                        await transaction.CommitAsync();
                        return true;
                    }
                    catch (Exception)
                    {
                        await transaction.RollbackAsync();
                        return false;
                    }
                }
            }
        }


        public async Task<bool> DeclinePendingSale(int receiptID)
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                await connection.OpenAsync();
                using (var transaction = await connection.BeginTransactionAsync())
                {
                    try
                    {
                        // Delete the receipt
                        string deleteReceiptQuery = @"DELETE FROM CraftReceipt
                                              WHERE ReceiptID = @ReceiptID AND PendingSale = 1;";
                        using (var deleteReceiptCommand = new MySqlCommand(deleteReceiptQuery, connection, (MySqlTransaction)transaction))
                        {
                            deleteReceiptCommand.Parameters.AddWithValue("@ReceiptID", receiptID);
                            await deleteReceiptCommand.ExecuteNonQueryAsync();
                        }

                        // Commit the transaction
                        await transaction.CommitAsync();
                        return true;
                    }
                    catch (Exception)
                    {
                        await transaction.RollbackAsync();
                        return false;
                    }
                }
            }
        }


        public async Task<(HashSet<CraftReceiptWithItemModel>, int count)> GetPendingReceiptsWithItemInfo(string? userHash, int pageNum, int pageSize)
        {
            var receipts = new HashSet<CraftReceiptWithItemModel>();
            int totalCount = 0;

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                await connection.OpenAsync();

                // Base query to get receipts with item details
                string baseQuery = @"FROM CraftReceipt cr LEFT JOIN CraftItem ci ON cr.SKU = ci.SKU WHERE cr.PendingSale = 1 AND ci.CreatorHash = @CreatorHash";

                // Query to get total count of pending receipts
                string countQuery = $@"SELECT COUNT(cr.ReceiptID) {baseQuery};";

                // Query to get paginated receipts with item details
                string dataQuery = $@"SELECT cr.ReceiptID, cr.SKU, cr.OfferPrice, cr.Quantity, cr.Profit, cr.Revenue, cr.SaleDate, ci.Name, ci.Price, ci.StockAvailable, ci.Image AS FirstImage {baseQuery} ORDER BY cr.SaleDate ASC LIMIT @Offset, @PageSize;";

                using (MySqlCommand countCommand = new MySqlCommand(countQuery, connection))
                {
                    countCommand.Parameters.AddWithValue("@CreatorHash", userHash ?? string.Empty);
                    totalCount = Convert.ToInt32(await countCommand.ExecuteScalarAsync());
                }

                using (MySqlCommand dataCommand = new MySqlCommand(dataQuery, connection))
                {
                    dataCommand.Parameters.AddWithValue("@CreatorHash", userHash ?? string.Empty);
                    dataCommand.Parameters.AddWithValue("@Offset", (pageNum - 1) * pageSize);
                    dataCommand.Parameters.AddWithValue("@PageSize", pageSize);

                    using (var reader = (MySqlDataReader)await dataCommand.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            string sku = reader.GetString("SKU");
                            string firstImageField = reader.IsDBNull(reader.GetOrdinal("FirstImage")) ? "" : reader.GetString("FirstImage");
                            List<string> imageUrls = firstImageField.Split(',').ToList();

                            var receiptWithItem = new CraftReceiptWithItemModel
                            {
                                ReceiptID = reader.GetInt32("ReceiptID"),
                                SKU = sku,
                                OfferPrice = reader.GetDecimal("OfferPrice"),
                                Quantity = reader.GetInt32("Quantity"),
                                Profit = reader.GetDecimal("Profit"),
                                Revenue = reader.GetDecimal("Revenue"),
                                SaleDate = reader.IsDBNull(reader.GetOrdinal("SaleDate")) ? (DateTime?)null : reader.GetDateTime("SaleDate"),
                                Item = new PaginationItemModel
                                {
                                    Name = reader.IsDBNull(reader.GetOrdinal("Name")) ? "Unknown" : reader.GetString("Name"),
                                    Sku = sku,
                                    Price = reader.GetDecimal("Price"),
                                    StockAvailable = reader.GetInt32("StockAvailable"),
                                    FirstImage = imageUrls.Count > 0 ? itemPaginationDAO.GetImageUrl(sku, imageUrls[0]) : null
                                }
                            };

                            receipts.Add(receiptWithItem);
                        }
                    }
                }
            }

            return (receipts, totalCount);
        }

        public async Task<CraftReceiptModel?> GetReceiptByIDAsync(int receiptID)
        {
            CraftReceiptModel? receipt = null;

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                await connection.OpenAsync();

                string query = @"
            SELECT
                ReceiptID,
                CreatorHash,
                BuyerHash,
                SKU,
                SellPrice,
                OfferPrice,
                Profit,
                Revenue,
                Quantity,
                SaleDate,
                PendingSale
            FROM
                CraftReceipt
            WHERE
                ReceiptID = @ReceiptID;";

                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@ReceiptID", receiptID);

                    using (MySqlDataReader reader = (MySqlDataReader)await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            receipt = new CraftReceiptModel
                            {
                                ReceiptID = reader.GetInt32("ReceiptID"),
                                CreatorHash = reader.GetString("CreatorHash"),
                                BuyerHash = reader.GetString("BuyerHash"),
                                SKU = reader.GetString("SKU"),
                                SellPrice = reader.GetDecimal("SellPrice"),
                                OfferPrice = reader.GetDecimal("OfferPrice"),
                                Profit = reader.GetDecimal("Profit"),
                                Revenue = reader.GetDecimal("Revenue"),
                                Quantity = reader.GetInt32("Quantity"),
                                SaleDate = reader.IsDBNull(reader.GetOrdinal("SaleDate")) ? (DateTime?)null : reader.GetDateTime("SaleDate"),
                                PendingSale = reader.GetBoolean("PendingSale")
                            };
                        }
                    }
                }
            }

            return receipt;
        }
    }
}