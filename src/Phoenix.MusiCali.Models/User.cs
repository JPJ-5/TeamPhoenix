namespace Phoenix.MusiCali.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string userHash { get; set; }

        public RegistrationStatus GetRegistrationStatus()
        {
            // Implement logic to determine the registration status
            // This might involve checking if the user has completed the registration process
            // You can replace this with your actual logic
            return RegistrationStatus.Completed;
        }

        public bool IsLoggedIn()
        {
            // Implement logic to determine if the user is logged in
            // This might involve checking if the user has an active session
            // You can replace this with your actual logic
            return true;
        }

        public bool HasActiveSession()
        {
            // Implement logic to determine if the user has an active session
            // You can replace this with your actual logic
            return true;
        }

        // Other methods...
    }

    public enum RegistrationStatus
    {
        Incomplete,
        Completed,
        // Add other statuses as needed
    }

        // Additional properties, if needed

 }
