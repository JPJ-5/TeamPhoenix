using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System;
using System.Security.Cryptography;

namespace Phoenix.MusiCali.Models
{
    public class Hasher
    {
        // Generate a random salt
        public static string GenerateSalt()
        {
            byte[] saltBytes = new byte[32];
            using (var rng = new RNGCryptoServiceProvider())
            {
                rng.GetBytes(saltBytes);
            }
            return Convert.ToBase64String(saltBytes);
        }

        // Hash the password using the provided salt
        public static string HashPassword(string password, string salt)
        {
            using (var sha256 = SHA256.Create())
            {
                byte[] passwordBytes = Encoding.UTF8.GetBytes(password + salt);
                byte[] hashedBytes = sha256.ComputeHash(passwordBytes);
                return Convert.ToBase64String(hashedBytes);
            }
        }

        // Verify if the entered password matches the stored hash
        public static bool VerifyPassword(string enteredPassword, string storedHash, string salt)
        {
            string enteredHash = HashPassword(enteredPassword, salt);
            return storedHash == enteredHash;
        }
    }
}
