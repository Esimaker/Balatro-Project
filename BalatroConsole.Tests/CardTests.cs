using BalatroConsole.Models;

namespace BalatroConsole.Tests;

public class CardTests
{
    [Fact]
    public void NewCard_KeepsItsRankAndSuit()
    {
        Card card = new(Rank.Queen, Suit.Hearts);

        Assert.Equal(Rank.Queen, card.Rank);
        Assert.Equal(Suit.Hearts, card.Suit);
    }

    [Theory]
    [InlineData(Rank.Two, 2)]
    [InlineData(Rank.Seven, 7)]
    [InlineData(Rank.Ten, 10)]
    [InlineData(Rank.Jack, 10)]
    [InlineData(Rank.Queen, 10)]
    [InlineData(Rank.King, 10)]
    [InlineData(Rank.Ace, 11)]
    public void Points_FollowTheAssignmentRules(Rank rank, int expectedPoints)
    {
        Card card = new(rank, Suit.Clubs);

        Assert.Equal(expectedPoints, card.Points);
    }

    [Theory]
    [InlineData(Rank.Ace, Suit.Spades, "A♠")]
    [InlineData(Rank.Ten, Suit.Hearts, "10♥")]
    [InlineData(Rank.Two, Suit.Diamonds, "2♦")]
    [InlineData(Rank.King, Suit.Clubs, "K♣")]
    public void ToString_ShowsTheCardReadably(Rank rank, Suit suit, string expected)
    {
        Card card = new(rank, suit);

        Assert.Equal(expected, card.ToString());
    }

    [Fact]
    public void Card_CanBeUsedAsAnICard()
    {
        ICard card = new Card(Rank.Five, Suit.Spades);

        Assert.Equal(5, card.Points);
    }
}
