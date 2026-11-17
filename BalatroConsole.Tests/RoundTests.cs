using BalatroConsole.Logic;
using BalatroConsole.Models;

namespace BalatroConsole.Tests;

// These tests check counts and consistency, not specific poker hands,
// so they don't break when more combinations are added later.
public class RoundTests
{
    private static Round CreateRound(out Deck deck)
    {
        deck = new Deck(new Random(7));
        deck.Shuffle();
        return new Round(deck);
    }

    private static void SelectFirstCards(Round round, int count)
    {
        for (int i = 0; i < count; i++)
            round.Hand.Select(i);
    }

    [Fact]
    public void NewRound_DealsEightCards_AndTakesThemFromTheDeck()
    {
        Round round = CreateRound(out Deck deck);

        Assert.Equal(8, round.Hand.Count);
        Assert.Equal(44, deck.Cards.Count);
        Assert.Equal(0, round.Score);
    }

    [Fact]
    public void PlayHand_RefillsTheHandToEight()
    {
        Round round = CreateRound(out Deck deck);
        SelectFirstCards(round, 3);

        round.PlayHand();

        Assert.Equal(8, round.Hand.Count);
        Assert.Equal(41, deck.Cards.Count);
    }

    [Fact]
    public void PlayHand_AddsTheTotalOfTheResultToTheScore()
    {
        Round round = CreateRound(out _);
        SelectFirstCards(round, 2);

        ScoreResult result = round.PlayHand();

        Assert.True(result.Total > 0);
        Assert.Equal(result.Total, round.Score);
    }

    [Fact]
    public void PlayHand_TwiceInARow_AddsBothResultsToTheScore()
    {
        Round round = CreateRound(out _);
        SelectFirstCards(round, 2);
        ScoreResult first = round.PlayHand();
        SelectFirstCards(round, 3);
        ScoreResult second = round.PlayHand();

        Assert.Equal(first.Total + second.Total, round.Score);
    }

    [Fact]
    public void PlayHand_PlayedCardsAreNotInTheHandAnymore()
    {
        Round round = CreateRound(out _);
        SelectFirstCards(round, 4);
        List<ICard> selected = round.Hand.SelectedCards.ToList();

        round.PlayHand();

        Assert.All(selected, card => Assert.DoesNotContain(card, round.Hand.Cards));
        Assert.Empty(round.Hand.SelectedCards);
    }

    [Fact]
    public void PlayHand_WithNothingSelected_Throws()
    {
        Round round = CreateRound(out _);

        Assert.Throws<InvalidOperationException>(() => round.PlayHand());
        Assert.Equal(8, round.Hand.Count);
    }

    [Fact]
    public void PlayHand_NeverLeavesMoreThanEightCardsInTheHand()
    {
        Round round = CreateRound(out _);

        for (int play = 0; play < 9; play++)
        {
            SelectFirstCards(round, 5);
            round.PlayHand();
            Assert.True(round.Hand.Count <= Round.HandSize);
        }
    }

    [Fact]
    public void PlayHand_WhenTheDeckRunsOut_TheHandStaysSmaller_AndNoCardIsLost()
    {
        Round round = CreateRound(out Deck deck);

        // 8 plays use 40 of the 44 cards left in the deck, the 9th play only finds 4.
        for (int play = 0; play < 9; play++)
        {
            SelectFirstCards(round, 5);
            round.PlayHand();
        }

        Assert.Empty(deck.Cards);
        Assert.Equal(7, round.Hand.Count);   // 8 - 5 played + 4 drawn
    }
}
