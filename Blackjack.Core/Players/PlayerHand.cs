using System;
using Blackjack.Core.Betting;
using Blackjack.Core.Domain;

namespace Blackjack.Core.Players
{
    // Represents a single playable hand for a player.
    // Split creates multiple PlayerHand instances for the same Player.
    public sealed class PlayerHand
    {
        public Hand Hand { get; }
        public Bet Bet { get; private set; }
        public PlayerHandState State { get; }

        public PlayerHand(Bet bet)
        {
            Bet = bet ?? throw new ArgumentNullException(nameof(bet));
            Hand = new Hand();
            State = new PlayerHandState();
        }

        public void Reset(Bet bet)
        {
            // Resets this hand for a new round.
            Bet = bet ?? throw new ArgumentNullException(nameof(bet));
            Hand.Clear();
            State.Reset();
        }
    }
}
