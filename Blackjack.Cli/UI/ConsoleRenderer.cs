using System;
using Blackjack.Core.Domain;

namespace Blackjack.Cli.UI;

// UI Helper: Responsible for displaying game state in a visually pleasing and informative way.
/*
 ConsoleRenderer
 - Small, focused rendering helper for console-based UI.
 - Provides simple, synchronous helpers to display dealer and player hands and short result messages.
 - Keeps presentation logic separate from game logic so engine and domain classes remain UI-agnostic.
 - Designed for clarity rather than rich formatting; intended for a terminal/console environment.
*/
public static class ConsoleRenderer
{
    /*
     ShowHands
     - Prints the dealer's and player's hands to the console.
     - `player` and `dealer` are the Hand instances to display.
     - `hideDealerHoleCard` controls whether the dealer's second card (hole card) is shown.
       Typical blackjack UI hides the dealer hole card until the dealer's turn ends.
     - Uses ShowHand to render each hand consistently.
    */
    public static void ShowHands(Hand player, Hand dealer, bool hideDealerHoleCard)
    { 
        Console.WriteLine();
        Console.WriteLine("=== Dealer's Hand ===");
        ShowHand(dealer, hideDealerHoleCard);

        Console.WriteLine();
        Console.WriteLine("=== Player ===");
        ShowHand(player, hideDealerHoleCard: false);

        Console.WriteLine();
    }

    /*
     ShowHand
     - Renders the cards of a single hand.
     - If `hideDealerHoleCard` is true, the second card (index 1) is displayed as "[Hidden card]".
     - When hiding the hole card, the method prints only the visible card's raw value as "Value (visible)".
     - Otherwise, prints the computed hand value via Hand.GetValue().
     - Uses index-based iteration to preserve card order and to allow selective hiding.
    */
    private static void ShowHand(Hand hand, bool hideDealerHoleCard)
    {
        for (int i = 0; i < hand.Cards.Count; i++)
        {
            if(hideDealerHoleCard && i == 1)
            {
                Console.WriteLine("[Hidden card]");
                continue;
            }
            Console.WriteLine(hand.Cards[i].ToString());
        }

        if (hideDealerHoleCard)
        {
            // Only shows the value of the first visible card (Normal blackjack rules)
            if (hand.Cards.Count > 0)
            {
                Card first = hand.Cards[0];
                Console.WriteLine($"Value (visible): {first.Value}");
            }
        }
        else
        {
            Console.WriteLine($"Value: {hand.GetValue()}");
        }
    }

    /*
     ShowResult
     - Convenience method for printing a short, framed message to the console.
     - Used to display round outcomes, errors or informational messages in a consistent way.
    */
    public static void ShowResult(string message)
    {
        Console.WriteLine();
        Console.WriteLine(message);
        Console.WriteLine();
    }
}