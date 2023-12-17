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

        Console.WriteLine("Welcome to Account Modification!"); //console message when starting the program
        DisplayAccountInfo();

        while (true) //loop everytime the user chooses options 1 or 2
        {
            Console.WriteLine("\nWhat would you like to change?:");
            Console.WriteLine("1. Modify Your User Name");
            Console.WriteLine("2. Modify Your Email");
            Console.WriteLine("3. Exit");

            string choice = Console.ReadLine();

            switch (choice)
            {
                case "1": //option 1
                    ModifyName();
                    break;
                case "2": //option 2
                    ModifyEmail();
                    break;
                case "3"://option 3
                    Environment.Exit(0);
                    break;
                default://in case the user has invalid input
                    Console.WriteLine("Invalid option. Please try again.");
                    break;
            }

            DisplayAccountInfo();
        }
    }

    static void DisplayAccountInfo() //shows your account information
    {
        Console.WriteLine("\nAccount Information:");
        foreach (var entry in accountInformation)
        {
            Console.WriteLine($"{entry.Key}: {entry.Value}");
        }
    }

    static void ModifyName() //changing your name
    {
        Console.Write("Enter your new user name: ");
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