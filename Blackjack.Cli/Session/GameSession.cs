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
    /*
     GameSession
     - Manages a short-lived play session containing one or more players.
     - Owns players for the lifetime of the session and preserves their bankroll across rounds.
     - Coordinates reading bets, creating a deck and game engine per round, running the round,
       applying payouts and showing a per-round summary to the CLI user.
     - Keeps the session loop simple and synchronous because it's a CLI-driven workflow.
    */
    public sealed class GameSession
    {
        private readonly IPayoutCalculator _payoutCalculator;
        private readonly List<Player> _players;

        /*
         Initializes a new GameSession.
         - payoutCalculator: used to compute payouts when results are resolved.
         - players: the players participating in this session; must be non-null and contain at least one player.
         Throws ArgumentNullException for null args and ArgumentException if players is empty.
        */
        public GameSession(IPayoutCalculator payoutCalculator, List<Player> players)
        {
            _payoutCalculator = payoutCalculator ?? throw new ArgumentNullException(nameof(payoutCalculator));
            _players = players ?? throw new ArgumentNullException(nameof(players));

            if (_players.Count == 0)
            {
                throw new ArgumentException("At least one player is required.", nameof(players));
            }
        }

        /*
         Runs the interactive session loop.
         - Clears console and shows balances at the start of each iteration.
         - Stops the session if no player has a positive bankroll.
         - Prompts for bets (human players via ConsoleHumanStrategy are asked, bots use a simple rule).
         - Creates a new shuffled deck and GameEngine per round, executes the round and dealer play,
           resolves results and applies payouts.
         - Displays a round summary and asks whether to continue or exit.
         - This method blocks and drives the CLI flow until the user exits or all bankrolls are exhausted.
        */
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

        /*
         Iterates players and asks each active player to place a bet.
         - Skips players with zero or negative balance.
         - Human players (detected by ConsoleHumanStrategy) are prompted via ConsoleInput.
         - Bot players use ChooseBotBet().
         - Each player starts a new round by calling StartNewRoundWithBet.
        */
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

        /*
         Prompt helper for human players to enter a bet.
         - Limits input between 1 and the player's current bankroll balance.
         - Uses ConsoleInput.ReadIntInRange to validate input.
        */
        private static int ReadHumanBet(Player player)
        {
            int max = player.Bankroll.Balance;

            Console.WriteLine();
            Console.WriteLine($"Your balance: {max}");
            int bet = ConsoleInput.ReadIntInRange("Place your bet: ", 1, max);

            return bet;
        }

        /*
         Simple bot betting strategy.
         - Bets 10% of the bankroll, with a minimum bet of 1.
         - This rule is intentionally simple for demo/testing purposes.
        */
        private static int ChooseBotBet(Player player)
        {
            // Simple bot bet rule: bet 10% of bankroll, at least 1.
            int bet = Math.Max(1, player.Bankroll.Balance / 10);
            return bet;
        }

        /*
         Displays a detailed round summary:
         - Prints dealer's cards and value.
         - For each player, prints player name, updated balance and each hand's cards, value and result.
         - Looks up the result for each hand using FindResult.
         - Waits for a key press before returning to the session loop.
        */
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

        /*
         Finds the RoundResult associated with a specific player's hand by comparing
         the Player and PlayerHand references stored in the result keys.
         - Returns RoundResult.Push if no matching entry is found (acts as a safe default).
         - Uses ReferenceEquals to ensure we match the exact instances used by the engine.
        */
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

        /*
         Writes the current session balances for all players to the console.
         - Simple helper used at the start of each round iteration to inform the user.
        */
        private void ShowBalances()
        {
            Console.WriteLine("=== Session Balances ===");
            foreach (Player player in _players)
            {
                Console.WriteLine($"{player.Name}: {player.Bankroll.Balance}");
            }
            Console.WriteLine();
        }

        /*
         Determines whether any player in the session can continue playing.
         - A player can continue when their Bankroll.Balance is greater than zero.
         - Returns true if at least one player has a positive balance; otherwise false.
         - Used to exit the session gracefully when all players are broke.
        */
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
