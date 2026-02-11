using System;

namespace Blackjack.Cli.UI
{
    // UI Helper: Keeps input-validation away from Program.cs, thus making it easier to test and use UI logic seperately.
    public static class ConsoleInput
    {
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

        // Assists in input menu choices, fore example 1/2/0
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
