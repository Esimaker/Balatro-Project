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
    public void Pair_DoesNotMatch_WhenThreeShareARank()
    {
        // 7-7-7 is three of a kind, not a pair.
        List<ICard> cards = new()
        {
            new Card(Rank.Seven, Suit.Spades),
            new Card(Rank.Seven, Suit.Hearts),
            new Card(Rank.Seven, Suit.Clubs)
        };

        Assert.False(new Pair().Matches(cards));
        Assert.Empty(new Pair().GetScoringCards(cards));
    }

    [Fact]
    public void Pair_SevenSeven_Matches_AndScoresBothCards()
    {
        List<ICard> cards = new()
        {
            new Card(Rank.Seven, Suit.Spades),
            new Card(Rank.Seven, Suit.Hearts)
        };

        Assert.True(new Pair().Matches(cards));
        Assert.Equal(2, new Pair().GetScoringCards(cards).Count);
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
    public void ThreeOfAKind_DoesNotMatch_WhenFourShareARank()
    {
        // 9-9-9-9 is four of a kind, not three of a kind.
        List<ICard> cards = new()
        {
            new Card(Rank.Nine, Suit.Spades),
            new Card(Rank.Nine, Suit.Hearts),
            new Card(Rank.Nine, Suit.Clubs),
            new Card(Rank.Nine, Suit.Diamonds)
        };

        Assert.False(new ThreeOfAKind().Matches(cards));
        Assert.Empty(new ThreeOfAKind().GetScoringCards(cards));
    }

    [Fact]
    public void ThreeOfAKind_NineNineNine_Matches_AndScoresAllThree()
    {
        List<ICard> cards = new()
        {
            new Card(Rank.Nine, Suit.Spades),
            new Card(Rank.Nine, Suit.Hearts),
            new Card(Rank.Nine, Suit.Clubs)
        };

        Assert.True(new ThreeOfAKind().Matches(cards));
        Assert.Equal(3, new ThreeOfAKind().GetScoringCards(cards).Count);
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

    [Fact]
    public void TwoPair_Matches_AndScoresTheFourPairedCards()
    {
        List<ICard> cards = new()
        {
            new Card(Rank.King, Suit.Spades),
            new Card(Rank.King, Suit.Diamonds),
            new Card(Rank.Three, Suit.Hearts),
            new Card(Rank.Three, Suit.Clubs),
            new Card(Rank.Nine, Suit.Diamonds)
        };
        TwoPair twoPair = new();

        Assert.True(twoPair.Matches(cards));
        IReadOnlyList<ICard> scoring = twoPair.GetScoringCards(cards);
        Assert.Equal(4, scoring.Count);
        Assert.DoesNotContain(scoring, card => card.Rank == Rank.Nine);
    }

    [Fact]
    public void TwoPair_DoesNotMatch_WithOnlyOnePair()
    {
        List<ICard> cards = new()
        {
            new Card(Rank.King, Suit.Spades),
            new Card(Rank.King, Suit.Diamonds),
            new Card(Rank.Three, Suit.Hearts)
        };

        Assert.False(new TwoPair().Matches(cards));
        Assert.Empty(new TwoPair().GetScoringCards(cards));
    }

    [Fact]
    public void TwoPair_DoesNotMatch_WhenFourCardsShareOneRank()
    {
        // Four of the same rank is ONE rank, not two different pairs.
        List<ICard> cards = new()
        {
            new Card(Rank.Six, Suit.Spades),
            new Card(Rank.Six, Suit.Hearts),
            new Card(Rank.Six, Suit.Clubs),
            new Card(Rank.Six, Suit.Diamonds)
        };

        Assert.False(new TwoPair().Matches(cards));
    }

    [Fact]
    public void TwoPair_DoesNotMatch_WhenThreeShareARankAndTwoShareAnother()
    {
        // K-K-K-4-4 is a full house, not two pair.
        List<ICard> cards = new()
        {
            new Card(Rank.King, Suit.Spades),
            new Card(Rank.King, Suit.Hearts),
            new Card(Rank.King, Suit.Clubs),
            new Card(Rank.Four, Suit.Spades),
            new Card(Rank.Four, Suit.Hearts)
        };

        Assert.False(new TwoPair().Matches(cards));
    }

    [Fact]
    public void TwoPair_FourFourThreeTimes_DoesNotMatch()
    {
        // The criteria: 4-4-4-K is not two pair.
        List<ICard> cards = new()
        {
            new Card(Rank.Four, Suit.Spades),
            new Card(Rank.Four, Suit.Hearts),
            new Card(Rank.Four, Suit.Clubs),
            new Card(Rank.King, Suit.Spades)
        };

        Assert.False(new TwoPair().Matches(cards));
    }

    [Fact]
    public void TwoPair_WithThreePairs_ScoresOnlyTheTwoHighest()
    {
        List<ICard> cards = new()
        {
            new Card(Rank.Two, Suit.Spades),
            new Card(Rank.Two, Suit.Hearts),
            new Card(Rank.Five, Suit.Clubs),
            new Card(Rank.Five, Suit.Diamonds),
            new Card(Rank.Queen, Suit.Spades),
            new Card(Rank.Queen, Suit.Hearts)
        };

        IReadOnlyList<ICard> scoring = new TwoPair().GetScoringCards(cards);

        Assert.Equal(4, scoring.Count);
        Assert.All(scoring, card => Assert.NotEqual(Rank.Two, card.Rank));
    }

    [Fact]
    public void FourOfAKind_Matches_AndScoresExactlyFourCards()
    {
        List<ICard> cards = new()
        {
            new Card(Rank.Nine, Suit.Spades),
            new Card(Rank.Nine, Suit.Hearts),
            new Card(Rank.Nine, Suit.Clubs),
            new Card(Rank.Nine, Suit.Diamonds),
            new Card(Rank.Ace, Suit.Spades)
        };
        FourOfAKind four = new();

        Assert.True(four.Matches(cards));
        Assert.Equal(4, four.GetScoringCards(cards).Count);
    }

    [Fact]
    public void FourOfAKind_DoesNotMatch_WithOnlyThreeOfTheSameRank()
    {
        List<ICard> cards = new()
        {
            new Card(Rank.Nine, Suit.Spades),
            new Card(Rank.Nine, Suit.Hearts),
            new Card(Rank.Nine, Suit.Clubs)
        };

        Assert.False(new FourOfAKind().Matches(cards));
    }

    [Fact]
    public void BaseScores_StayInOrder_FromHighCardUpToFourOfAKind()
    {
        // Every combination the game has, weakest to strongest. A new combination must fit in this order.
        List<IHandCombination> weakestToStrongest = HandCombinations.CreateAll().OrderBy(c => c.Type).ToList();

        Assert.Equal(Enum.GetValues<HandType>().Length, weakestToStrongest.Count);   // none forgotten
        for (int i = 1; i < weakestToStrongest.Count; i++)
        {
            Assert.True(weakestToStrongest[i].BasePoints > weakestToStrongest[i - 1].BasePoints);
            Assert.True(weakestToStrongest[i].BaseMultiplier >= weakestToStrongest[i - 1].BaseMultiplier);
            Assert.True(weakestToStrongest[i].Type > weakestToStrongest[i - 1].Type);
        }
    }
}
