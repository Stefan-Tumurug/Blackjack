namespace Blackjack.Core.Players
{
    // PlayerHandState
    // - Tracks transient, per-hand flags that influence play flow and settlement.
    // - Keeps these concerns separate from the card-centric `Hand` class so state
    //   like "has stood" or "has doubled down" is explicit and easy to reason about.
    // - Typical lifecycle:
    //     1. Created with a new PlayerHand.
    //     2. Mutated during play via `Stand()` or `MarkDoubledDown()`.
    //     3. Reset at the start of a new round via `Reset()`.
    // - Notes / gotchas:
    //     * `HasDoubledDown` should be considered by payout logic (it indicates the stake was doubled).
    //     * `HasStood` is used to indicate the hand should no longer receive hits.
    //     * This class is intentionally simple and not thread-safe — callers should use it only
    //       from the game loop thread.
    public sealed class PlayerHandState
    {
        // True when the hand has been instructed to stand (no further hits).
        public bool HasStood { get; private set; }

        // True when the hand has been doubled down (used by payout calculation).
        public bool HasDoubledDown { get; private set; }

        // Mark the hand as stood; the engine will stop taking actions on this hand.
        public void Stand()
        {
            HasStood = true;
        }

        // Mark the hand as doubled down; typically used so settlement logic can apply 2x bet.
        public void MarkDoubledDown()
        {
            HasDoubledDown = true;
        }

        // Reset both flags to their initial false state so the PlayerHandState can be reused.
        public void Reset()
        {
            HasStood = false;
            HasDoubledDown = false;
        }
    }
}
