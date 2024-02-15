using System.Security.Cryptography;
using System.Text;
namespace TeamPhoenix.MusiCali.Security
{
    public class Hasher
    {
        private static readonly char[] ValidCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ".ToCharArray();
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

        public static string GenerateOTP()
        {
            // Create a random number generator
            using (var rng = new RNGCryptoServiceProvider())
            {
                // Generate an array of random bytes
                byte[] randomBytes = new byte[8];
                rng.GetBytes(randomBytes);

                // Convert the random bytes to characters
                char[] otpChars = new char[8];
                for (int i = 0; i < 8; i++)
                {
                    otpChars[i] = ValidCharacters[randomBytes[i] % ValidCharacters.Length];
                }

                return new string(otpChars);
            }
        }
    }
}