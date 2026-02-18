using System;
using Blackjack.Core.Players;

namespace Blackjack.Core.Game
{
    // Key object used when returning results per player hand.
    public sealed class PlayerHandKey
    {
        public Player Player { get; }
        public PlayerHand Hand { get; }

        public PlayerHandKey(Player player, PlayerHand hand)
        {
            Player = player ?? throw new ArgumentNullException(nameof(player));
            Hand = hand ?? throw new ArgumentNullException(nameof(hand));
        }
    }
}
