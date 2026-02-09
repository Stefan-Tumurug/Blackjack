using Blackjack.Core.Domain;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Blackjack.Tests.Domain;

[TestClass]
public sealed class HandValueTests
{
    [TestMethod]
    public void GetValue_TwoAcesAndNine_Returns21()
    {
        // Arrange
        Hand hand = new Hand();
        hand.AddCard(new Card(Suit.Spades, Rank.Ace));
        hand.AddCard(new Card(Suit.Hearts, Rank.Ace));
        hand.AddCard(new Card(Suit.Clubs, Rank.Nine));

        // Act
        int value = hand.GetValue();

        // Assert
        Assert.AreEqual(21, value);
        Assert.IsFalse(hand.IsBust);
    }

    [TestMethod]
    public void GetValue_AceKingNine_AceCountsAsOne_Returns20()
    {
        // Arrange
        Hand hand = new Hand();
        hand.AddCard(new Card(Suit.Spades, Rank.Ace));
        hand.AddCard(new Card(Suit.Hearts, Rank.King));
        hand.AddCard(new Card(Suit.Clubs, Rank.Nine));

        // Act
        int value = hand.GetValue();

        // Assert
        Assert.AreEqual(20, value);
        Assert.IsFalse(hand.IsBust);
    }

    [TestMethod]
    public void IsBust_ValueOver21_ReturnsTrue()
    {
        // Arrange
        Hand hand = new Hand();
        hand.AddCard(new Card(Suit.Spades, Rank.Ten));
        hand.AddCard(new Card(Suit.Hearts, Rank.Nine));
        hand.AddCard(new Card(Suit.Clubs, Rank.Five));

        // Act
        bool isBust = hand.IsBust;

        // Assert
        Assert.IsTrue(isBust);
    }
}
