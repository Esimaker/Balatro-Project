using BalatroConsole.Logic;
using BalatroConsole.Logic.Combinations;

namespace BalatroConsole.Tests;

// One row per hand: which combination it must be, and the exact points and total.
// The arithmetic is written out so a failing row can be checked by hand.
public class ScoringTableTests
{
    private readonly HandEvaluator _evaluator = new(HandCombinations.CreateAll());

    [Theory]
    // High card: 5 base x1, only the highest card scores.
    [InlineData("AS 7D 3C 2H 9S", HandType.HighCard, 16, 16)]          // 5 + 11
    [InlineData("2H", HandType.HighCard, 7, 7)]                          // 5 + 2
    [InlineData("KH 4D", HandType.HighCard, 15, 15)]                     // 5 + 10
    // Pair: 10 base x2, only the pair scores.
    [InlineData("TH TS 4D 9C JH", HandType.Pair, 30, 60)]                // 10 + 10 + 10
    [InlineData("2H 2S", HandType.Pair, 14, 28)]                         // 10 + 2 + 2
    [InlineData("AH AS 3D", HandType.Pair, 32, 64)]                      // 10 + 11 + 11
    // Two pair: 20 base x2, the four paired cards score.
    [InlineData("AH AS KD KC 2H", HandType.TwoPair, 62, 124)]            // 20 + 11 + 11 + 10 + 10
    [InlineData("4H 4S KD KC 9H", HandType.TwoPair, 48, 96)]             // 20 + 4 + 4 + 10 + 10
    // Three of a kind: 30 base x3.
    [InlineData("2S 2H 2C 9D KS", HandType.ThreeOfAKind, 36, 108)]       // 30 + 6
    [InlineData("AS AH AC", HandType.ThreeOfAKind, 63, 189)]             // 30 + 33
    // Straight: 35 base x4, all five score.
    [InlineData("5H 6S 7C 8D 9H", HandType.Straight, 70, 280)]           // 35 + 35
    [InlineData("AH 2S 3C 4D 5H", HandType.Straight, 60, 240)]           // 35 + 11 + 2 + 3 + 4 + 5
    [InlineData("TH JS QC KD AH", HandType.Straight, 86, 344)]           // 35 + 10 + 10 + 10 + 10 + 11
    // Flush: 40 base x4, all five score.
    [InlineData("KS QS JS 9S 3S", HandType.Flush, 82, 328)]              // 40 + 10 + 10 + 10 + 9 + 3
    [InlineData("2H 5H 9H JH KH", HandType.Flush, 76, 304)]              // 40 + 2 + 5 + 9 + 10 + 10
    [InlineData("5H 6H 7H 8H 9H", HandType.Flush, 75, 300)]              // straight flush scores as Flush: 40 + 35
    // Full house: 50 base x4, all five score.
    [InlineData("AS AH AC 2D 2S", HandType.FullHouse, 87, 348)]          // 50 + 33 + 4
    [InlineData("KS KH KC 4D 4S", HandType.FullHouse, 88, 352)]          // 50 + 30 + 8
    // Four of a kind: 60 base x7, only the four score.
    [InlineData("AS AH AC AD KS", HandType.FourOfAKind, 104, 728)]       // 60 + 44
    [InlineData("3S 3H 3C 3D", HandType.FourOfAKind, 72, 504)]           // 60 + 12
    public void Evaluate_GivesTheExpectedCombination_Points_AndTotal(string hand, HandType type, int points, int total)
    {
        ScoreResult result = _evaluator.Evaluate(TestHands.Parse(hand));

        Assert.Equal(type, result.Combination.Type);
        Assert.Equal(points, result.Points);
        Assert.Equal(total, result.Total);
    }

    [Theory]
    [InlineData("5H 6S 7C 8D 9H", true)]       // the criteria: 5-6-7-8-9 matches
    [InlineData("5H 6S 7C 8D 8H", false)]      // the criteria: 5-6-7-8-8 does not
    [InlineData("AH 2S 3C 4D 5H", true)]       // the criteria: A-2-3-4-5 matches
    [InlineData("TH JS QC KD AH", true)]
    [InlineData("JH QS KC AD 2H", false)]      // no wrap-around
    [InlineData("2H 3S 4C 5D 7H", false)]      // gap
    public void Straight_MatchesTheCriteria(string hand, bool expected)
    {
        Assert.Equal(expected, new Straight().Matches(TestHands.Parse(hand)));
    }

    [Theory]
    [InlineData("4H 4S KD KC", true)]          // the criteria: 4-4-K-K matches
    [InlineData("4H 4S 4D KC", false)]         // the criteria: 4-4-4-K does not
    [InlineData("4H 4S KD", false)]
    [InlineData("4H 4S 4D KC KS", false)]      // that is a full house
    public void TwoPair_MatchesTheCriteria(string hand, bool expected)
    {
        Assert.Equal(expected, new TwoPair().Matches(TestHands.Parse(hand)));
    }

    [Theory]
    [InlineData("3H 3S 3D 3C", true)]          // the criteria: 3-3-3-3 scores
    [InlineData("3H 3S 3D", false)]            // the criteria: 3-3-3 does not
    [InlineData("3H 3S 3D 3C KS", true)]
    [InlineData("3H 3S KD KC", false)]
    public void FourOfAKind_MatchesTheCriteria(string hand, bool expected)
    {
        Assert.Equal(expected, new FourOfAKind().Matches(TestHands.Parse(hand)));
    }

    [Theory]
    [InlineData("TH TS 4D", true)]
    [InlineData("TH TS TD", false)]            // the criteria: 7-7-7 is not a pair
    [InlineData("TH 4S 9D", false)]
    public void Pair_MatchesTheCriteria(string hand, bool expected)
    {
        Assert.Equal(expected, new Pair().Matches(TestHands.Parse(hand)));
    }

    [Theory]
    [InlineData("9H 9S 9D", true)]             // the criteria: 9-9-9 scores
    [InlineData("9H 9S", false)]               // the criteria: 9-9 does not
    [InlineData("9H 9S 9D 9C", false)]
    public void ThreeOfAKind_MatchesTheCriteria(string hand, bool expected)
    {
        Assert.Equal(expected, new ThreeOfAKind().Matches(TestHands.Parse(hand)));
    }

    [Theory]
    [InlineData("2H 5H 9H JH KH", true)]       // the criteria: five hearts match
    [InlineData("2H 5H 9H JH KD", false)]      // the criteria: four hearts + one diamond do not
    [InlineData("2H 5H 9H JH", false)]
    public void Flush_MatchesTheCriteria(string hand, bool expected)
    {
        Assert.Equal(expected, new Flush().Matches(TestHands.Parse(hand)));
    }

    [Fact]
    public void Parse_ReadsRanksAndSuits()
    {
        var cards = TestHands.Parse("TH AS 2D");

        Assert.Equal(3, cards.Count);
        Assert.Equal(("Ten", "Hearts"), (cards[0].Rank.ToString(), cards[0].Suit.ToString()));
        Assert.Equal(("Ace", "Spades"), (cards[1].Rank.ToString(), cards[1].Suit.ToString()));
        Assert.Equal(("Two", "Diamonds"), (cards[2].Rank.ToString(), cards[2].Suit.ToString()));
    }
}
