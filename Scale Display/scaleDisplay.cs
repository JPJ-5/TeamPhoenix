using System;

namespace ScaleDisplay
{
    class ScaleDisplay {         
        static void Main(string[] args)
        {
            
            Console.WriteLine("\nWelcome to Scale Display!");

            Boolean condition = true;
            
            while(condition){
                string name = " ";
                string type = " ";
                int scaleName;
                int scaleType;


                Console.WriteLine("\nPlease select which scale you'd like to see from the menu: ");
                Console.WriteLine("\n1. A \n2. B \n3. C \n4. D \n5. E \n6. F \n7. G \n8. Quit");

                scaleName = Convert.ToInt32(Console.ReadLine());

                if(scaleName == 1){

                    name = "A";
                }

                if(scaleName == 8){

                    Console.WriteLine("Exiting feature. Goodbye");

                    condition = false;
                    break;
                }

                Console.WriteLine("\nWhat type of scale?: \n1. Major \n2. Minor");

                scaleType = Convert.ToInt32(Console.ReadLine());

                Console.WriteLine("\nYou've chosen the " + name + " " + type + " scale.");
            }
        }
    }
}