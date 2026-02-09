using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blackjack.Core.Domain;

// Kulører i et standard kortspil (52 kort).
public enum Suit
{
    Hearts,
    Diamonds,
    Clubs,
    Spades
}

// Rank er kortets navn. Vi gemmer også standard-point for Blackjack.
// Es håndteres som 11 her, men kan blive 1 i Hånd-beregningen for at undgå bust.
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

// Immutale kortobjekt: Når et kort er oprettet, kan det ikke ændres.
// Det gør programmet enklere at teste og reasonere om, da kortets værdi og kulør er faste.
public sealed class Card
{
    public Suit Suit { get; }
    public Rank Rank { get; }

    public Card(Suit suit, Rank rank)
    {
        Suit = suit;
        Rank = rank;
    }

    // Standardværdi i Blackjack (Es justeres evt. i Hand).
    public int Value => (int)Rank;

    public bool IsAce => Rank == Rank.Ace;

    // Hjælper til senere UI-visning og debugging.
    public override string ToString()
    {
        return $"{Rank} of {Suit}";
    }
}
