using TeamPhoenix.MusiCali.DataAccessLayer;
public class Program
{
    public static void Main(string[] args)
    {
        MariaDB mariaDB = new MariaDB();
        bool test = mariaDB.connect();
        Console.WriteLine(test);
    }
}