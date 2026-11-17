using BalatroConsole.Logic;
using BalatroConsole.Logic.Combinations;
using BalatroConsole.Models;

namespace BalatroConsole.Tests;

// Heavier checks of the whole evaluator. Instead of a few hand-picked hands, they run it on
// every possible poker hand and on thousands of random hands.
public class EvaluatorVerificationTests
{
    private readonly HandEvaluator _evaluator = new(HandCombinations.CreateAll());
    private readonly List<IHandCombination> _combinations = HandCombinations.CreateAll();

    [Fact]
    [Trait("Category", "Slow")]   // about 20 seconds: skip it with  dotnet test --filter "Category!=Slow"
    public void AllFiveCardHands_MatchTheKnownPokerCounts()
    {
        // There are 2,598,960 different 5-card hands. How many of each kind exist is known maths,
        // so if the evaluator is wrong for even one family of hands, a count here will be off.
        List<ICard> deck = new Deck().Cards.ToList();
        int typeCount = Enum.GetValues<HandType>().Length;
        long[] totals = new long[typeCount];

        Parallel.For(0, deck.Count - 4, a =>
        {
            long[] local = new long[typeCount];
            ICard[] hand = new ICard[5];
            hand[0] = deck[a];
            for (int b = a + 1; b < deck.Count - 3; b++)
            {
                hand[1] = deck[b];
                for (int c = b + 1; c < deck.Count - 2; c++)
                {
                    hand[2] = deck[c];
                    for (int d = c + 1; d < deck.Count - 1; d++)
                    {
                        hand[3] = deck[d];
                        for (int e = d + 1; e < deck.Count; e++)
                        {
                            hand[4] = deck[e];
                            local[(int)_evaluator.Evaluate(hand).Combination.Type]++;
                        }
                    }
                }
            }
            lock (totals)
            {
                for (int i = 0; i < typeCount; i++) totals[i] += local[i];
            }
        });

        Assert.Equal(624, totals[(int)HandType.FourOfAKind]);
        Assert.Equal(3_744, totals[(int)HandType.FullHouse]);
        Assert.Equal(5_148, totals[(int)HandType.Flush]);           // 5,108 flushes + 40 straight flushes
        Assert.Equal(10_200, totals[(int)HandType.Straight]);       // 10,240 straights - 40 straight flushes
        Assert.Equal(54_912, totals[(int)HandType.ThreeOfAKind]);
        Assert.Equal(123_552, totals[(int)HandType.TwoPair]);
        Assert.Equal(1_098_240, totals[(int)HandType.Pair]);
        Assert.Equal(1_302_540, totals[(int)HandType.HighCard]);
        Assert.Equal(2_598_960, totals.Sum());
    }

    [Fact]
    public void AllFourCardHands_NeverGiveStraightFlushOrFullHouse_AndHave13FourOfAKinds()
    {
        // With fewer than 5 cards those three combinations are impossible.
        List<ICard> deck = new Deck().Cards.ToList();
        Dictionary<HandType, int> counts = new();

        for (int a = 0; a < deck.Count; a++)
        for (int b = a + 1; b < deck.Count; b++)
        for (int c = b + 1; c < deck.Count; c++)
        for (int d = c + 1; d < deck.Count; d++)
        {
            HandType type = _evaluator.Evaluate(new[] { deck[a], deck[b], deck[c], deck[d] }).Combination.Type;
            counts[type] = counts.GetValueOrDefault(type) + 1;
        }

        Assert.DoesNotContain(HandType.Straight, counts.Keys);
        Assert.DoesNotContain(HandType.Flush, counts.Keys);
        Assert.DoesNotContain(HandType.FullHouse, counts.Keys);
        Assert.Equal(13, counts[HandType.FourOfAKind]);
        Assert.Equal(270_725, counts.Values.Sum());                 // 52 choose 4
    }

    [Fact]
    public void RandomHands_FollowEveryScoringRule()
    {
        Random random = new(2026);   // fixed seed: the same hands every run

        for (int round = 0; round < 20_000; round++)
        {
            Deck deck = new(random);
            deck.Shuffle();
            int size = random.Next(1, 6);
            List<ICard> played = deck.Deal(size);

            ScoreResult result = _evaluator.Evaluate(played);

            // 1. Only played cards can score, each at most once.
            Assert.All(result.ScoringCards, card => Assert.Contains(card, played));
            Assert.Equal(result.ScoringCards.Count, result.ScoringCards.Distinct().Count());
            Assert.NotEmpty(result.ScoringCards);

            // 2. The score follows the formula.
            Assert.Equal(result.Combination.BasePoints + result.ScoringCards.Sum(c => c.Points), result.Points);
            Assert.Equal(result.Combination.BaseMultiplier, result.Multiplier);
            Assert.Equal(result.Points * result.Multiplier, result.Total);

            // 3. The chosen combination really matches...
            Assert.True(result.Combination.Matches(played));

            // 4. ...and no stronger combination matches (the strongest one always wins).
            Assert.All(_combinations.Where(c => c.Type > result.Combination.Type),
                stronger => Assert.False(stronger.Matches(played), $"{stronger.Name} also matched"));
        }
    }

    [Fact]
    public void ScoringCards_OfEveryCombination_AreEmptyExactlyWhenItDoesNotMatch()
    {
        Random random = new(11);

        for (int i = 0; i < 5_000; i++)
        {
            Deck deck = new(random);
            deck.Shuffle();
            List<ICard> played = deck.Deal(random.Next(1, 6));

            foreach (IHandCombination combination in _combinations)
            {
                bool matches = combination.Matches(played);
                int scoring = combination.GetScoringCards(played).Count;
                Assert.Equal(matches, scoring > 0);
            }
        }
    }

    [Fact]
    public void Evaluate_MoreThanFiveCards_StillGivesAValidResult()
    {
        // Round never plays more than 5, but the evaluator should not fall over if it is given the whole hand.
        Random random = new(5);

        for (int i = 0; i < 2_000; i++)
        {
            Deck deck = new(random);
            deck.Shuffle();
            List<ICard> played = deck.Deal(random.Next(6, 9));

            ScoreResult result = _evaluator.Evaluate(played);

            Assert.True(result.Combination.Matches(played));
            Assert.Equal(result.Points * result.Multiplier, result.Total);
            Assert.All(result.ScoringCards, card => Assert.Contains(card, played));
        }
    }

    [Fact]
    public void Evaluate_DoesNotChangeTheCardsItIsGiven()
    {
        List<ICard> played = TestHands.Parse("KS KD 3H 7C 9D");
        List<ICard> copy = new(played);

        _evaluator.Evaluate(played);

        Assert.Equal(copy, played);   // same cards, same order
    }

    [Fact]
    public void Evaluate_GivesTheSameResultForTheSameCards_InAnyOrder()
    {
        Random random = new(99);

        for (int i = 0; i < 2_000; i++)
        {
            Deck deck = new(random);
            deck.Shuffle();
            List<ICard> played = deck.Deal(random.Next(1, 6));
            List<ICard> reordered = played.OrderBy(_ => random.Next()).ToList();

            ScoreResult first = _evaluator.Evaluate(played);
            ScoreResult second = _evaluator.Evaluate(reordered);

            Assert.Equal(first.Combination.Type, second.Combination.Type);
            Assert.Equal(first.Total, second.Total);
        }
    }
}
