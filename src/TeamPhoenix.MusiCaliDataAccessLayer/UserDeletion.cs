using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using TeamPhoenix.MusiCali.DataAccessLayer.Models;

namespace TeamPhoenix.MusiCali.DataAccessLayer
{
    public class UserDeletion
    {
        public static Result DeleteUser(UserAccount userA)
        {
            try
            {
                string connectionString = "Server=3.142.241.151;Database=MusiCali;User ID=julie;Password=j1234;";
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();

                    using (MySqlTransaction transaction = connection.BeginTransaction())
                    {
                        string deleteUserAccountQuery = "DELETE FROM UserAccount WHERE Username = @Username";
                        using (MySqlCommand cmdDeleteUserAccount = new MySqlCommand(deleteUserAccountQuery, connection, transaction))
                        {
                            cmdDeleteUserAccount.Parameters.AddWithValue("@Username", userA.Username);
                            cmdDeleteUserAccount.ExecuteNonQuery();
                        }
                    }
                }
                Result result = new Result();
                result.HasError = false;
                result.Success = true;
                return result;
            } 
            catch (Exception ex)
            {
                Result res = new Models.Result();
                res.ErrorMessage = ex.Message;
                res.HasError = true;
                return res;
            }

        }
    }
}
