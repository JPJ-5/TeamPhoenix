using MySql.Data.MySqlClient;
using Org.BouncyCastle.Asn1.X509;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TeamPhoenix.MusiCali.DataAccessLayer.Models;
using logU = TeamPhoenix.MusiCali.Logging.Logger;
using authN = TeamPhoenix.MusiCali.DataAccessLayer.Authentication;


namespace TeamPhoenix.MusiCali.DataAccessLayer
{
    public class RecoverUser
    {
        public static UserRecovery GetUserRecovery(string username)
        {
            string connectionString = "Server=3.142.241.151;Database=MusiCali;User ID=julie;Password=j1234;";
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();

                string selectUserProfileSql = "SELECT * FROM UserRecovery WHERE Username = @Username";
                using (MySqlCommand cmd = new MySqlCommand(selectUserProfileSql, connection))
                {
                    cmd.Parameters.AddWithValue("@Username", username);

                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new UserRecovery(
                                reader["Username"].ToString(),
                                reader["Question"].ToString(),
                                reader["Answer"].ToString());
                        }
                    }
                }
            }
            return null;
        }
        

        public static string GetUserHash(string username)
        {
            string connectionString = "Server=3.142.241.151;Database=MusiCali;User ID=julie;Password=j1234;";
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();

                string selectUserProfileSql = "SELECT UserHash FROM UserAccount WHERE Username = @Username";
                using (MySqlCommand cmd = new MySqlCommand(selectUserProfileSql, connection))
                {
                    cmd.Parameters.AddWithValue("@Username", username);

                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            string userH = new string(
                                reader["UserHash"].ToString()
                            );
                            return userH;
                        }
                    }
                }
            }
            return null;
        }

        public static bool updateUserR(UserRecovery userR)
        {
            string connectionString = "Server=3.142.241.151;Database=MusiCali;User ID=julie;Password=j1234;";

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                try
                {
                    connection.Open();

                    // Update data in UserProfile table
                    string insertUserRecoverySql = "UPDATE UserRecovery SET Username = @Username, Question = @Question, Answer = @Answer, SuccessRecovery = @SuccessRecovery WHERE Username = @Username";
                    using (MySqlCommand cmd = new MySqlCommand(insertUserRecoverySql, connection))
                    {
                        cmd.Parameters.AddWithValue("@Username", userR.Username);
                        cmd.Parameters.AddWithValue("@Question", userR.Question);
                        cmd.Parameters.AddWithValue("@Answer", userR.Answer);
                        cmd.Parameters.AddWithValue("@SuccessRecovery", userR.Success);
                        cmd.ExecuteNonQuery();
                    }
                }
                catch (Exception ex)
                {
                    // You can add more specific exception handling if needed
                    throw new Exception($"Error updating UserProfile: {ex.Message}");
                }
                UserAuthN theUser = Authentication.findUsernameInfo(userR.Username).userA;

                if(!EnableUser(theUser)) {
                    return false;
                }
                string userHash = GetUserHash(userR.Username);
                string level = "Info";
                string category = "View";
                string context = "Recover User";
                logU logDis = new logU();
                Result wow = logDis.CreateLog(userHash, level, category, context);
                return true;
            }
        }

        public static bool DisableUser(UserAuthN userAuthN)
        {
            string connectionString = "Server=3.142.241.151;Database=MusiCali;User ID=julie;Password=j1234;";

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                try
                {
                    connection.Open();

                    // Update data in UserProfile table
                    string insertUserRecoverySql = "UPDATE UserAuthN SET IsDisabled = @IsDisabled WHERE Username = @Username";
                    using (MySqlCommand cmd = new MySqlCommand(insertUserRecoverySql, connection))
                    {
                        cmd.Parameters.AddWithValue("@Username", userAuthN.Username);
                        cmd.Parameters.AddWithValue("@IsDisabled", true);
                        cmd.ExecuteNonQuery();
                    }
                }
                catch (Exception ex)
                {
                    // You can add more specific exception handling if needed
                    throw new Exception($"Error updating UserAuthN: {ex.Message}");
                }
                string userHash = GetUserHash(userAuthN.Username);
                string level = "Info";
                string category = "View";
                string context = "Disable User";
                logU logDis = new logU();
                Result wow = logDis.CreateLog(userHash, level, category, context);
                return true;
            }
        }

        public static bool EnableUser(UserAuthN userAuthN)
        {
            string connectionString = "Server=3.142.241.151;Database=MusiCali;User ID=julie;Password=j1234;";

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                try
                {
                    connection.Open();

                    // Update data in UserProfile table
                    string insertUserRecoverySql = "UPDATE UserAuthN SET IsDisabled = @IsDisabled WHERE Username = @Username";
                    using (MySqlCommand cmd = new MySqlCommand(insertUserRecoverySql, connection))
                    {
                        cmd.Parameters.AddWithValue("@Username", userAuthN.Username);
                        cmd.Parameters.AddWithValue("@IsDisabled", false);
                        cmd.ExecuteNonQuery();
                    }
                }
                catch (Exception ex)
                {
                    // You can add more specific exception handling if needed
                    throw new Exception($"Error updating UserAuthN: {ex.Message}");
                }
                string userHash = GetUserHash(userAuthN.Username);
                string level = "Info";
                string category = "View";
                string context = "Enable User";
                logU logDis = new logU();
                Result wow = logDis.CreateLog(userHash, level, category, context);
                return true;
            }

        }

    }
}
