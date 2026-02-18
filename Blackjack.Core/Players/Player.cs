using System;
using System.Collections.Generic;
using Blackjack.Core.Abstractions;
using Blackjack.Core.Betting;

namespace Blackjack.Core.Players
{
    // Represents a participant (human or bot).
    // Holds bankroll and one or more hands for the current round.
    public sealed class Player
    {
        public string Name { get; }
        public Bankroll Bankroll { get; }

        // The decision maker (bot/human) is injected to keep Player UI-agnostic.
        public IPlayerStrategy Strategy { get; }

        // Each player can have 1+ hands (after split).
        public List<PlayerHand> Hands { get; }

        public Player(string name, Bankroll bankroll, IPlayerStrategy strategy)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException("Name is required.", nameof(name));
            }

            Name = name;
            Bankroll = bankroll ?? throw new ArgumentNullException(nameof(bankroll));
            Strategy = strategy ?? throw new ArgumentNullException(nameof(strategy));

            Hands = new List<PlayerHand>();
        }

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

            // Ensure exactly one active hand at round start.
            Hands.Clear();
            Hands.Add(new PlayerHand(bet));
        }
    }
}
