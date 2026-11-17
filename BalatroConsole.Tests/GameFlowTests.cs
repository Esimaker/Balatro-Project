using BalatroConsole.Logic;
using BalatroConsole.Logic.Combinations;
using BalatroConsole.Models;

namespace BalatroConsole.Tests;

// Tests that several classes work together: Deck, Hand, Round and HandEvaluator.
public class GameFlowTests
{
    [Fact]
    public void PlayingAWholeDeck_UsesEveryCardExactlyOnce_AndTheScoresAddUp()
    {
        // Play 5 cards at a time until the hand is empty. Every card must be played once, none lost.
        Deck deck = new(new Random(2026));
        deck.Shuffle();
        Round round = new(deck);
        HashSet<ICard> played = new();
        int sumOfResults = 0;

        while (round.Hand.Count > 0)
        {
            int toSelect = Math.Min(5, round.Hand.Count);
            for (int i = 0; i < toSelect; i++)
                round.Hand.Select(i);
            List<ICard> selected = round.Hand.SelectedCards.ToList();

            ScoreResult result = round.PlayHand();

            sumOfResults += result.Total;
            foreach (ICard card in selected)
                Assert.True(played.Add(card), $"{card} was played twice");
            Assert.Equal(52, played.Count + round.Hand.Count + deck.Cards.Count);   // no card lost or doubled
            Assert.True(round.Hand.Count <= Round.HandSize);
        }

        Assert.Equal(52, played.Count);
        Assert.Empty(deck.Cards);
        Assert.Equal(sumOfResults, round.Score);
        Assert.True(round.Score > 0);
    }

    [Fact]
    public void TwoRoundsWithTheSameSeed_PlayExactlyTheSame()
    {
        int ScoreOf(int seed)
        {
            Deck deck = new(new Random(seed));
            deck.Shuffle();
            Round round = new(deck);
            for (int play = 0; play < 4; play++)
            {
                for (int i = 0; i < 5; i++) round.Hand.Select(i);
                round.PlayHand();
            }
            return round.Score;
        }

        Assert.Equal(ScoreOf(123), ScoreOf(123));
    }

    [Fact]
    public void Round_AcceptsACustomEvaluator()
    {
        // The evaluator is injected, so a round can score with only some combinations.
        HandEvaluator onlyHighCard = new(new List<IHandCombination> { new HighCard() });
        Deck deck = new(new Random(1));
        deck.Shuffle();
        Round round = new(deck, onlyHighCard);
        round.Hand.Select(0);
        round.Hand.Select(1);

        ScoreResult result = round.PlayHand();

        Assert.Equal(HandType.HighCard, result.Combination.Type);
    }

    // ----- HandCombinations (the list of combinations the game uses) -----

    [Fact]
    public void HandCombinations_HasEveryHandTypeExactlyOnce()
    {
        List<HandType> types = HandCombinations.CreateAll().Select(c => c.Type).ToList();

        Assert.Equal(Enum.GetValues<HandType>().OrderBy(t => t), types.OrderBy(t => t));
        Assert.Equal(types.Count, types.Distinct().Count());
    }

    [Fact]
    public void HandCombinations_HaveReadableUniqueNames_AndPositiveScores()
    {
        List<IHandCombination> all = HandCombinations.CreateAll();

        Assert.All(all, c => Assert.False(string.IsNullOrWhiteSpace(c.Name)));
        Assert.Equal(all.Count, all.Select(c => c.Name).Distinct().Count());
        Assert.All(all, c => Assert.True(c.BasePoints > 0 && c.BaseMultiplier > 0));
    }

    [Fact]
    public void HandCombinations_CreateAll_GivesNewObjectsEachTime()
    {
        Assert.NotSame(HandCombinations.CreateAll(), HandCombinations.CreateAll());
    }

    [Fact]
    public void HighCard_IsTheFallback_ForEveryNonEmptyHand()
    {
        Random random = new(3);
        for (int i = 0; i < 500; i++)
        {
            Deck deck = new(random);
            deck.Shuffle();
            List<ICard> played = deck.Draw(random.Next(1, 6));

            Assert.True(new HighCard().Matches(played));
        }
    }

    // ----- Deck -----

    [Fact]
    public void Deck_WithTheSameSeed_ShufflesInTheSameOrder()
    {
        Deck first = new(new Random(77));
        Deck second = new(new Random(77));

        first.Shuffle();
        second.Shuffle();

        Assert.Equal(first.Cards.Select(c => c.ToString()), second.Cards.Select(c => c.ToString()));
    }

    [Fact]
    public void Deck_WithDifferentSeeds_ShufflesDifferently()
    {
        Deck first = new(new Random(1));
        Deck second = new(new Random(2));

        first.Shuffle();
        second.Shuffle();

        Assert.NotEqual(first.Cards.Select(c => c.ToString()), second.Cards.Select(c => c.ToString()));
    }

    [Fact]
    public void Deck_AfterShuffleAndDraws_StillHas52DifferentCards()
    {
        Deck deck = new(new Random(8));
        deck.Shuffle();
        List<ICard> drawn = deck.Draw(10);
        deck.Shuffle();

        List<ICard> all = drawn.Concat(deck.Cards).ToList();

        Assert.Equal(52, all.Select(c => (c.Rank, c.Suit)).Distinct().Count());
        Assert.Equal(10, deck.DrawnCards.Count);
    }

    [Fact]
    public void Deck_HasThirteenCardsOfEverySuit_AndFourOfEveryRank()
    {
        Deck deck = new();

        foreach (Suit suit in Enum.GetValues<Suit>())
            Assert.Equal(13, deck.Cards.Count(c => c.Suit == suit));
        foreach (Rank rank in Enum.GetValues<Rank>())
            Assert.Equal(4, deck.Cards.Count(c => c.Rank == rank));
    }

    [Fact]
    public void Deck_DrawZero_ReturnsNothingAndChangesNothing()
    {
        Deck deck = new();

        List<ICard> drawn = deck.Draw(0);

        Assert.Empty(drawn);
        Assert.Equal(52, deck.Cards.Count);
    }

    // ----- Hand -----

    [Fact]
    public void Hand_IsSelected_ReflectsSelectAndDeselect()
    {
        Hand hand = new();
        hand.AddCards(TestHands.Parse("2H 3S 4C"));

        Assert.False(hand.IsSelected(1));
        hand.Select(1);
        Assert.True(hand.IsSelected(1));
        hand.Deselect(1);
        Assert.False(hand.IsSelected(1));
        Assert.False(hand.IsSelected(-1));
        Assert.False(hand.IsSelected(99));
    }

    [Fact]
    public void Hand_KeepsTheOrderOfSelection_AndRemoveSelectedReturnsThemInThatOrder()
    {
        Hand hand = new();
        hand.AddCards(TestHands.Parse("2H 3S 4C 5D 6H"));
        hand.Select(3);
        hand.Select(0);
        hand.Select(2);

        List<ICard> removed = hand.RemoveSelected();

        Assert.Equal(new[] { "5♦", "2♥", "4♣" }, removed.Select(c => c.ToString()));
        Assert.Equal(new[] { "3♠", "6♥" }, hand.Cards.Select(c => c.ToString()));
    }

    [Fact]
    public void Hand_AfterRemoveSelected_IndexesPointToTheRemainingCards()
    {
        Hand hand = new();
        hand.AddCards(TestHands.Parse("2H 3S 4C"));
        hand.Select(0);
        hand.RemoveSelected();

        Assert.True(hand.Select(1));       // the 4 of clubs is now at index 1
        Assert.False(hand.Select(2));      // there is no index 2 anymore
        Assert.Equal("4♣", hand.SelectedCards[0].ToString());
    }

    [Fact]
    public void Hand_DeselectingOneCard_FreesASlotWhenFiveAreSelected()
    {
        Hand hand = new();
        hand.AddCards(TestHands.Parse("2H 3S 4C 5D 6H 7S"));
        for (int i = 0; i < 5; i++) hand.Select(i);

        Assert.False(hand.Select(5));      // the sixth is refused
        hand.Deselect(0);
        Assert.True(hand.Select(5));       // a slot is free again
        Assert.Equal(5, hand.SelectedCards.Count);
    }

    // ----- ScoreResult -----

    [Fact]
    public void ScoreResult_Total_IsPointsTimesMultiplier()
    {
        ScoreResult result = new(new Pair(), TestHands.Parse("KS KD"), 30, 2);

        Assert.Equal(60, result.Total);
        Assert.Equal("Pair", result.Combination.Name);
        Assert.Equal(2, result.ScoringCards.Count);
    }
}
