using BalatroConsole.Logic.Combinations;
using BalatroConsole.Models;

namespace BalatroConsole.Tests;

// Tests each combination class on its own, without the evaluator.
public class CombinationTests
{
    [Fact]
    public void Pair_Matches_WhenTwoCardsShareARank()
    {
        List<ICard> cards = new()
        {
            new Card(Rank.Four, Suit.Hearts),
            new Card(Rank.Four, Suit.Spades),
            new Card(Rank.Nine, Suit.Clubs)
        };

        Assert.True(new Pair().Matches(cards));
    }

    [Fact]
    public void Pair_DoesNotMatch_WhenAllRanksAreDifferent()
    {
        List<ICard> cards = new()
        {
            new Card(Rank.Four, Suit.Hearts),
            new Card(Rank.Five, Suit.Hearts),
            new Card(Rank.Nine, Suit.Hearts)
        };

        Assert.False(new Pair().Matches(cards));
        Assert.Empty(new Pair().GetScoringCards(cards));
    }

    [Fact]
    public void Pair_GetScoringCards_ReturnsExactlyTwo_WhenThreeShareARank()
    {
        List<ICard> cards = new()
        {
            new Card(Rank.Five, Suit.Spades),
            new Card(Rank.Five, Suit.Hearts),
            new Card(Rank.Five, Suit.Clubs)
        };

        IReadOnlyList<ICard> scoring = new Pair().GetScoringCards(cards);

        Assert.Equal(2, scoring.Count);
    }

    [Fact]
    public void Pair_GetScoringCards_PicksTheHighestPair_WhenThereAreTwoPairs()
    {
        List<ICard> cards = new()
        {
            new Card(Rank.Three, Suit.Spades),
            new Card(Rank.Three, Suit.Hearts),
            new Card(Rank.King, Suit.Clubs),
            new Card(Rank.King, Suit.Diamonds)
        };

        IReadOnlyList<ICard> scoring = new Pair().GetScoringCards(cards);

        Assert.Equal(2, scoring.Count);
        Assert.All(scoring, card => Assert.Equal(Rank.King, card.Rank));
    }

    [Fact]
    public void ThreeOfAKind_DoesNotMatch_WhenOnlyTwoShareARank()
    {
        List<ICard> cards = new()
        {
            new Card(Rank.Jack, Suit.Spades),
            new Card(Rank.Jack, Suit.Hearts),
            new Card(Rank.Two, Suit.Clubs)
        };

        Assert.False(new ThreeOfAKind().Matches(cards));
    }

    [Fact]
    public void ThreeOfAKind_GetScoringCards_ReturnsExactlyThree_WhenFourShareARank()
    {
        List<ICard> cards = new()
        {
            new Card(Rank.Seven, Suit.Spades),
            new Card(Rank.Seven, Suit.Hearts),
            new Card(Rank.Seven, Suit.Clubs),
            new Card(Rank.Seven, Suit.Diamonds)
        };

        IReadOnlyList<ICard> scoring = new ThreeOfAKind().GetScoringCards(cards);

        Assert.Equal(3, scoring.Count);
    }

    [Fact]
    public void HighCard_GetScoringCards_ReturnsOnlyTheHighestRank()
    {
        ICard queen = new Card(Rank.Queen, Suit.Clubs);
        List<ICard> cards = new()
        {
            new Card(Rank.Three, Suit.Spades),
            queen,
            new Card(Rank.Ten, Suit.Hearts)
        };

        IReadOnlyList<ICard> scoring = new HighCard().GetScoringCards(cards);

        Assert.Single(scoring);
        Assert.Same(queen, scoring[0]);
    }

    [Fact]
    public void HighCard_DoesNotMatch_WhenNoCardsArePlayed()
    {
        HighCard highCard = new();
        List<ICard> noCards = new();

        Assert.False(highCard.Matches(noCards));
        Assert.Empty(highCard.GetScoringCards(noCards));
    }

    [Fact]
    public void BaseScores_GetHigherForHarderCombinations()
    {
        // The assignment says harder combinations give more base points and multiplier.
        HighCard highCard = new();
        Pair pair = new();
        ThreeOfAKind three = new();

        Assert.True(three.BasePoints > pair.BasePoints && pair.BasePoints > highCard.BasePoints);
        Assert.True(three.BaseMultiplier > pair.BaseMultiplier && pair.BaseMultiplier > highCard.BaseMultiplier);
    }
}
