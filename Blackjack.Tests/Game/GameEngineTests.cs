using Blackjack.Core.Domain;
using Blackjack.Core.Game;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Blackjack.Tests.Game;

[TestClass]
public sealed class GameEngineTests
{
    [TestMethod]
    public void DealerPlay_DealerDrawsUntilAtleast17()
    {
        // Arrange
        // StartRound trækker: P!, D1, P2, D2
        // Dealer får 10 + 6 = 16 først, skal trække 1 kort mere
        // Næste kort er et Es: 16 + 11 = 27, men es burde justeres til 1 -> 17.
        FakeDeck deck = new FakeDeck(new[]
        {
            new Card(Suit.Clubs, Rank.Two),   // P1
            new Card(Suit.Spades, Rank.Ten), // D1
            new Card(Suit.Hearts, Rank.Three), // P2
            new Card(Suit.Diamonds, Rank.Six), // D2
            new Card(Suit.Hearts, Rank.Ace)   // D3 - Dealer hit -> 17 (via es-logik)
        });

        GameEngine engine = new GameEngine(deck);
        engine.StartRound();

        // Act
        engine.DealerPlay();

        // Assert
        Assert.AreEqual(17, engine.DealerHand.GetValue());
        Assert.HasCount(3, engine.DealerHand.Cards);

    }

    [TestMethod]
    public void DetermineWinner_PlayerBusts_DealerWins()
    {
        // Arrange
        // Player: 10 + 9 = 19, hits and draws a King -> 29 (bust)
        FakeDeck deck = new FakeDeck(new[]
        {
            new Card(Suit.Spades, Rank.Ten),   // P1
            new Card(Suit.Clubs, Rank.Two), // D1
            new Card(Suit.Hearts, Rank.Nine), // P2
            new Card(Suit.Diamonds, Rank.Three), // D2
            new Card(Suit.Clubs, Rank.King)   // P3 - Player hits -> bust
        });

        GameEngine engine = new GameEngine(deck);
        engine.StartRound();

        // Act
        engine.PlayerHit(); // Player draws third card and busts
        RoundResult result = engine.DetermineWinner();

        // Assert
        Assert.IsTrue(engine.PlayerHand.IsBust);
        Assert.AreEqual(RoundResult.DealerWin, result);
    }

    [TestMethod]
    public void DetermineWinner_SameValue_ReturnsPush()
    {
        // Arrange
        // Player: 10 + 7 = 17
        // Dealer: 9 + 8 = 17
        FakeDeck deck = new FakeDeck(new[]
        {
            new Card(Suit.Spades, Rank.Ten),   // P1
            new Card(Suit.Clubs, Rank.Nine), // D1
            new Card(Suit.Hearts, Rank.Seven), // P2
            new Card(Suit.Diamonds, Rank.Eight) // D2
        });

        GameEngine engine = new GameEngine(deck);
        engine.StartRound();

        // Act
        RoundResult result = engine.DetermineWinner();

        // Assert
        Assert.AreEqual(17, engine.PlayerHand.GetValue());
        Assert.AreEqual(17, engine.DealerHand.GetValue());
        Assert.AreEqual(RoundResult.Push, result);
    }
}