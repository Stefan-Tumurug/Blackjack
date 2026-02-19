using System;
using System.Collections.Generic;
using Blackjack.Core.Abstractions;
using Blackjack.Core.Betting;
using Blackjack.Core.Domain;
using Blackjack.Core.Game;
using Blackjack.Core.Players;
using Blackjack.Core.Players.Strategies;
using Blackjack.Cli.UI;
using Blackjack.Cli.Strategies;

namespace Blackjack.Cli.Session
{
    // Owns the list of players and keeps bankroll alive for the whole session.
    public sealed class GameSession
    {
        private readonly IPayoutCalculator _payoutCalculator;
        private readonly List<Player> _players;

        public GameSession(IPayoutCalculator payoutCalculator, List<Player> players)
        {
            _payoutCalculator = payoutCalculator ?? throw new ArgumentNullException(nameof(payoutCalculator));
            _players = players ?? throw new ArgumentNullException(nameof(players));

            if (_players.Count == 0)
            {
                throw new ArgumentException("At least one player is required.", nameof(players));
            }
        }

        public void Run()
        {
            while (true)
            {
                Console.Clear();
                ShowBalances();

                if (!CanAnyPlayerContinue())
                {
                    ConsoleRenderer.ShowResult("No players have money left. Session ended.");
                    return;
                }

                SetupBets();

                IDeck deck = new Deck(); // New shuffled deck per round (simple and stable).
                GameEngine engine = new GameEngine(deck, _payoutCalculator, _players);

                engine.StartRound();
                engine.PlayPlayers();
                engine.DealerPlay();

                IReadOnlyList<(PlayerHandKey Key, RoundResult Result)> results = engine.ResolveResults();
                engine.ApplyPayouts(results);

                ShowRoundSummary(engine, results);

                int again = ConsoleInput.ReadMenuChoice("Next round? 1 = Yes, 0 = Exit: ", 1, 0);
                if (again == 0)
                {
                    return;
                }
            }
        }

        private void SetupBets()
        {
            foreach (Player player in _players)
            {
                if (player.Bankroll.Balance <= 0)
                {
                    continue;
                }

                int betAmount = player.Strategy is ConsoleHumanStrategy
                    ? ReadHumanBet(player)
                    : ChooseBotBet(player);

                player.StartNewRoundWithBet(new Bet(betAmount));
            }
        }

        private static int ReadHumanBet(Player player)
        {
            int max = player.Bankroll.Balance;

            Console.WriteLine();
            Console.WriteLine($"Your balance: {max}");
            int bet = ConsoleInput.ReadIntInRange("Place your bet: ", 1, max);

            return bet;
        }

        private static int ChooseBotBet(Player player)
        {
            // Simple bot bet rule: bet 10% of bankroll, at least 1.
            int bet = Math.Max(1, player.Bankroll.Balance / 10);
            return bet;
        }

        private void ShowRoundSummary(
            GameEngine engine,
            IReadOnlyList<(PlayerHandKey Key, RoundResult Result)> results)
        {
            Console.Clear();
            Console.WriteLine("=== Round Summary ===");
            Console.WriteLine();

            Console.WriteLine("Dealer hand:");
            foreach (Card card in engine.DealerHand.Cards)
            {
                Console.WriteLine($" - {card}");
            }
            Console.WriteLine($"Dealer value: {engine.DealerHand.GetValue()}");
            Console.WriteLine();

            foreach (Player player in _players)
            {
                Console.WriteLine($"Player: {player.Name} (Balance: {player.Bankroll.Balance})");

                for (int i = 0; i < player.Hands.Count; i++)
                {
                    PlayerHand hand = player.Hands[i];
                    RoundResult result = FindResult(results, player, hand);

                    Console.WriteLine($"  Hand #{i + 1} (Bet: {hand.Bet.Amount}, Doubled: {hand.State.HasDoubledDown})");
                    foreach (Card card in hand.Hand.Cards)
                    {
                        Console.WriteLine($"   - {card}");
                    }
                    Console.WriteLine($"   Value: {hand.Hand.GetValue()}");
                    Console.WriteLine($"   Result: {result}");
                    Console.WriteLine();
                }

                Console.WriteLine();
            }

            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
        }

        private static RoundResult FindResult(
            IReadOnlyList<(PlayerHandKey Key, RoundResult Result)> results,
            Player player,
            PlayerHand hand)
        {
            foreach ((PlayerHandKey key, RoundResult result) in results)
            {
                if (ReferenceEquals(key.Player, player) && ReferenceEquals(key.Hand, hand))
                {
                    return result;
                }
            }

            return RoundResult.Push;
        }

        private void ShowBalances()
        {
            Console.WriteLine("=== Session Balances ===");
            foreach (Player player in _players)
            {
                Console.WriteLine($"{player.Name}: {player.Bankroll.Balance}");
            }
            Console.WriteLine();
        }

        // Determines whether any player in the session can continue playing.
        // Iterates the session's player list and checks each player's Bankroll.Balance.
        // A player can continue only when their balance is greater than zero.
        // Returns true if at least one player has a positive balance; otherwise false.
        private bool CanAnyPlayerContinue()
        {
            foreach (Player player in _players)
            {
                if (player.Bankroll.Balance > 0)
                {
                    return true;
                }
            }

            return false;
        }
    }
}
