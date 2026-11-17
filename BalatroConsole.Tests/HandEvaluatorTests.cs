using BalatroConsole.Logic;
using BalatroConsole.Logic.Combinations;
using BalatroConsole.Models;

namespace BalatroConsole.Tests;

public class HandEvaluatorTests
{
    private readonly HandEvaluator _evaluator = new();

    [Fact]
    public void Evaluate_KingKingThreeSevenNine_IsPair()
    {
        // Arrange (Opdracht C)
        List<ICard> cards = new()
        {
            new Card(Rank.King, Suit.Spades),
            new Card(Rank.King, Suit.Diamonds),
            new Card(Rank.Three, Suit.Hearts),
            new Card(Rank.Seven, Suit.Clubs),
            new Card(Rank.Nine, Suit.Diamonds)
        };

        // Act
        ScoreResult result = _evaluator.Evaluate(cards);

        // Assert
        Assert.Equal(HandType.Pair, result.Combination.Type);
    }

    [Fact]
    public void Evaluate_ThreeSameRank_IsThreeOfAKindNotPair()
    {
        List<ICard> cards = new()
        {
            new Card(Rank.Five, Suit.Spades),
            new Card(Rank.Five, Suit.Hearts),
            new Card(Rank.Five, Suit.Clubs)
        };

        ScoreResult result = _evaluator.Evaluate(cards);

        Assert.Equal(HandType.ThreeOfAKind, result.Combination.Type);
    }

    [Fact]
    public void Evaluate_NoMatchingRanks_IsHighCard()
    {
        List<ICard> cards = new()
        {
            new Card(Rank.Two, Suit.Spades),
            new Card(Rank.Ace, Suit.Hearts)
        };

        ScoreResult result = _evaluator.Evaluate(cards);

        Assert.Equal(HandType.HighCard, result.Combination.Type);
    }

    [Fact]
    public void Evaluate_PairOfKings_AddsCardPointsThenMultiplies()
    {
        // Pair base = 10 points x 2 mult. Two kings add 10 + 10.
        List<ICard> cards = new()
        {
            new Card(Rank.King, Suit.Spades),
            new Card(Rank.King, Suit.Hearts)
        };

        ScoreResult result = _evaluator.Evaluate(cards);

        Assert.Equal(30, result.Points);     // 10 + 10 + 10
        Assert.Equal(2, result.Multiplier);
        Assert.Equal(60, result.Total);      // 30 x 2
    }

    [Fact]
    public void Evaluate_PairWithExtraCards_OnlyScoresThePairedCards()
    {
        // K K 3 7 9: the 3, 7 and 9 are played but are not part of the pair, so they must not score.
        List<ICard> cards = new()
        {
            new Card(Rank.King, Suit.Spades),
            new Card(Rank.King, Suit.Diamonds),
            new Card(Rank.Three, Suit.Hearts),
            new Card(Rank.Seven, Suit.Clubs),
            new Card(Rank.Nine, Suit.Diamonds)
        };

        ScoreResult result = _evaluator.Evaluate(cards);

        Assert.Equal(2, result.ScoringCards.Count);
        Assert.All(result.ScoringCards, card => Assert.Equal(Rank.King, card.Rank));
        Assert.Equal(30, result.Points);     // 10 base + 10 + 10, not + 3 + 7 + 9
        Assert.Equal(60, result.Total);      // 30 x 2
    }

    [Fact]
    public void Evaluate_HighCard_OnlyScoresTheHighestCard()
    {
        List<ICard> cards = new()
        {
            new Card(Rank.Two, Suit.Spades),
            new Card(Rank.Ace, Suit.Hearts),
            new Card(Rank.Six, Suit.Clubs)
        };

        ScoreResult result = _evaluator.Evaluate(cards);

        Assert.Single(result.ScoringCards);
        Assert.Equal(Rank.Ace, result.ScoringCards[0].Rank);
        Assert.Equal(16, result.Points);     // 5 base + 11 for the ace
        Assert.Equal(16, result.Total);      // x1
    }

    [Fact]
    public void Evaluate_ThreeOfAKindWithExtraCards_OnlyScoresTheThree()
    {
        List<ICard> cards = new()
        {
            new Card(Rank.Eight, Suit.Spades),
            new Card(Rank.Eight, Suit.Hearts),
            new Card(Rank.Eight, Suit.Clubs),
            new Card(Rank.Ace, Suit.Diamonds),
            new Card(Rank.Two, Suit.Diamonds)
        };

        ScoreResult result = _evaluator.Evaluate(cards);

        Assert.Equal(HandType.ThreeOfAKind, result.Combination.Type);
        Assert.Equal(3, result.ScoringCards.Count);
        Assert.Equal(54, result.Points);     // 30 base + 8 + 8 + 8
        Assert.Equal(162, result.Total);     // 54 x 3
    }

    [Fact]
    public void Evaluate_TwoPairs_IsTwoPair_AndIgnoresTheKicker()
    {
        List<ICard> cards = new()
        {
            new Card(Rank.King, Suit.Spades),
            new Card(Rank.King, Suit.Diamonds),
            new Card(Rank.Three, Suit.Hearts),
            new Card(Rank.Three, Suit.Clubs),
            new Card(Rank.Nine, Suit.Diamonds)
        };

        ScoreResult result = _evaluator.Evaluate(cards);

        Assert.Equal(HandType.TwoPair, result.Combination.Type);
        Assert.Equal(4, result.ScoringCards.Count);
        Assert.Equal(46, result.Points);     // 20 base + 10 + 10 + 3 + 3, the 9 does not score
        Assert.Equal(92, result.Total);      // 46 x 2
    }

    [Fact]
    public void Evaluate_FourSameRank_IsFourOfAKindNotThreeOfAKind()
    {
        List<ICard> cards = new()
        {
            new Card(Rank.Four, Suit.Spades),
            new Card(Rank.Four, Suit.Hearts),
            new Card(Rank.Four, Suit.Clubs),
            new Card(Rank.Four, Suit.Diamonds),
            new Card(Rank.Jack, Suit.Spades)
        };

        ScoreResult result = _evaluator.Evaluate(cards);

        Assert.Equal(HandType.FourOfAKind, result.Combination.Type);
        Assert.Equal(76, result.Points);     // 60 base + 4 x 4
        Assert.Equal(532, result.Total);     // 76 x 7
    }

    [Fact]
    public void Evaluate_OnePair_IsStillPair_NotTwoPair()
    {
        List<ICard> cards = new()
        {
            new Card(Rank.King, Suit.Spades),
            new Card(Rank.King, Suit.Diamonds),
            new Card(Rank.Three, Suit.Hearts),
            new Card(Rank.Seven, Suit.Clubs)
        };

        Assert.Equal(HandType.Pair, _evaluator.Evaluate(cards).Combination.Type);
    }

    [Fact]
    public void Evaluate_NoCards_Throws()
    {
        Assert.Throws<ArgumentException>(() => _evaluator.Evaluate(new List<ICard>()));
    }
}
