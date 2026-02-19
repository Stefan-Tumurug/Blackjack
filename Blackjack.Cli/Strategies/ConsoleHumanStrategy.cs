using Blackjack.Cli.UI;
using Blackjack.Core.Abstractions;
using Blackjack.Core.Domain;
using Blackjack.Core.Game;
using Blackjack.Core.Players;
using System;

namespace Blackjack.Cli.Strategies
{
    // Human decision maker used by the CLI.
    /*
     ConsoleHumanStrategy
     - Implements IPlayerStrategy for an interactive human player.
     - Presents a clear console UI that shows the dealer's up card and the player's hand,
       then prompts the human to choose an action.
     - Keeps the GameEngine UI-agnostic by encapsulating console-specific logic inside this strategy.
    */
    public sealed class ConsoleHumanStrategy : IPlayerStrategy
    {
        private readonly string _playerName;

        /*
         Initializes a new ConsoleHumanStrategy.
         - playerName: label shown on the decision screen to identify the human player.
        */
        public ConsoleHumanStrategy(string playerName)
        {
            _playerName = playerName;
        }

        /*
         Decide
         - Entry point called by the engine to obtain a PlayerDecision for the given context.
         - Validates the context reference and displays a decision screen followed by a choice prompt.
         - Maps numeric menu choices to the corresponding PlayerDecision value.
         - Default fallback returns Stand if the returned choice is unexpected.
        */
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

        /*
         ShowDecisionScreen
         - Renders the minimal information a human needs to decide:
           * Player name
           * Dealer up card
           * The player's hand cards and current computed value
         - Uses Console.Clear to provide a focused screen per decision.
        */
        private void ShowDecisionScreen(PlayerDecisionContext context)
        {
            Console.Clear();
            Console.WriteLine("=== Blackjack ===");
            Console.WriteLine($"Player: {_playerName}");
            Console.WriteLine($"Dealer shows: {context.DealerUpCard}");
            Console.WriteLine();

            PlayerHand playerHand = context.PlayerHand;

            Console.WriteLine("Your hand:");
            foreach (Card card in playerHand.Hand.Cards)
            {
                Console.WriteLine($" - {card}");
            }

            Console.WriteLine($"Value: {playerHand.Hand.GetValue()}");
            Console.WriteLine();
        }

        /*
         ReadChoice
         - Builds the allowed menu options dynamically based on the context:
           CanDoubleDown and CanSplit control whether the corresponding options are shown.
         - Delegates input validation to ConsoleInput.ReadMenuChoice which ensures the user picks
           one of the allowed numeric options.
         - Returns the numeric choice which is later mapped to a PlayerDecision.
        */
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
