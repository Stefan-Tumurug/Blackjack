using System;

namespace Blackjack.Cli.Session
{
    /*
     TutorialSession
     - Displays a simple, interactive tutorial on the console before gameplay begins.
     - Explains core blackjack rules and project-specific behaviors (dealer rules, actions, betting, bots).
     - Intentionally synchronous and blocking because it is shown as a carriage-returned help screen
       that the user dismisses with a key press.
    */
    public sealed class TutorialSession
    {
        /*
         Run
         - Clears the console and writes a brief, human-friendly tutorial covering:
           * Goal and card values
           * Dealer drawing rules
           * Available player actions (Hit, Stand, Double Down, Split)
           * Betting behavior and persistence of balance between rounds
           * Bot behavior and per-round results
         - Waits for a key press before returning to the caller (typically the main menu).
         - Designed for readability rather than exhaustive rule coverage; keep content concise.
        */
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
