using BalatroConsole.Logic.Combinations;
using BalatroConsole.Models;

namespace BalatroConsole.Tests;

public class FlushTests
{
    private readonly Flush _flush = new();

    [Fact]
    public void FiveOfTheSameSuit_Matches_AndAllFiveScore()
    {
        List<ICard> cards = TestHands.Of(
            (Rank.Two, Suit.Hearts), (Rank.Five, Suit.Hearts), (Rank.Nine, Suit.Hearts),
            (Rank.Jack, Suit.Hearts), (Rank.King, Suit.Hearts));

        Assert.True(_flush.Matches(cards));
        Assert.Equal(5, _flush.GetScoringCards(cards).Count);
    }

    [Fact]
    public void FourOfOneSuitAndOneOther_DoesNotMatch()
    {
        List<ICard> cards = TestHands.Of(
            (Rank.Two, Suit.Hearts), (Rank.Five, Suit.Hearts), (Rank.Nine, Suit.Hearts),
            (Rank.Jack, Suit.Hearts), (Rank.King, Suit.Spades));

        Assert.False(_flush.Matches(cards));
        Assert.Empty(_flush.GetScoringCards(cards));
    }

    [Fact]
    public void FewerThanFiveCards_DoesNotMatch_EvenWhenTheyShareASuit()
    {
        List<ICard> cards = TestHands.Of((Rank.Two, Suit.Clubs), (Rank.Nine, Suit.Clubs));

        Assert.False(_flush.Matches(cards));
    }

    [Fact]
    public void SevenCards_WithSixOfOneSuit_ScoresTheFiveHighestOfThatSuit()
    {
        List<ICard> cards = TestHands.Of(
            (Rank.Two, Suit.Clubs), (Rank.Four, Suit.Clubs), (Rank.Six, Suit.Clubs),
            (Rank.Eight, Suit.Clubs), (Rank.Ten, Suit.Clubs), (Rank.Queen, Suit.Clubs),
            (Rank.Ace, Suit.Hearts));

        IReadOnlyList<ICard> scoring = _flush.GetScoringCards(cards);

        Assert.Equal(5, scoring.Count);
        Assert.All(scoring, card => Assert.Equal(Suit.Clubs, card.Suit));
        Assert.DoesNotContain(scoring, card => card.Rank == Rank.Two);
    }

    [Fact]
    public void NoCards_DoesNotMatch()
    {
        Assert.False(_flush.Matches(new List<ICard>()));
        Assert.Empty(_flush.GetScoringCards(new List<ICard>()));
    }
}
