using TeamPhoenix.MusiCali.DataAccessLayer.Models;

public class UpdateClaimsRequest
{
    public string Username { get; set; } = string.Empty;
    public UserRoles Claims { get; set; } = new UserRoles();
}