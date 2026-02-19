using System;

namespace Blackjack.Cli.UI
{
    // UI helper: keeps input-validation away from Program.cs, making it easier to test
    // and reuse console-related logic separately from application flow.
    /*
     ConsoleInput
     - Provides synchronous, blocking helpers to read validated user input from the console.
     - Centralizes validation and retry loops so calling code can assume valid values.
     - All methods block until valid input is provided; designed for CLI usage where
       prompting the user repeatedly is acceptable.
    */
    public static class ConsoleInput
    {
        /*
         ReadRequiredString
         - Prompts the user until a non-empty, non-whitespace string is entered.
         - Trims and returns the entered value.
         - Prints an explanatory message when the input is null or empty and repeats the prompt.
        */
        public static string ReadRequiredString(string prompt)
        {
            while (true)
            {
                Console.Write(prompt);
                string? input = Console.ReadLine();

                if (!string.IsNullOrWhiteSpace(input))

                    return input.Trim();

                Console.WriteLine(input == null ? "Input cannot be null. Please try again." : "Input cannot be empty. Please try again.");
            }
        }

        // Assists in input menu choices, for example 1/2/0
        /*
         ReadMenuChoice
         - Repeatedly prompts the user using `ReadRequiredString` and parses the input as an integer.
         - Accepts only values contained in the `allowed` parameter list and returns the chosen value.
         - Prints a helpful error message listing allowed options when validation fails.
        */
        public static int ReadMenuChoice(string prompt, params int[] allowed)
        {
            while (true)
            {
                string input = ReadRequiredString(prompt);

                if (int.TryParse(input, out int choice) && IsAllowed(choice, allowed))
                    return choice;

                Console.WriteLine($"Invalid choice. Please enter one of the following: {string.Join(", ", allowed)}");
            }
        }

        /*
         ReadIntInRange
         - Prompts the user until a valid integer within the inclusive range [min, max] is entered.
         - Uses Console.ReadLine() directly for simplicity and prints an error message for invalid input.
         - Common use: reading a bet amount constrained by a player's bankroll.
        */
        public static int ReadIntInRange(string prompt, int min, int max)
        {
            while (true)
            {
                System.Console.Write(prompt);
                string? input = System.Console.ReadLine();

                if (int.TryParse(input, out int value) && value >= min && value <= max)
                {
                    return value;
                }

                System.Console.WriteLine($"Please enter a number between {min} and {max}.");
            }
        }

        /*
         IsAllowed
         - Small helper to check whether a given integer is present in the allowed array.
         - Implemented with a simple loop for clarity and predictable behavior.
        */
        private static bool IsAllowed(int value, int[] allowed)
        {
            foreach (int a in allowed)
            {
                if (value == a) return true;
            }
            return false;

        }
    }
}
