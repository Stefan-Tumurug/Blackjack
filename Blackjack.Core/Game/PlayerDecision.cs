namespace Blackjack.Core.Game
{
    /*
     PlayerDecision
     - Enumerates the discrete actions a player's strategy can return to the GameEngine.
     - Integer values are chosen to match the CLI menu choices used elsewhere (1..4).
     - Semantics:
       * Hit        - Take another card.
       * Stand      - Stop taking cards and finish the hand.
       * DoubleDown - Double the bet, take exactly one additional card, then stand.
       * Split      - When the first two cards are the same rank, split into two hands (if allowed).
     - Note: The GameEngine performs validation (e.g., CanDoubleDown/CanSplit) and will ignore or
       override invalid decisions from a strategy to keep the engine robust.
    */
    public enum PlayerDecision
    {
        Hit = 1,
        Stand = 2,
        DoubleDown = 3,
        Split = 4
    }
}
