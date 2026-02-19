using System;

namespace Blackjack.Cli.Session
{
    // Simple tutorial shown before the game starts.
    // Explains rules and project-specific features.
    public sealed class TutorialSession
    {
        public void Run()
        {
            Console.Clear();

            Console.WriteLine("=== Blackjack Tutorial ===");
            Console.WriteLine();

            Console.WriteLine("Goal:");
            Console.WriteLine("Get as close to 21 as possible without going over.");
            Console.WriteLine();

            Console.WriteLine("Card values:");
            Console.WriteLine("- Cards 2–10: face value");
            Console.WriteLine("- Face cards (J, Q, K): 10");
            Console.WriteLine("- Ace: 1 or 11 (automatically chosen)");
            Console.WriteLine();

            Console.WriteLine("Dealer rules:");
            Console.WriteLine("- Dealer draws until at least 17");
            Console.WriteLine();

            Console.WriteLine("Your actions:");
            Console.WriteLine("- Hit: take another card");
            Console.WriteLine("- Stand: stop your turn");
            Console.WriteLine("- Double Down: double your bet, receive one card, then stand");
            Console.WriteLine("- Split: if your first two cards match, split into two hands");
            Console.WriteLine();

            Console.WriteLine("Betting:");
            Console.WriteLine("- You place a bet each round");
            Console.WriteLine("- Your balance carries over between rounds");
            Console.WriteLine();

            Console.WriteLine("Bots:");
            Console.WriteLine("- Other players use automated strategies");
            Console.WriteLine("- Each bot bets and plays independently");
            Console.WriteLine();

            Console.WriteLine("After each round:");
            Console.WriteLine("- Results are shown per hand");
            Console.WriteLine("- Your updated balance is displayed");
            Console.WriteLine();

            Console.WriteLine("Press any key to return to the main menu...");
            Console.ReadKey();
        }
    }
}
