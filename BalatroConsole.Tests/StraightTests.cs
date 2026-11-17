using BalatroConsole.Logic.Combinations;
using BalatroConsole.Models;

namespace BalatroConsole.Tests;

public class StraightTests
{
    private readonly Straight _straight = new();

    [Fact]
    public void FiveInARow_MixedSuits_Matches_AndAllFiveScore()
    {
        List<ICard> cards = TestHands.Of(
            (Rank.Five, Suit.Hearts), (Rank.Six, Suit.Spades), (Rank.Seven, Suit.Clubs),
            (Rank.Eight, Suit.Diamonds), (Rank.Nine, Suit.Hearts));

        Assert.True(_straight.Matches(cards));
        Assert.Equal(5, _straight.GetScoringCards(cards).Count);
    }

    [Fact]
    public void FiveInARow_InAnyOrder_Matches()
    {
        List<ICard> cards = TestHands.Of(
            (Rank.Nine, Suit.Hearts), (Rank.Five, Suit.Hearts), (Rank.Seven, Suit.Clubs),
            (Rank.Six, Suit.Spades), (Rank.Eight, Suit.Diamonds));

        Assert.True(_straight.Matches(cards));
    }

    [Fact]
    public void TenToAce_AceHigh_Matches()
    {
        List<ICard> cards = TestHands.Of(
            (Rank.Ten, Suit.Hearts), (Rank.Jack, Suit.Spades), (Rank.Queen, Suit.Clubs),
            (Rank.King, Suit.Diamonds), (Rank.Ace, Suit.Hearts));

        Assert.True(_straight.Matches(cards));
    }

    [Fact]
    public void AceToFive_AceLow_Matches()
    {
        List<ICard> cards = TestHands.Of(
            (Rank.Ace, Suit.Hearts), (Rank.Two, Suit.Spades), (Rank.Three, Suit.Clubs),
            (Rank.Four, Suit.Diamonds), (Rank.Five, Suit.Hearts));

        Assert.True(_straight.Matches(cards));
        Assert.Equal(5, _straight.GetScoringCards(cards).Count);
    }

    [Fact]
    public void QueenKingAceTwoThree_DoesNotWrapAround()
    {
        List<ICard> cards = TestHands.Of(
            (Rank.Queen, Suit.Hearts), (Rank.King, Suit.Spades), (Rank.Ace, Suit.Clubs),
            (Rank.Two, Suit.Diamonds), (Rank.Three, Suit.Hearts));

        Assert.False(_straight.Matches(cards));
        Assert.Empty(_straight.GetScoringCards(cards));
    }

    [Fact]
    public void OnlyFourInARow_DoesNotMatch()
    {
        List<ICard> cards = TestHands.Of(
            (Rank.Five, Suit.Hearts), (Rank.Six, Suit.Spades), (Rank.Seven, Suit.Clubs),
            (Rank.Eight, Suit.Diamonds));

        Assert.False(_straight.Matches(cards));
    }

    [Fact]
    public void FiveCardsWithAGap_DoesNotMatch()
    {
        List<ICard> cards = TestHands.Of(
            (Rank.Two, Suit.Hearts), (Rank.Three, Suit.Spades), (Rank.Four, Suit.Clubs),
            (Rank.Five, Suit.Diamonds), (Rank.Seven, Suit.Hearts));

        Assert.False(_straight.Matches(cards));
    }

    [Fact]
    public void FiveCardsWithAPair_DoesNotMatch()
    {
        List<ICard> cards = TestHands.Of(
            (Rank.Four, Suit.Hearts), (Rank.Five, Suit.Spades), (Rank.Five, Suit.Clubs),
            (Rank.Six, Suit.Diamonds), (Rank.Seven, Suit.Hearts));

        Assert.False(_straight.Matches(cards));
    }

    [Fact]
    public void SixCards_ScoresTheHighestStraight_AndOnlyOneCardPerRank()
    {
        // 3 4 5 6 7 8: both 3-7 and 4-8 are straights, the higher one wins.
        List<ICard> cards = TestHands.Of(
            (Rank.Three, Suit.Hearts), (Rank.Four, Suit.Spades), (Rank.Five, Suit.Clubs),
            (Rank.Six, Suit.Diamonds), (Rank.Seven, Suit.Hearts), (Rank.Eight, Suit.Spades));

        IReadOnlyList<ICard> scoring = _straight.GetScoringCards(cards);

        Assert.Equal(5, scoring.Count);
        Assert.DoesNotContain(scoring, card => card.Rank == Rank.Three);
        Assert.Contains(scoring, card => card.Rank == Rank.Eight);
    }

    [Fact]
    public void NoCards_DoesNotMatch()
    {
        Assert.False(_straight.Matches(new List<ICard>()));
        Assert.Empty(_straight.GetScoringCards(new List<ICard>()));
    }
}
