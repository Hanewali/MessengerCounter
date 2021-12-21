using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace MessengerCounter
{
    static class Program
    {
        private static string? ConversationName { get; set; }
        private static string? InputPath { get; set; }
        private static string? OutputPath { get; set; }
        
        public static void Main(string[] args)
        {
            HandleArguments(args);

            if (!string.IsNullOrWhiteSpace(ConversationName))
            {
                var analyzer = Analyzer.CreateInstance(ConversationName, InputPath, OutputPath);

                analyzer.GetMessages();
                analyzer.Analyze();
                analyzer.PrettyPrint();
                
                return;
            }
            
            DisplayHelp();
        }

        private static void HandleArguments(string[] arguments)
        {
            try
            {
                if (arguments.Length == 0)
                {
                    Console.WriteLine("ERROR: No argument provided");
                    Console.WriteLine(Environment.NewLine);
                    DisplayHelp();
                    return;
                }

                if (arguments.Contains("-h") || arguments.Contains("--help"))
                {
                    DisplayHelp();
                }

                if (arguments.Contains("-o") || arguments.Contains("--output"))
                {
                    var argIndex = Array.FindIndex(arguments, x => Regex.IsMatch( x, "^(-o)|(--output)$"));
                    OutputPath = arguments[argIndex + 1];
                }
                
                if (arguments.Contains("-i") || arguments.Contains("--input"))
                {
                    var argIndex = Array.FindIndex(arguments, x => Regex.IsMatch( x, "^(-i)|(--input)$"));
                    InputPath = arguments[argIndex + 1];
                }
                
                if (arguments.Contains("-c") || arguments.Contains("--conversation"))
                {
                    var argIndex = Array.FindIndex(arguments, x => Regex.IsMatch( x, "^(-c)|(--conversation)$"));
                    ConversationName = arguments[argIndex + 1];
                }
            }
            catch(Exception ex)
            {
                if (!string.IsNullOrWhiteSpace(ex.Message))
                {   
                    Console.WriteLine(ex.Message);
                }

                throw;
            }
        }

        private static void DisplayHelp()
        {
            Console.WriteLine("Application will count messages in provided conversation from your data downloaded from Facebook. ");
            Console.WriteLine("All counted statistics will be provided in output JSON file.");
            Console.WriteLine("Please provide it with at least conversation name, according to list of arguments below.");
            Console.WriteLine(Environment.NewLine);
            Console.WriteLine("Arguments:");
            Console.WriteLine("-h \t --help \t Displays this message");
            Console.WriteLine("-i \t --input \t Input directory \t \"Messages\" directory of your Facebook data. If not provided, the app will use current location.");
            Console.WriteLine("-o \t --output \t Output directory \t Path to output prepared JSON file. If not provided, the app will use current location.");
            Console.WriteLine("-c \t --conversation \t Conversation name \t Name of conversation");
        }
    }
}