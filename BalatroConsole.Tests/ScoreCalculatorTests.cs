using BalatroConsole.Logic;
using BalatroConsole.Logic.Combinations;
using BalatroConsole.Models;

namespace BalatroConsole.Tests;

public class ScoreCalculatorTests
{
    private readonly ScoreCalculator _calculator = new();

    // A combination with fixed base values, so a test can pick the exact numbers it wants.
    private class FixedCombination : IHandCombination
    {
        public FixedCombination(int basePoints, int baseMultiplier)
        {
            BasePoints = basePoints;
            BaseMultiplier = baseMultiplier;
        }

        public HandType Type => HandType.Pair;
        public string Name => "Fixed";
        public int BasePoints { get; }
        public int BaseMultiplier { get; }
        public bool Matches(IReadOnlyList<ICard> cards) => true;
        public IReadOnlyList<ICard> GetScoringCards(IReadOnlyList<ICard> cards) => cards;
    }

    // A card the calculator has never heard of: adds 3 points and then adds 1 to the multiplier.
    private class MadeUpCard : ICard
    {
        public Rank Rank => Rank.Two;
        public Suit Suit => Suit.Hearts;
        public int Points => 2;
        public Score Apply(Score current) => new(current.Points + 3, current.Multiplier + 1);
    }

    [Fact]
    public void WithoutScoringCards_TheScoreIsTheBaseScore()
    {
        Score score = _calculator.Calculate(new FixedCombination(20, 2), new List<ICard>());

        Assert.Equal(new Score(20, 2), score);
    }

    [Fact]
    public void NormalCards_AddTheirPoints_AndKeepTheMultiplier()
    {
        List<ICard> cards = TestHands.Of((Rank.King, Suit.Spades), (Rank.Five, Suit.Hearts));

        Score score = _calculator.Calculate(new FixedCombination(10, 2), cards);

        Assert.Equal(new Score(25, 2), score);   // 10 + 10 + 5
    }

    [Fact]
    public void Score_WithABonusCard_IsExactlyTenPointsMoreThanWithANormalCard()
    {
        // Opdracht D: base 20 (a pair), a BonusCard in the hand is worth +10 on top of the normal score.
        FixedCombination pair = new(20, 1);
        List<ICard> normal = new() { new Card(Rank.Five, Suit.Clubs) };
        List<ICard> bonus = new() { new BonusCard(Rank.Five, Suit.Clubs) };

        Score withNormal = _calculator.Calculate(pair, normal);
        Score withBonus = _calculator.Calculate(pair, bonus);

        Assert.Equal(25, withNormal.Total);                  // 20 + 5
        Assert.Equal(35, withBonus.Total);                   // 20 + 15
        Assert.Equal(10, withBonus.Total - withNormal.Total);
    }

    [Fact]
    public void Score_WithAGlassCard_DoublesTheMultiplier()
    {
        // Opdracht E: base 50, the card brings the 10 to make 60, and the glass card doubles it: 120.
        List<ICard> cards = new() { new GlassCard(Rank.Ten, Suit.Hearts) };

        Score score = _calculator.Calculate(new FixedCombination(50, 1), cards);

        Assert.Equal(60, score.Points);
        Assert.Equal(2, score.Multiplier);
        Assert.Equal(120, score.Total);
    }

    [Fact]
    public void ACardTheCalculatorDoesNotKnow_StillWorks()
    {
        // The calculator (and the evaluator) only use ICard.Apply, so a brand new card type
        // needs no change to them.
        Score score = _calculator.Calculate(new FixedCombination(10, 2), new List<ICard> { new MadeUpCard() });

        Assert.Equal(new Score(13, 3), score);
    }

    [Fact]
    public void HandEvaluator_UsesTheCalculator_ForACardTypeItDoesNotKnow()
    {
        HandEvaluator evaluator = new(HandCombinations.CreateAll());

        ScoreResult result = evaluator.Evaluate(new List<ICard> { new MadeUpCard() });   // one card: high card 5 x 1

        Assert.Equal(8, result.Points);        // 5 base + 3 from the made-up card
        Assert.Equal(2, result.Multiplier);    // 1 + 1
        Assert.Equal(16, result.Total);
    }
}
