using System;
using Blackjack.Core.Betting;
using Blackjack.Core.Domain;

namespace Blackjack.Core.Players
{
    // Represents a single playable hand for a player.
    // - A Player can own multiple PlayerHand instances (e.g. after a split).
    // - This type groups the domain `Hand` (cards), the `Bet` for the hand and the mutable
    //   `PlayerHandState` that tracks whether the hand has stood, doubled, etc.
    // - Design notes:
    //     * The class intentionally keeps simple semantics: construction requires a valid Bet,
    //       Hand and State are created internally and Reset allows reusing the instance between rounds.
    //     * Betting and bankroll enforcement are handled at higher levels (Player / GameEngine).
    public sealed class PlayerHand
    {
        // The hand containing the actual cards and value logic (Ace handling, bust detection).
        public Hand Hand { get; }

        // The bet associated with this hand. Updated via Reset when starting a new round.
        public Bet Bet { get; private set; }

        // Tracks transient per-hand state (stood, doubled down, etc.).
        public PlayerHandState State { get; }

        /*
         PlayerHand(bet)
         - Creates a new PlayerHand guarded by a non-null Bet.
         - Throws ArgumentNullException when bet is null to keep invariants simple.
         - Initializes an empty Hand and a fresh PlayerHandState.
         - Instances are reusable via Reset rather than allocating new objects repeatedly.
        */
        public PlayerHand(Bet bet)
        {
            Bet = bet ?? throw new ArgumentNullException(nameof(bet));
            Hand = new Hand();
            State = new PlayerHandState();
        }

        /*
         Reset(bet)
         - Prepares this PlayerHand for a new round.
         - Replaces the Bet with the provided one, clears any cards in Hand and resets State.
         - Throws ArgumentNullException when bet is null.
         - Purpose: avoid reallocating PlayerHand objects each round; callers (Player.StartNewRoundWithBet)
           call Reset/clear patterns or recreate hands as needed.
        */
        public void Reset(Bet bet)
        {
            // Validate input to preserve invariant: Bet must always be non-null and valid.
            Bet = bet ?? throw new ArgumentNullException(nameof(bet));
            Hand.Clear();
            State.Reset();
        }
    }
}
