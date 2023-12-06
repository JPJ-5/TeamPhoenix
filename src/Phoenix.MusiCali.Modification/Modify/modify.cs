//Author: Kihambo Muhumuza

using System;
using System.Collections.Generic;

class Program
{
    static Dictionary<string, string> accountInformation = new Dictionary<string, string>();

    static void Main()
    {
        // Initialize account information
        accountInformation.Add("Name", "John Doe");
        accountInformation.Add("Email", "john.doe@example.com");

        Console.WriteLine("Welcome to the Account Modification Program!"); //console message when starting the program
        DisplayAccountInfo();

        while (true) //loop everytime the user chooses options 1 or 2
        {
            Console.WriteLine("\nSelect an option:");
            Console.WriteLine("1. Modify Name");
            Console.WriteLine("2. Modify Email");
            Console.WriteLine("3. Exit");

            string choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    ModifyName();
                    break;
                case "2":
                    ModifyEmail();
                    break;
                case "3":
                    Environment.Exit(0);
                    break;
                default:
                    Console.WriteLine("Invalid option. Please try again.");
                    break;
            }

            DisplayAccountInfo();
        }
    }

    static void DisplayAccountInfo()
    {
        Console.WriteLine("\nAccount Information:");
        foreach (var entry in accountInformation)
        {
            Console.WriteLine($"{entry.Key}: {entry.Value}");
        }
    }

    static void ModifyName() //changing your name
    {
        Console.Write("Enter your new name: ");
        string newName = Console.ReadLine();
        accountInformation["Name"] = newName;
        Console.WriteLine("Name updated successfully!");
    }

    static void ModifyEmail() //changing your email
    {
        Console.Write("Enter your new email: ");
        string newEmail = Console.ReadLine();
        accountInformation["Email"] = newEmail;
        Console.WriteLine("Email updated successfully!");
    }
}