using System;
using System.Threading;
using System.Collections.Generic;
using Blackjack.Core.Abstractions;
using Blackjack.Core.Domain;
using Blackjack.Core.Players;

namespace Blackjack.Cli.Observers
{
    /*
     ConsoleGameObserver
     - A lightweight console-based implementation of IGameObserver used by the CLI.
     - Prints human-readable messages describing the progress of a blackjack round.
     - Optionally pauses after each event to allow step-by-step observation (controlled by _delayMs).
     - Keeps a short-lived buffer of initial dealt cards per player so initial two-card hands
       are printed only once both cards have been received for that player.
    */
    public sealed class ConsoleGameObserver : IGameObserver
    {
        private readonly int _delayMs;

        /*
         Initializes a new instance of ConsoleGameObserver.
         - delayMs: number of milliseconds to pause after each written event. Values below zero
           are clamped to zero to avoid passing an invalid value to Thread.Sleep.
        */
        public ConsoleGameObserver(int delayMs)
        {
            _delayMs = Math.Max(0, delayMs);
        }

        /*
         Called at the start of each round.
         - Clears the per-player initial-deal buffer so stale data from previous rounds is removed.
         - Clears the console and writes a short round header.
         - Pauses for the configured delay to give the user time to read the message.
        */
        public void OnRoundStarted()
        {
            _initialDealBuffer.Clear();
            Console.Clear();
            Console.WriteLine("=== New round starting ===");
            Pause();
        }

        /*
         Called when the dealer receives their initial two cards.
         - upCard: visible card shown to players.
         - holeCard: dealer's hidden (face-down) card; not revealed here.
         - Intentionally prints the hole card as "[hidden]" to match typical blackjack presentation.
        */
        public void OnDealerDealt(Card upCard, Card holeCard)
        {
            Console.WriteLine();
            Console.WriteLine("Dealer is dealt two cards:");
            Console.WriteLine($"- Up card: {upCard}");
            Console.WriteLine("- Hole card: [hidden]");
            Pause();
        }

        /*
         Called when a player is dealt a card.
         - Buffers the first two cards per player so we don't print a partial initial hand when
           the first initial card arrives; only prints once both initial cards are available.
         - Supports players with multiple hands (splits) by accepting the specific PlayerHand instance.
         - After printing the two-card initial hand, the method prints the computed hand value
           using hand.Hand.GetValue().
        */
        public void OnPlayerDealt(Player player, PlayerHand hand, Card card)
        {
            if (!_initialDealBuffer.TryGetValue(player, out List<Card>? cards))
            {
                cards = new List<Card>();
                _initialDealBuffer[player] = cards;
            }

            cards.Add(card);

            // Only print once we have both initial cards for this player.
            if (cards.Count < 2)
            {
                return;
            }

            string label = GetHandLabel(player, hand);

            Console.WriteLine();
            Console.WriteLine($"{player.Name} ({label}) is dealt:");
            Console.WriteLine($"- {cards[0]}");
            Console.WriteLine($"- {cards[1]}");
            Console.WriteLine($"Value: {hand.Hand.GetValue()}");

            Pause();
        }

        /*
         Called when a player makes a decision (e.g., "Hit", "Stand", "Double", "Split").
         - Prints the player's name, the hand label and the textual decision.
        */
        public void OnPlayerDecision(Player player, PlayerHand hand, string decision)
        {
            string label = GetHandLabel(player, hand);
            Console.WriteLine();
            Console.WriteLine($"{player.Name} ({label}) chooses: {decision}");
            Pause();
        }


        /*
         Called when a player draws an additional card after the initial deal.
         - Prints the drawn card for the provided player and hand.
        */
        public void OnPlayerCardDrawn(Player player, PlayerHand hand, Card card)
        {
            Console.WriteLine();
            Console.WriteLine($"{player.Name} draws: {card}");
            Pause();
        }

        /*
         Called when the dealer draws a card during dealer play.
         - Prints the drawn card and the dealer's current hand value.
        */
        public void OnDealerCardDrawn(Card card, int dealerValue)
        {
            Console.WriteLine();
            Console.WriteLine($"Dealer draws: {card} (value now: {dealerValue})");
            Pause();
        }


        /*
         Helper to produce a human-friendly label for a PlayerHand.
         - If the player has multiple hands (split), returns "Hand #N" where N is 1-based index.
         - Uses ReferenceEquals to match the exact PlayerHand instance.
         - Falls back to the generic "Hand" label if the hand is not found.
        */
        private static string GetHandLabel(Player player, PlayerHand hand)
        {
            for (int i = 0; i < player.Hands.Count; i++)
            {
                if (ReferenceEquals(player.Hands[i], hand))
                {
                    return $"Hand #{i + 1}";
                }
            }

            return "Hand";
        }

        /*
         Pauses execution for the configured delay.
         - Uses Thread.Sleep(_delayMs). If _delayMs is zero, this is a no-op.
         - Kept private because it's an implementation detail for the console observer only.
        */
        private void Pause()
        {
            Thread.Sleep(_delayMs);
        }

        /*
         Temporary buffer that collects the initial dealt cards per player within a round.
         - Key: Player instance
         - Value: list of cards received so far in the initial deal sequence for that player.
         - Cleared at the start of each round by OnRoundStarted to avoid leaking across rounds.
        */
        private readonly Dictionary<Player, List<Card>> _initialDealBuffer = new Dictionary<Player, List<Card>>();
    }
}
