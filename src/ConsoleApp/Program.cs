using TeamPhoenix.MusiCali.DataAccessLayer;
using uc = TeamPhoenix.MusiCali.Services.UserCreation;
public class Program
{
    public static void Main(string[] args)
    {
        MariaDB mariaDB = new MariaDB();
        bool test = mariaDB.connect();
        Console.WriteLine(test);

        DateTime dob = new DateTime(2001, 01, 26);
        bool test2 = uc.RegisterUser("joshuareyes@gmail.com", dob, "Julie0126", "Julie", "Reyes", "What is your pets name", "Ace");

        Console.WriteLine(test2);

    }


}