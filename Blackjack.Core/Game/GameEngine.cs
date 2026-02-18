using System;
using System.Collections.Generic;
using Blackjack.Core.Abstractions;
using Blackjack.Core.Betting;
using Blackjack.Core.Domain;
using Blackjack.Core.Players;

namespace Blackjack.Core.Game
{
    // Coordinates one blackjack round for multiple players against a dealer.
    // Contains no UI logic and depends only on abstractions.
    public sealed class GameEngine
    {
        private readonly IDeck _deck;
        private readonly IPayoutCalculator _payoutCalculator;

        public IReadOnlyList<Player> Players { get; }
        public Hand DealerHand { get; }

        public GameEngine(
            IDeck deck,
            IPayoutCalculator payoutCalculator,
            IReadOnlyList<Player> players)
        {
            _deck = deck ?? throw new ArgumentNullException(nameof(deck));
            _payoutCalculator = payoutCalculator ?? throw new ArgumentNullException(nameof(payoutCalculator));
            Players = players ?? throw new ArgumentNullException(nameof(players));

            if (Players.Count == 0)
            {
                throw new ArgumentException("At least one player is required.", nameof(players));
            }

            DealerHand = new Hand();
        }
        public void StartRound()
        {
            DealerHand.Clear();

            foreach (Player player in Players)
            {
                if (player.CurrentBet == null)
                {
                    throw new InvalidOperationException($"Player {player.Name} has no bet.");
                }

                player.Hand.Clear();
                player.HandState.Reset();
            }

            DealInitialCards();
        }

        private void DealInitialCards()
        {
            foreach (Player player in Players)
            {
                player.Hand.AddCard(_deck.Draw());
                player.Hand.AddCard(_deck.Draw());
            }

            DealerHand.AddCard(_deck.Draw());
            DealerHand.AddCard(_deck.Draw());
        }
        public void PlayPlayers()
        {
            Card dealerUpCard = DealerHand.Cards[0];

            foreach (Player player in Players)
            {
                PlaySinglePlayer(player, dealerUpCard);
            }
        }

        private void PlaySinglePlayer(Player player, Card dealerUpCard)
        {
            while (true)
            {
                if (player.Hand.IsBust)
                {
                    return;
                }

                PlayerDecision decision = player.Strategy.Decide(
                    new PlayerDecisionContext(
                        player.Hand,
                        dealerUpCard,
                        canDoubleDown: CanDoubleDown(player),
                        canSplit: false));

                if (decision == PlayerDecision.Stand)
                {
                    player.HandState.Stand();
                    return;
                }

                if (decision == PlayerDecision.Hit)
                {
                    player.Hand.AddCard(_deck.Draw());
                    continue;
                }

                if (decision == PlayerDecision.DoubleDown && CanDoubleDown(player))
                {
                    player.Hand.AddCard(_deck.Draw());
                    player.HandState.MarkDoubledDown();
                    return;
                }

                return;
            }
        }

        private static bool CanDoubleDown(Player player)
        {
            return player.Hand.Cards.Count == 2 && !player.Hand.IsBust;
        }
        public void DealerPlay()
        {
            while (DealerHand.GetValue() < 17)
            {
                DealerHand.AddCard(_deck.Draw());
            }
        }

        public IReadOnlyDictionary<Player, RoundResult> ResolveResults()
        {
            Dictionary<Player, RoundResult> results = new();

            foreach (Player player in Players)
            {
                results[player] = DetermineWinner(player.Hand, DealerHand);
            }

            return results;
        }

        public void ApplyPayouts(IReadOnlyDictionary<Player, RoundResult> results)
        {
            foreach (KeyValuePair<Player, RoundResult> kvp in results)
            {
                Player player = kvp.Key;
                RoundResult result = kvp.Value;

                if (player.CurrentBet == null)
                {
                    throw new InvalidOperationException($"Player {player.Name} has no bet.");
                }

                int net = _payoutCalculator.CalculateNetChange(
                    player.CurrentBet.Amount,
                    result,
                    player.HandState.HasDoubledDown);

                player.Bankroll.ApplyNetChange(net);
            }
        }

        private static RoundResult DetermineWinner(Hand playerHand, Hand dealerHand)
        {
            if (playerHand.IsBust) return RoundResult.DealerWin;
            if (dealerHand.IsBust) return RoundResult.PlayerWin;

            int playerValue = playerHand.GetValue();
            int dealerValue = dealerHand.GetValue();

            if (playerValue > dealerValue) return RoundResult.PlayerWin;
            if (playerValue < dealerValue) return RoundResult.DealerWin;

            return RoundResult.Push;
        }
    }
}