namespace Blackjack.Core.Domain;

// Suits in a standard 52-card deck.
public enum Suit
{
    Hearts,
    Diamonds,
    Clubs,
    Spades
}

// Rank is the card's name. We also store the standard Blackjack points.
// Ace is represented as 11 here, but can be treated as 1 in hand calculation to avoid a bust.
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
public sealed class Card
{
    public Suit Suit { get; }
    public Rank Rank { get; }

    public Card(Suit suit, Rank rank)
    {
        Suit = suit;
        Rank = rank;
    }

    // Standard Blackjack value (Ace may be adjusted in Hand).
    public int Value => (int)Rank;

    public bool IsAce => Rank == Rank.Ace;

    // Helpful for UI display and debugging.
    public override string ToString()
    {
        return $"{Rank} of {Suit}";
    }
}
