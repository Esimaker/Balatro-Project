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
}
