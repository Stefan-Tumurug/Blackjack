using System;
using System.Collections.Generic;
using Blackjack.Core.Abstractions;
using Blackjack.Core.Betting;

namespace Blackjack.Core.Players
{
    // Represents a participant (human or bot) in the game.
    /*
     Player
     - Encapsulates a player's identity, bankroll and decision-making strategy.
     - Holds one or more PlayerHand instances for the current round (splits create multiple hands).
     - Designed to be UI-agnostic: the strategy is provided as an IPlayerStrategy implementation.
     - Responsibilities:
       * Validate constructor inputs and enforce basic invariants.
       * Provide a convenience method to start a new round with a given bet.
     - Gotchas:
       * The class does not manage persistence of bankroll across sessions — callers must reset
         or reuse the Bankroll instance as appropriate.
       * Hands are mutable and reused per round; call StartNewRoundWithBet to clear prior hands.
    */
    public sealed class Player
    {
        // Human-friendly name for display and logging.
        public string Name { get; }

        // Encapsulates the player's available money and related operations.
        public Bankroll Bankroll { get; }

        // Decision maker (human UI or automated bot). Injected to keep engine UI-agnostic.
        public IPlayerStrategy Strategy { get; }

        // Active hands for the current round. Starts with one hand and may grow when splitting.
        public List<PlayerHand> Hands { get; }

        /*
         Constructor
         - name: non-empty label for the player.
         - bankroll: must be non-null and represents the player's money (balance must be >= 0).
         - strategy: non-null IPlayerStrategy implementation (human or bot).
         - Throws on invalid arguments to ensure Player instances are always valid.
        */
        public Player(string name, Bankroll bankroll, IPlayerStrategy strategy)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException("Name is required.", nameof(name));
            }

            Name = name;
            Bankroll = bankroll ?? throw new ArgumentNullException(nameof(bankroll));
            Strategy = strategy ?? throw new ArgumentNullException(nameof(strategy));

            // Initialize hands collection; StartNewRoundWithBet will populate it per round.
            Hands = new List<PlayerHand>();
        }

        /*
         StartNewRoundWithBet
         - Prepares the player for a new round by validating and applying the provided Bet:
           * Verifies bet is not null.
           * Uses Bankroll.CanPlaceBet to ensure the bet amount is allowed for the current balance.
         - Side effects:
           * Clears any existing hands and creates a single PlayerHand seeded with the bet.
         - Throws:
           * ArgumentNullException when bet is null.
           * InvalidOperationException when the bankroll cannot place the requested bet.
         - Note:
           * This method intentionally does not deduct the bet from the bankroll immediately.
           * Settlement (wins/losses) is applied later through the payout calculator and Bankroll.ApplyNetChange.
        */
        public void StartNewRoundWithBet(Bet bet)
        {
            if (bet == null)
            {
                throw new ArgumentNullException(nameof(bet));
            }

            if (!Bankroll.CanPlaceBet(bet.Amount))
            {
                throw new InvalidOperationException("Bet is not allowed for this bankroll.");
            }

            // Ensure exactly one active hand at round start (splits will add more hands later).
            Hands.Clear();
            Hands.Add(new PlayerHand(bet));
        }
    }
}
