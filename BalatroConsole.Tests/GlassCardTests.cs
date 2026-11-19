using BalatroConsole.Logic;
using BalatroConsole.Logic.Combinations;
using BalatroConsole.Models;

namespace BalatroConsole.Tests;

public class GlassCardTests
{
    private readonly HandEvaluator _evaluator = new(HandCombinations.CreateAll());

    // ----- The card itself -----

    [Fact]
    public void GlassCard_IsAnICard_ButNotAChildOfCard()
    {
        GlassCard glass = new(Rank.Queen, Suit.Spades);

        Assert.IsAssignableFrom<ICard>(glass);
        Assert.False(glass is Card);
        Assert.Equal(Rank.Queen, glass.Rank);
        Assert.Equal(Suit.Spades, glass.Suit);
    }

    [Theory]
    [InlineData(Rank.Two)]
    [InlineData(Rank.Seven)]
    [InlineData(Rank.Jack)]
    [InlineData(Rank.Ace)]
    public void GlassCard_HasTheSamePointsAsANormalCard(Rank rank)
    {
        Assert.Equal(new Card(rank, Suit.Clubs).Points, new GlassCard(rank, Suit.Clubs).Points);
    }

    [Fact]
    public void GlassCard_ShowsAStar_InTheConsole()
    {
        Assert.Equal("5♣*", new GlassCard(Rank.Five, Suit.Clubs).ToString());
        Assert.Equal("A♠*", new GlassCard(Rank.Ace, Suit.Spades).ToString());
    }

    [Fact]
    public void Apply_AddsThePoints_AndDoublesTheMultiplier()
    {
        GlassCard glass = new(Rank.Five, Suit.Clubs);

        Score result = glass.Apply(new Score(10, 2));

        Assert.Equal(new Score(15, 4), result);
    }

    // ----- Scoring with glass cards (the evaluator does not know about glass cards) -----

    [Fact]
    public void Score_WithGlassCard_DoublesFinalScore()
    {
        List<ICard> normal = TestHands.Of((Rank.King, Suit.Spades), (Rank.King, Suit.Hearts));
        List<ICard> withGlass = new() { new GlassCard(Rank.King, Suit.Spades), new Card(Rank.King, Suit.Hearts) };

        ScoreResult normalResult = _evaluator.Evaluate(normal);
        ScoreResult glassResult = _evaluator.Evaluate(withGlass);

        Assert.Equal(60, normalResult.Total);                    // (10 + 10 + 10) x 2
        Assert.Equal(normalResult.Points, glassResult.Points);   // the points do not change
        Assert.Equal(4, glassResult.Multiplier);                 // x2 becomes x4
        Assert.Equal(120, glassResult.Total);                    // so the total is exactly doubled
    }

    [Fact]
    public void TwoGlassCards_MultiplyTheMultiplierBy4()
    {
        List<ICard> hand = new() { new GlassCard(Rank.King, Suit.Spades), new GlassCard(Rank.King, Suit.Hearts) };

        ScoreResult result = _evaluator.Evaluate(hand);

        Assert.Equal(8, result.Multiplier);   // pair x2, then x2, then x2
        Assert.Equal(30 * 8, result.Total);
    }

    [Fact]
    public void GlassCard_OutsideTheCombination_ChangesNothing()
    {
        // The glass 9 is played but is not part of the pair, so it does not score.
        List<ICard> hand = new()
        {
            new Card(Rank.King, Suit.Spades), new Card(Rank.King, Suit.Hearts),
            new Card(Rank.Three, Suit.Clubs), new GlassCard(Rank.Nine, Suit.Diamonds)
        };

        ScoreResult result = _evaluator.Evaluate(hand);

        Assert.Equal(HandType.Pair, result.Combination.Type);
        Assert.Equal(2, result.Multiplier);
        Assert.Equal(60, result.Total);
    }

    [Fact]
    public void BonusAndGlass_TogetherAddThePointsAndDoubleTheMultiplier()
    {
        List<ICard> hand = new() { new BonusCard(Rank.Five, Suit.Hearts), new GlassCard(Rank.Five, Suit.Spades) };

        ScoreResult result = _evaluator.Evaluate(hand);

        Assert.Equal(30, result.Points);       // 10 base + 15 (bonus 5) + 5 (glass 5)
        Assert.Equal(4, result.Multiplier);    // pair x2, glass x2
        Assert.Equal(120, result.Total);
    }

    [Fact]
    public void TheOrderOfBonusAndGlass_DoesNotChangeTheScore()
    {
        // Points only add up and the multiplier only multiplies, so the order does not matter yet.
        // (It would once a card adds to the multiplier instead of multiplying it.)
        ICard bonus = new BonusCard(Rank.Five, Suit.Hearts);
        ICard glass = new GlassCard(Rank.Five, Suit.Spades);

        ScoreResult bonusFirst = _evaluator.Evaluate(new List<ICard> { bonus, glass });
        ScoreResult glassFirst = _evaluator.Evaluate(new List<ICard> { glass, bonus });

        Assert.Equal(bonusFirst.Total, glassFirst.Total);
    }

    [Fact]
    public void ASingleGlassCard_ScoresAsAHighCard_WithADoubledMultiplier()
    {
        ScoreResult result = _evaluator.Evaluate(new List<ICard> { new GlassCard(Rank.Ace, Suit.Hearts) });

        Assert.Equal(HandType.HighCard, result.Combination.Type);
        Assert.Equal(16, result.Points);       // 5 base + 11
        Assert.Equal(2, result.Multiplier);    // 1 x 2
        Assert.Equal(32, result.Total);
    }

    [Fact]
    public void TurningScoringCardsIntoGlassCards_MultipliesTheMultiplierBy2_ForEachOne()
    {
        // The strongest check: for thousands of random hands, a glass version of the same hand
        // must keep the combination and the points, and double the multiplier once per scoring card.
        Random random = new(808);

        for (int i = 0; i < 5_000; i++)
        {
            Deck deck = new(random);
            deck.Shuffle();
            List<ICard> plain = deck.Draw(random.Next(1, 6));
            List<ICard> glass = plain.Select(c => (ICard)new GlassCard(c.Rank, c.Suit)).ToList();

            ScoreResult plainResult = _evaluator.Evaluate(plain);
            ScoreResult glassResult = _evaluator.Evaluate(glass);

            Assert.Equal(plainResult.Combination.Type, glassResult.Combination.Type);
            Assert.Equal(plainResult.Points, glassResult.Points);
            Assert.Equal(plainResult.Multiplier << plainResult.ScoringCards.Count, glassResult.Multiplier);
        }
    }

    [Fact]
    public void SwappingOneScoringCardForAGlassCard_DoublesTheTotal_InRandomHands()
    {
        Random random = new(909);

        for (int i = 0; i < 5_000; i++)
        {
            Deck deck = new(random);
            deck.Shuffle();
            List<ICard> plain = deck.Draw(random.Next(1, 6));
            ScoreResult plainResult = _evaluator.Evaluate(plain);

            ICard scoring = plainResult.ScoringCards[0];
            List<ICard> swapped = new(plain);
            swapped[plain.IndexOf(scoring)] = new GlassCard(scoring.Rank, scoring.Suit);
            ScoreResult swappedResult = _evaluator.Evaluate(swapped);

            Assert.Equal(plainResult.Total * 2, swappedResult.Total);
        }
    }

    // ----- Glass cards in the deck -----

    [Fact]
    public void Deck_WithGlassAndBonusCards_HasTheExactCounts_AndStill52UniqueCards()
    {
        Deck deck = new(new Random(1), bonusCardCount: 6, glassCardCount: 4);

        Assert.Equal(6, deck.Cards.OfType<BonusCard>().Count());
        Assert.Equal(4, deck.Cards.OfType<GlassCard>().Count());
        Assert.Equal(52, deck.Cards.Count);
        Assert.Equal(52, deck.Cards.Select(c => (c.Rank, c.Suit)).Distinct().Count());
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(52)]
    public void Deck_GlassCardCount_IsExact(int count)
    {
        Deck deck = new(new Random(2), glassCardCount: count);

        Assert.Equal(count, deck.Cards.OfType<GlassCard>().Count());
        Assert.Equal(52, deck.Cards.Count);
    }

    [Fact]
    public void Deck_ByDefault_HasNoGlassCards()
    {
        Assert.Empty(new Deck().Cards.OfType<GlassCard>());
    }

    [Theory]
    [InlineData(-1, 0)]
    [InlineData(0, 53)]
    [InlineData(30, 23)]   // 53 special cards in a deck of 52
    public void Deck_WithImpossibleGlassCounts_Throws(int bonus, int glass)
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => new Deck(new Random(1), bonus, glass));
    }

    [Fact]
    public void Deck_BonusAndGlassTogether_CanFillTheWholeDeck_WithoutOverlap()
    {
        Deck deck = new(new Random(3), bonusCardCount: 30, glassCardCount: 22);

        Assert.Equal(30, deck.Cards.OfType<BonusCard>().Count());
        Assert.Equal(22, deck.Cards.OfType<GlassCard>().Count());
        Assert.Empty(deck.Cards.Where(c => c is not BonusCard and not GlassCard));
    }

    [Fact]
    public void Deck_AddingGlassCards_DoesNotChangeWhichCardsAreBonusCards()
    {
        // Same seed: the bonus picks come first, so glass cards cannot move them.
        Deck onlyBonus = new(new Random(7), bonusCardCount: 8);
        Deck withGlass = new(new Random(7), bonusCardCount: 8, glassCardCount: 5);

        Assert.Equal(
            onlyBonus.Cards.Where(c => c is BonusCard).Select(c => c.ToString()),
            withGlass.Cards.Where(c => c is BonusCard).Select(c => c.ToString()));
    }

    [Fact]
    public void Deck_WithTheSameSeed_PicksTheSameGlassCards()
    {
        Deck first = new(new Random(9), glassCardCount: 8);
        Deck second = new(new Random(9), glassCardCount: 8);

        Assert.Equal(first.Cards.Select(c => c.ToString()), second.Cards.Select(c => c.ToString()));
    }

    [Fact]
    public void Round_WithAnAllGlassDeck_DoublesTheMultiplierOfAPlayedCard()
    {
        Deck deck = new(new Random(5), glassCardCount: 52);
        deck.Shuffle();
        Round round = new(deck);
        round.Hand.Select(0);

        ScoreResult result = round.PlayHand();   // one card: high card 5 x 1, doubled by the glass card

        Assert.Equal(HandType.HighCard, result.Combination.Type);
        Assert.Equal(2, result.Multiplier);
        Assert.Equal((5 + result.ScoringCards[0].Points) * 2, round.Score);
    }
}
