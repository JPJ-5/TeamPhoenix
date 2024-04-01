using System;
using System.Security.Cryptography;
using System.Text;

public class AuthenticationService
{
    // Simulated database or user repository
    private readonly UserRepository userRepository;

    public AuthService(UserRepository userRepository)
    {
        this.userRepository = userRepository;
    }

    public bool RegisterUser(string username, string email, string password)
    {
        // Check if the username or email is already registered
        if (userRepository.UserExists(username) || userRepository.EmailExists(email))
        {
            // User or email already exists
            return false;
        }

        // Generate a salt and hash the password
        byte[] salt = GenerateSalt();
        string hashedPassword = HashPassword(password, salt);

        // Create a new user
        User newUser = new User
        {
            Username = username,
            Email = email,
            PasswordHash = hashedPassword,
            Salt = salt
        };

        // Save the user to the repository (simulated)
        userRepository.AddUser(newUser);

        return true;
    }

    public bool AuthenticateUser(string username, string password)
    {
        // Retrieve the user from the repository
        User user = userRepository.GetUserByUsername(username);

        if (user != null)
        {
            // Hash the provided password with the user's salt
            string hashedPassword = HashPassword(password, user.Salt);

            // Compare the hashed password with the stored password hash
            return string.Equals(hashedPassword, user.PasswordHash);
        }

        return false; // User not found
    }

    // Other authentication-related methods can be added here

    // ... (methods for password recovery, session management, etc.)

    // Hashing password with salt
    private string HashPassword(string password, byte[] salt)
    {
        using (Rfc2898DeriveBytes pbkdf2 = new Rfc2898DeriveBytes(password, salt, 10000))
        {
            byte[] hash = pbkdf2.GetBytes(32); // 32 bytes = 256 bits
            return Convert.ToBase64String(hash);
        }
    }

    // Generate a random salt
    private byte[] GenerateSalt()
    {
        byte[] salt = new byte[16];
        using (RNGCryptoServiceProvider rngCsp = new RNGCryptoServiceProvider())
        {
            rngCsp.GetBytes(salt);
        }
        return salt;
    }
}
