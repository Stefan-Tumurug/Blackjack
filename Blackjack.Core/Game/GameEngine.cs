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
                if (player.Hands.Count == 0)
                {
                    throw new InvalidOperationException($"Player {player.Name} has no hands for the round.");
                }
            }

            DealInitialCards();
        }

        private void DealInitialCards()
        {
            foreach (Player player in Players)
            {
                // At round start we only deal to the first hand.
                PlayerHand firstHand = player.Hands[0];

                firstHand.Hand.AddCard(_deck.Draw());
                firstHand.Hand.AddCard(_deck.Draw());
            }

            DealerHand.AddCard(_deck.Draw());
            DealerHand.AddCard(_deck.Draw());
        }

        public void PlayPlayers()
        {
            Card dealerUpCard = DealerHand.Cards[0];

            foreach (Player player in Players)
            {
                for (int handIndex = 0; handIndex < player.Hands.Count; handIndex++)
                {
                    PlayerHand playerHand = player.Hands[handIndex];
                    PlaySingleHand(player, playerHand, dealerUpCard);
                }
            }
        }

        public void DealerPlay()
        {
            // Dealer draws until at least 17 (standard rule).
            while (DealerHand.GetValue() < 17)
            {
                DealerHand.AddCard(_deck.Draw());
            }
        }

        private void PlaySingleHand(Player player, PlayerHand playerHand, Card dealerUpCard)
        {
            while (true)
            {
                if (playerHand.Hand.IsBust)
                {
                    return;
                }

                PlayerDecision decision = player.Strategy.Decide(
                    new PlayerDecisionContext(
                        playerHand,
                        dealerUpCard,
                        canDoubleDown: CanDoubleDown(playerHand),
                        canSplit: CanSplit(playerHand, player)));

                if (decision == PlayerDecision.Stand)
                {
                    playerHand.State.Stand();
                    return;
                }

                if (decision == PlayerDecision.Hit)
                {
                    playerHand.Hand.AddCard(_deck.Draw());
                    continue;
                }

                if (decision == PlayerDecision.DoubleDown && CanDoubleDown(playerHand))
                {
                    // Double down: exactly one card, then stand.
                    playerHand.Hand.AddCard(_deck.Draw());
                    playerHand.State.MarkDoubledDown();
                    return;
                }

                if (decision == PlayerDecision.Split && CanSplit(playerHand, player))
                {
                    PerformSplit(player, playerHand);
                    // After splitting, continue playing the current hand.
                    // The new hand will be played later by the outer loop.
                    continue;
                }

                // If strategy returns an invalid decision, default to stand.
                playerHand.State.Stand();
                return;
            }
        }

        private static bool CanDoubleDown(PlayerHand playerHand)
        {
            return playerHand.Hand.Cards.Count == 2 && !playerHand.State.HasDoubledDown;
        }

        private static bool CanSplit(PlayerHand playerHand, Player player)
        {
            // Minimal split rule:
            // - only allowed on exactly two cards
            // - ranks must match
            // - only one split per original player (no re-splitting)
            if (playerHand.Hand.Cards.Count != 2)
            {
                return false;
            }

            if (player.Hands.Count >= 2)
            {
                return false;
            }

            Card first = playerHand.Hand.Cards[0];
            Card second = playerHand.Hand.Cards[1];

            return first.Rank == second.Rank;
        }

        private void PerformSplit(Player player, PlayerHand originalHand)
        {
            // Splitting duplicates the bet for the new hand.
            // We keep bankroll checks simple: require bankroll can cover the same bet again.
            if (!player.Bankroll.CanPlaceBet(originalHand.Bet.Amount))
            {
                // Not enough money to split. Strategy asked for it, but we reject it.
                return;
            }

            // Create a new hand with the same bet.
            PlayerHand splitHand = new PlayerHand(originalHand.Bet);

            // Move one card from the original hand to the split hand.
            Card movedCard = originalHand.Hand.Cards[1];

            // Rebuild original hand: keep first card only.
            // We don't expose mutation of Cards, so we reset and re-add.
            Card firstCard = originalHand.Hand.Cards[0];

            originalHand.Hand.Clear();
            originalHand.Hand.AddCard(firstCard);

            splitHand.Hand.AddCard(movedCard);

            // Deal one new card to each hand after splitting.
            originalHand.Hand.AddCard(_deck.Draw());
            splitHand.Hand.AddCard(_deck.Draw());

            player.Hands.Add(splitHand);
        }
        public IReadOnlyList<(PlayerHandKey Key, RoundResult Result)> ResolveResults()
        {
            List<(PlayerHandKey, RoundResult)> results = new();

            foreach (Player player in Players)
            {
                foreach (PlayerHand hand in player.Hands)
                {
                    RoundResult result = DetermineWinner(hand.Hand, DealerHand);
                    results.Add((new PlayerHandKey(player, hand), result));
                }
            }

            return results;
        }
        public void ApplyPayouts(IReadOnlyList<(PlayerHandKey Key, RoundResult Result)> results)
        {
            foreach ((PlayerHandKey key, RoundResult result) in results)
            {
                int net = _payoutCalculator.CalculateNetChange(
                    key.Hand.Bet.Amount,
                    result,
                    key.Hand.State.HasDoubledDown);

                key.Player.Bankroll.ApplyNetChange(net);
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