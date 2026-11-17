using BalatroConsole.Models;

namespace BalatroConsole.Tests;

public class HandTests
{
    // Builds a hand of 8 different cards: 2-9 of hearts.
    private static Hand CreateHandOfEight()
    {
        Hand hand = new();
        hand.AddCards(new List<ICard>
        {
            new Card(Rank.Two, Suit.Hearts),
            new Card(Rank.Three, Suit.Hearts),
            new Card(Rank.Four, Suit.Hearts),
            new Card(Rank.Five, Suit.Hearts),
            new Card(Rank.Six, Suit.Hearts),
            new Card(Rank.Seven, Suit.Hearts),
            new Card(Rank.Eight, Suit.Hearts),
            new Card(Rank.Nine, Suit.Hearts)
        });
        return hand;
    }

    [Fact]
    public void AddCards_PutsTheCardsInTheHand()
    {
        Hand hand = CreateHandOfEight();

        Assert.Equal(8, hand.Count);
        Assert.Empty(hand.SelectedCards);
    }

    [Fact]
    public void Select_MarksTheCardAsSelected()
    {
        Hand hand = CreateHandOfEight();

        bool selected = hand.Select(2);

        Assert.True(selected);
        Assert.True(hand.IsSelected(2));
        Assert.Single(hand.SelectedCards);
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(8)]
    [InlineData(100)]
    public void Select_WithAnInvalidIndex_ReturnsFalse(int index)
    {
        Hand hand = CreateHandOfEight();

        Assert.False(hand.Select(index));
        Assert.Empty(hand.SelectedCards);
    }

    [Fact]
    public void Select_TheSameCardTwice_OnlySelectsItOnce()
    {
        Hand hand = CreateHandOfEight();

        hand.Select(0);
        bool secondTime = hand.Select(0);

        Assert.False(secondTime);
        Assert.Single(hand.SelectedCards);
    }

    [Fact]
    public void Select_SixthCard_IsRefused()
    {
        Hand hand = CreateHandOfEight();
        for (int i = 0; i < 5; i++)
            hand.Select(i);

        bool sixth = hand.Select(5);

        Assert.False(sixth);
        Assert.Equal(5, hand.SelectedCards.Count);
    }

    [Fact]
    public void Deselect_RemovesTheSelection_AndAllowsSelectingAgain()
    {
        Hand hand = CreateHandOfEight();
        hand.Select(1);

        bool deselected = hand.Deselect(1);

        Assert.True(deselected);
        Assert.False(hand.IsSelected(1));
        Assert.Empty(hand.SelectedCards);
        Assert.Equal(8, hand.Count);
    }

    [Fact]
    public void Deselect_ACardThatIsNotSelected_ReturnsFalse()
    {
        Hand hand = CreateHandOfEight();

        Assert.False(hand.Deselect(3));
        Assert.False(hand.Deselect(-1));
    }

    [Fact]
    public void RemoveSelected_ReturnsTheSelectedCards_AndTakesThemOutOfTheHand()
    {
        Hand hand = CreateHandOfEight();
        ICard first = hand.Cards[0];
        ICard third = hand.Cards[2];
        hand.Select(0);
        hand.Select(2);

        List<ICard> removed = hand.RemoveSelected();

        Assert.Equal(2, removed.Count);
        Assert.Contains(first, removed);
        Assert.Contains(third, removed);
        Assert.Equal(6, hand.Count);
        Assert.DoesNotContain(first, hand.Cards);
        Assert.DoesNotContain(third, hand.Cards);
    }

    [Fact]
    public void RemoveSelected_ClearsTheSelection()
    {
        Hand hand = CreateHandOfEight();
        hand.Select(0);

        hand.RemoveSelected();

        Assert.Empty(hand.SelectedCards);
    }

    [Fact]
    public void RemoveSelected_WithNothingSelected_ReturnsAnEmptyList_AndKeepsTheHand()
    {
        Hand hand = CreateHandOfEight();

        List<ICard> removed = hand.RemoveSelected();

        Assert.Empty(removed);
        Assert.Equal(8, hand.Count);
    }
}
