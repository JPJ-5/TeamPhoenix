using System.Dynamic;
using System.Data.SqlClient;
using System.Text.RegularExpressions;
using System;
using Phoenix.MusiCali.Models;

namespace Phoenix.MusiCali.DataAccesslayer
{

    public class DataAccess
    {
        private bool isValidUsername(string username)
        {
            return !string.IsNullOrWhiteSpace(username) && username.Length >= 6 && username.Length <= 30 && !username.Contains(" ");
        }

        private bool isValidEmail(string email)
        {
            string emailPattern = @"^[a-zA-Z0-9_.+-]+@[a-zA-Z0-9-]+\.[a-zA-Z0-9-.]+$";
            return !string.IsNullOrWhiteSpace(email) && System.Text.RegularExpressions.Regex.IsMatch(email, emailPattern);
        }

        private bool isValidDateOfBirth(DateTime dateOfBirth)
        {
            DateTime minDate = new DateTime(1970, 1, 1);
            return dateOfBirth >= minDate && dateOfBirth <= DateTime.Today;
        }

        private bool isValidPassword(string password)
        {
            return !string.IsNullOrWhiteSpace(password) && password.Length >= 8
                && containsLowercase(password) && containsUppercase(password) && containsSingleDigit(password);
        }

        private bool containsLowercase(string password)
        {
            return password.Any(char.IsLower);
        }

        private bool containsUppercase(string password)
        {
            return password.Any(char.IsUpper);
        }

        private bool containsSingleDigit(string password)
        {
            return password.Count(char.IsDigit) == 1;
        }

        public Result createLog(DateTime timestamp, string logLevel, string logCategory, string context)
        {
            var result = new Result();

            try
            {
                Console.WriteLine($"Logging - Timestamp: {timestamp}, Level: {logLevel}, Category: {logCategory}, Context: {context}");
                result.hasError = false;
            }
            catch (Exception ex)
            {
                result.hasError = true;
                result.errorMessage = ex.Message;
            }

            return result;
        }

        public Result createUser(string firstName, string lastName, string username, string email, DateTime dateOfBirth)
        {
            var result = new Result();

            if (!isValidUsername(username))
            {
                result.hasError = true;
                result.errorMessage = "Invalid username. Username must be between 6 and 30 characters, and cannot contain spaces.";
                return result;
            }

            if (!isValidEmail(email))
            {
                result.hasError = true;
                result.errorMessage = "Invalid email format.";
                return result;
            }

            if (!isValidDateOfBirth(dateOfBirth))
            {
                result.hasError = true;
                result.errorMessage = "Invalid date of birth. It should be between January 1st, 1970, and the current date.";
                return result;
            }

            try
            {
                var connectionString = "your_connection_string"; // Replace with connection string
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    var sql = "INSERT INTO users (firstName, lastName, username, email, dateOfBirth) " +
                              "VALUES (@FirstName, @LastName, @Username, @Email, @DateOfBirth)";
                    using (var command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@FirstName", firstName);
                        command.Parameters.AddWithValue("@LastName", lastName);
                        command.Parameters.AddWithValue("@Username", username);
                        command.Parameters.AddWithValue("@Email", email);
                        command.Parameters.AddWithValue("@DateOfBirth", dateOfBirth);
                        command.ExecuteNonQuery();
                    }
                }
                result.hasError = false;
            }
            catch (Exception ex)
            {
                result.hasError = true;
                result.errorMessage = ex.Message;
            }

            return result;
        }

        public int Users { get; private set; } // Updated to a private set

        // Additional methods or properties can be added here as needed
    }

    public class Logger
    {
        private DataAccess dao; // Create DataAccessObject once for Logger

        public Logger()
        {
            dao = new DataAccess();
        }

        public Result log(string logLevel, string logCategory, string context)
        {
            return dao.createLog(DateTime.UtcNow, logLevel, logCategory, context);
        }
    }
}
