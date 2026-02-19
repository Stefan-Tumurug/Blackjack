using System;
using Blackjack.Core.Abstractions;
using Blackjack.Core.Game;
using Blackjack.Core.Players;
using Blackjack.Cli.UI;

namespace Blackjack.Cli.Strategies
{
    // Human decision maker for the CLI.
    // GameEngine stays UI-agnostic by depending only on IPlayerStrategy.
    public sealed class ConsoleHumanStrategy : IPlayerStrategy
    {
        private readonly string _playerName;

        public ConsoleHumanStrategy(string playerName)
        {
            _playerName = playerName;
        }

        public PlayerDecision Decide(PlayerDecisionContext context)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));

            ShowDecisionScreen(context);

            int choice = ReadChoice(context);

            return choice switch
            {
                1 => PlayerDecision.Hit,
                2 => PlayerDecision.Stand,
                3 => PlayerDecision.DoubleDown,
                4 => PlayerDecision.Split,
                _ => PlayerDecision.Stand
            };
        }

        private void ShowDecisionScreen(PlayerDecisionContext context)
        {
            Console.Clear();
            Console.WriteLine("=== Blackjack ===");
            Console.WriteLine($"Player: {_playerName}");
            Console.WriteLine($"Dealer shows: {context.DealerUpCard}");
            Console.WriteLine();

            PlayerHand playerHand = context.PlayerHand;

            Console.WriteLine("Your hand:");
            foreach (var card in playerHand.Hand.Cards)
            {
                Console.WriteLine($" - {card}");
            }

            Console.WriteLine($"Value: {playerHand.Hand.GetValue()}");
            Console.WriteLine();
        }

        private static int ReadChoice(PlayerDecisionContext context)
        {
            // Build allowed choices dynamically.
            // 1 Hit, 2 Stand, 3 Double, 4 Split
            if (context.CanDoubleDown && context.CanSplit)
            {
                return ConsoleInput.ReadMenuChoice("Choose: (1) Hit, (2) Stand, (3) Double Down, (4) Split: ", 1, 2, 3, 4);
            }

            if (context.CanDoubleDown)
            {
                return ConsoleInput.ReadMenuChoice("Choose: (1) Hit, (2) Stand, (3) Double Down: ", 1, 2, 3);
            }

            if (context.CanSplit)
            {
                return ConsoleInput.ReadMenuChoice("Choose: (1) Hit, (2) Stand, (4) Split: ", 1, 2, 4);
            }

            return ConsoleInput.ReadMenuChoice("Choose: (1) Hit, (2) Stand: ", 1, 2);
        }
    }
}
