using System;

namespace Blackjack.Core.Domain;

// Suits in a standard 52-card deck.
/*
 Suit
 - Enumerates the four suits used in a standard deck.
 - Kept simple and explicit to make tests and display code straightforward.
*/
public enum Suit
{
    Hearts,
    Diamonds,
    Clubs,
    Spades
}

// Rank is the card's name. We also store the standard Blackjack points.
// Ace is represented as 11 here, but can be treated as 1 in hand calculation to avoid a bust:
/*
 Rank
 - Encodes both the face/rank and the conventional Blackjack point value for that rank.
 - Face cards (Jack, Queen, King) are represented with value 10.
 - Ace is stored as 11; Hand logic is responsible for treating Aces as 1 when necessary.
*/
public enum Rank
{
    Two = 2,
    Three = 3,
    Four = 4,
    Five = 5,
    Six = 6,
    Seven = 7,
    Eight = 8,
    Nine = 9,
    Ten = 10,
    Jack = 10,
    Queen = 10,
    King = 10,
    Ace = 11
}

// Immutable card object: once a card is created it cannot be changed.
// This makes the program easier to test and reason about, since a card's value and suit are fixed.
/*
 Card
 - Immutable value object representing a single playing card used by the engine and UI.
 - Responsibilities:
     * Expose Suit and Rank.
     * Provide a standard Blackjack value via the `Value` property (Ace as 11).
     * Offer an `IsAce` helper to simplify hand-evaluation logic.
     * Provide a readable `ToString()` for UI and test output.
 - Note: Hand-level logic (in `Hand`) handles Ace adjustments to prevent busts.
*/
public sealed class Card
{
    // The card's suit (Hearts, Diamonds, Clubs, Spades).
    public Suit Suit { get; }

    // The card's rank (Two..Ace). The enum also encodes the default Blackjack value.
    public Rank Rank { get; }

    public Card(Suit suit, Rank rank)
    {
        Suit = suit;
        Rank = rank;
    }

    // Standard Blackjack value (Ace may be adjusted in Hand).
    // Returns the integer value associated with the Rank enum.
    public int Value => (int)Rank;

    // Convenience flag used by hand evaluation logic to detect aces.
    public bool IsAce => Rank == Rank.Ace;

    // Helpful for UI display and debugging.
    public override string ToString()
    {
        return $"{Rank} of {Suit}";
    }
}
