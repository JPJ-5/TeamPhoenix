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
        private static string SystemPepper = "Vongisamazing";
        public static string GenerateSalt()
        {
            byte[] saltBytes = new byte[32];
            using (var rng = new RNGCryptoServiceProvider())
            {
                rng.GetBytes(saltBytes);
            }
            return Convert.ToBase64String(saltBytes);
        }

        // Hash the password using the provided salt and pepper
        public static string HashPassword(string password, string userSalt)
        {
            // Combine user salt, password, and system-wide pepper
            string saltedPassword = userSalt + password + SystemPepper;

            // Use a secure hash algorithm like SHA256
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(saltedPassword));
                return BitConverter.ToString(hashedBytes).Replace("-", "").ToLower();
            }
        }

        // Verify if the entered password matches the stored hash
        public static bool VerifyPassword(string enteredPassword, string storedHash, string salt)
        {
            string enteredHash = HashPassword(enteredPassword, salt);
            return storedHash == enteredHash;
        }
    }

    /*
        public class RandomValue{
            public static byte[] GenerateRandom(int size){
                var rng = RandomNumberGenerator.GetBytes(size)
                var rndom = 
            }
        }
    */
}
