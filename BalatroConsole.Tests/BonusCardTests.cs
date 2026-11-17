using BalatroConsole.Logic;
using BalatroConsole.Logic.Combinations;
using BalatroConsole.Models;

namespace BalatroConsole.Tests;

public class BonusCardTests
{
    private readonly HandEvaluator _evaluator = new(HandCombinations.CreateAll());

    // ----- The card itself -----

    [Fact]
    public void FiveOfClubs_AsBonusCard_IsWorth15_InsteadOf5()
    {
        // The example from the assignment.
        Card normal = new(Rank.Five, Suit.Clubs);
        BonusCard bonus = new(Rank.Five, Suit.Clubs);

        Assert.Equal(5, normal.Points);
        Assert.Equal(15, bonus.Points);
    }

    [Theory]
    [InlineData(Rank.Two, 12)]
    [InlineData(Rank.Ten, 20)]
    [InlineData(Rank.Jack, 20)]
    [InlineData(Rank.King, 20)]
    [InlineData(Rank.Ace, 21)]
    public void BonusCard_IsAlwaysTenPointsMoreThanTheNormalCard(Rank rank, int expectedPoints)
    {
        Assert.Equal(expectedPoints, new BonusCard(rank, Suit.Hearts).Points);
        Assert.Equal(new Card(rank, Suit.Hearts).Points + BonusCard.ExtraPoints, new BonusCard(rank, Suit.Hearts).Points);
    }

    [Fact]
    public void BonusCard_KeepsItsRankAndSuit_AndIsAnICardAndACard()
    {
        BonusCard bonus = new(Rank.Queen, Suit.Spades);

        Assert.Equal(Rank.Queen, bonus.Rank);
        Assert.Equal(Suit.Spades, bonus.Suit);
        Assert.IsAssignableFrom<ICard>(bonus);
        Assert.IsAssignableFrom<Card>(bonus);
    }

    [Fact]
    public void BonusCard_ShowsAPlus_InTheConsole()
    {
        Assert.Equal("5♣+", new BonusCard(Rank.Five, Suit.Clubs).ToString());
        Assert.Equal("A♠+", new BonusCard(Rank.Ace, Suit.Spades).ToString());
        Assert.Equal("5♣", new Card(Rank.Five, Suit.Clubs).ToString());   // a normal card has no plus
    }

    // ----- Scoring with bonus cards (the evaluator does not know about bonus cards) -----

    [Fact]
    public void Pair_WithOneBonusCard_ScoresTenPointsMore()
    {
        List<ICard> normalPair = TestHands.Of((Rank.Five, Suit.Hearts), (Rank.Five, Suit.Spades));
        List<ICard> withBonus = new() { new BonusCard(Rank.Five, Suit.Hearts), new Card(Rank.Five, Suit.Spades) };

        ScoreResult normal = _evaluator.Evaluate(normalPair);
        ScoreResult bonus = _evaluator.Evaluate(withBonus);

        Assert.Equal(20, normal.Points);     // 10 base + 5 + 5
        Assert.Equal(30, bonus.Points);      // 10 base + 15 + 5
        Assert.Equal(60, bonus.Total);       // 30 x 2
    }

    [Fact]
    public void BonusCard_InTheCombination_ScoresItsBonus_ForEveryBonusCard()
    {
        List<ICard> bothBonus = new() { new BonusCard(Rank.Five, Suit.Hearts), new BonusCard(Rank.Five, Suit.Spades) };

        ScoreResult result = _evaluator.Evaluate(bothBonus);

        Assert.Equal(40, result.Points);     // 10 base + 15 + 15
        Assert.Equal(80, result.Total);
    }

    [Fact]
    public void BonusCard_OutsideTheCombination_AddsNothing()
    {
        // The bonus only counts "als hij gescored wordt". The 9 is played, but it is not part of the pair.
        List<ICard> hand = new()
        {
            new Card(Rank.King, Suit.Spades), new Card(Rank.King, Suit.Hearts),
            new Card(Rank.Three, Suit.Clubs), new BonusCard(Rank.Nine, Suit.Diamonds)
        };

        ScoreResult result = _evaluator.Evaluate(hand);

        Assert.Equal(HandType.Pair, result.Combination.Type);
        Assert.Equal(30, result.Points);     // 10 base + 10 + 10, the bonus 9 does not count
    }

    [Fact]
    public void HighCard_BonusCard_ScoresWhenItIsTheHighestCard()
    {
        List<ICard> hand = new() { new Card(Rank.Two, Suit.Hearts), new BonusCard(Rank.Ace, Suit.Spades) };

        ScoreResult result = _evaluator.Evaluate(hand);

        Assert.Equal(26, result.Points);     // 5 base + 11 + 10
    }

    [Fact]
    public void Straight_WithABonusCard_StillIsAStraight_AndScoresTheBonus()
    {
        List<ICard> hand = new()
        {
            new Card(Rank.Five, Suit.Hearts), new Card(Rank.Six, Suit.Spades), new BonusCard(Rank.Seven, Suit.Clubs),
            new Card(Rank.Eight, Suit.Diamonds), new Card(Rank.Nine, Suit.Hearts)
        };

        ScoreResult result = _evaluator.Evaluate(hand);

        Assert.Equal(HandType.Straight, result.Combination.Type);
        Assert.Equal(80, result.Points);     // 35 base + 35 + 10 bonus
    }

    [Fact]
    public void TurningEveryCardIntoABonusCard_AddsExactlyTenPerScoringCard_AndKeepsTheCombination()
    {
        // The strongest check: for thousands of random hands, a bonus version of the same hand
        // must pick the same combination and score exactly 10 points more per scoring card.
        Random random = new(404);

        for (int i = 0; i < 5_000; i++)
        {
            Deck deck = new(random);
            deck.Shuffle();
            List<ICard> plain = deck.Deal(random.Next(1, 6));
            List<ICard> bonus = plain.Select(c => (ICard)new BonusCard(c.Rank, c.Suit)).ToList();

            ScoreResult plainResult = _evaluator.Evaluate(plain);
            ScoreResult bonusResult = _evaluator.Evaluate(bonus);

            Assert.Equal(plainResult.Combination.Type, bonusResult.Combination.Type);
            Assert.Equal(plainResult.ScoringCards.Count, bonusResult.ScoringCards.Count);
            Assert.Equal(plainResult.Points + 10 * plainResult.ScoringCards.Count, bonusResult.Points);
        }
    }

    // ----- Bonus cards in the deck -----

    [Fact]
    public void Deck_WithFiveBonusCards_HasFiveBonusCards_AndStill52UniqueCards()
    {
        Deck deck = new(new Random(1), bonusCardCount: 5);

        Assert.Equal(5, deck.Cards.OfType<BonusCard>().Count());
        Assert.Equal(52, deck.Cards.Count);
        Assert.Equal(52, deck.Cards.Select(c => (c.Rank, c.Suit)).Distinct().Count());
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(26)]
    [InlineData(52)]
    public void Deck_BonusCardCount_IsExact(int count)
    {
        Deck deck = new(new Random(2), count);

        Assert.Equal(count, deck.Cards.OfType<BonusCard>().Count());
        Assert.Equal(52, deck.Cards.Count);
    }

    [Fact]
    public void Deck_ByDefault_HasNoBonusCards()
    {
        Assert.Empty(new Deck().Cards.OfType<BonusCard>());
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(53)]
    public void Deck_WithAnImpossibleBonusCardCount_Throws(int count)
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => new Deck(new Random(1), count));
    }

    [Fact]
    public void Deck_WithTheSameSeed_PicksTheSameBonusCards()
    {
        Deck first = new(new Random(9), bonusCardCount: 8);
        Deck second = new(new Random(9), bonusCardCount: 8);

        Assert.Equal(first.Cards.Select(c => c.ToString()), second.Cards.Select(c => c.ToString()));
    }

    [Fact]
    public void Deck_WithoutBonusCards_ShufflesExactlyAsBefore()
    {
        // A count of 0 must not use the Random, or every seeded test would change its shuffle.
        Deck withParameter = new(new Random(42), bonusCardCount: 0);
        Deck withoutParameter = new(new Random(42));

        withParameter.Shuffle();
        withoutParameter.Shuffle();

        Assert.Equal(withoutParameter.Cards.Select(c => c.ToString()), withParameter.Cards.Select(c => c.ToString()));
    }

    [Fact]
    public void BonusCards_SurviveShuffleAndDeal()
    {
        Deck deck = new(new Random(3), bonusCardCount: 10);
        deck.Shuffle();

        List<ICard> dealt = deck.Deal(20);

        int bonusInTotal = dealt.OfType<BonusCard>().Count() + deck.Cards.OfType<BonusCard>().Count();
        Assert.Equal(10, bonusInTotal);
    }

    [Fact]
    public void Round_WithAnAllBonusDeck_ScoresTheBonusOnThePlayedPair()
    {
        // Every card is a bonus card, so any played card is worth 10 more than normal.
        Deck deck = new(new Random(5), bonusCardCount: 52);
        deck.Shuffle();
        Round round = new(deck);
        round.Hand.Select(0);

        ScoreResult result = round.PlayHand();   // one card: high card

        Assert.Equal(HandType.HighCard, result.Combination.Type);
        Assert.Equal(5 + result.ScoringCards[0].Points, result.Points);
        Assert.True(result.ScoringCards[0].Points >= 12);   // 2 + 10 is the lowest possible
    }
}
