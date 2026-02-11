using System;
using Blackjack.Core.Domain;

namespace Blackjack.Cli.UI;

// UI Helper: Responsible for displaying game state in a visually pleasing and informative way.
public static class ConsoleRenderer
{
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

    public static void ShowResult(string message)
    {
        Console.WriteLine();
        Console.WriteLine(message);
        Console.WriteLine();
    }
}