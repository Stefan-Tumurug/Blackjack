using System;
using System.Threading;
using System.Collections.Generic;
using Blackjack.Core.Abstractions;
using Blackjack.Core.Domain;
using Blackjack.Core.Players;

namespace Blackjack.Cli.Observers
{
    // Simple CLI observer to visualize game flow step-by-step.
    public sealed class ConsoleGameObserver : IGameObserver
    {
        private readonly int _delayMs;

        public ConsoleGameObserver(int delayMs)
        {
            _delayMs = Math.Max(0, delayMs);
        }

        public void OnRoundStarted()
        {
            _initialDealBuffer.Clear();
            Console.Clear();
            Console.WriteLine("=== New round starting ===");
            Pause();
        }

        public void OnDealerDealt(Card upCard, Card holeCard)
        {
            Console.WriteLine();
            Console.WriteLine("Dealer is dealt two cards:");
            Console.WriteLine($"- Up card: {upCard}");
            Console.WriteLine("- Hole card: [hidden]");
            Pause();
        }

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

        public void OnPlayerDecision(Player player, PlayerHand hand, string decision)
        {
            string label = GetHandLabel(player, hand);
            Console.WriteLine();
            Console.WriteLine($"{player.Name} ({label}) chooses: {decision}");
            Pause();
        }


        public void OnPlayerCardDrawn(Player player, PlayerHand hand, Card card)
        {
            Console.WriteLine();
            Console.WriteLine($"{player.Name} draws: {card}");
            Pause();
        }

        public void OnDealerCardDrawn(Card card, int dealerValue)
        {
            Console.WriteLine();
            Console.WriteLine($"Dealer draws: {card} (value now: {dealerValue})");
            Pause();
        }


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

        private void Pause()
        {
            Thread.Sleep(_delayMs);
        }
        private readonly Dictionary<Player, List<Card>> _initialDealBuffer = new Dictionary<Player, List<Card>>();
    }
}
