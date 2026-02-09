using Blackjack.Core.Domain;

// Hurtig sanity test: A, A, 9 skal give 21 (ikke 31).
Hand hand = new Hand();
hand.AddCard(new Card(Suit.Spades, Rank.Ace));
hand.AddCard(new Card(Suit.Hearts, Rank.Ace));
hand.AddCard(new Card(Suit.Clubs, Rank.Nine));

Console.WriteLine(string.Join(", ", hand.Cards));
Console.WriteLine($"Value: {hand.GetValue()} (Bust: {hand.IsBust})");
