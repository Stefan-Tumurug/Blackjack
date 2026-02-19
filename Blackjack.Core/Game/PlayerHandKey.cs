using System;
using Blackjack.Core.Players;

namespace Blackjack.Core.Game
{
    /*
     PlayerHandKey
     - Lightweight key object used to identify a specific player's hand when returning or storing
       per-hand results (for example: ResolveResults() returns (PlayerHandKey, RoundResult) tuples).
     - Holds strong references to the Player and the exact PlayerHand instance. Other parts of the
       code use reference equality (ReferenceEquals) on these values to match results to hands.
     - Design notes / gotchas:
       * This type intentionally does not implement structural equality or hashing; it is a simple
         carrier of references. If you need dictionary keys or set semantics, consider adding
         appropriate Equals/GetHashCode implementations based on the desired identity rules.
       * The constructor enforces non-null arguments to keep instances valid.
    */
    public sealed class PlayerHandKey
    {
        // The player who owns the referenced hand.
        public Player Player { get; }

        // The specific PlayerHand instance this key refers to.
        public PlayerHand Hand { get; }

        /*
         PlayerHandKey(player, hand)
         - Creates a new key for a specific player's hand.
         - Throws ArgumentNullException when either argument is null to ensure callers always
           have valid references.
         - Consumers typically construct this right after resolving a hand outcome so the tuple
           (PlayerHandKey, RoundResult) can be returned to the caller.
        */
        public PlayerHandKey(Player player, PlayerHand hand)
        {
            Player = player ?? throw new ArgumentNullException(nameof(player));
            Hand = hand ?? throw new ArgumentNullException(nameof(hand));
        }
    }
}
