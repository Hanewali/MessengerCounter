using System;
using System.Linq;

namespace MessengerCounter
{
    static class Program
    {
        public static string ConversationName { get; set; }
        
        public static void Main(string[] args)
        {

            if (args.Length == 0)
            {
                Console.WriteLine("ERROR: No argument provided");
                Console.WriteLine(Environment.NewLine);
                DisplayHelp();
                return;
            }

            if (args.Contains("-h") || args.Contains("--help"))
            {
                DisplayHelp();
            }
            
            if (args.Contains("-c"))
            {
                // Console.WriteLine("-o &emsp; --input &emsp; Input directory &emsp; \"Messages\" directory of your Facebook data");
            }            
            
            DisplayHelp();
        }

        private static void DisplayHelp()
        {
            Console.WriteLine("Application will count messages in provided conversation from your data downloaded from Facebook. ");
            Console.WriteLine("All counted statistics will be provided in output JSON file.");
            Console.WriteLine("Please provide it with at least conversation name, according to list of arguments below.");
            Console.WriteLine(Environment.NewLine);
            Console.WriteLine("Arguments:");
            Console.WriteLine("-i\t--input\tInput directory &emsp; \"Messages\" directory of your Facebook data. If not provided, the app will use current location.");
            Console.WriteLine("-o\t--output\tPath to output prepared JSON file");
            Console.WriteLine("-c\t--conversation\tName of conversation");
        }
    }
}