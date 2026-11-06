namespace BalatroConsole.Models;

public class Card : ICard
{
    public Rank Rank { get; }
    public Suit Suit { get; }

    public Card(Rank rank, Suit suit)
    {
        Rank = rank;
        Suit = suit;
    }

    public virtual int Points => Rank switch
    {
        Rank.Jack or Rank.Queen or Rank.King => 10,
        Rank.Ace => 11,
        _ => (int)Rank
    };

    public override string ToString() => $"{RankSymbol(Rank)}{SuitSymbol(Suit)}";

    private static string RankSymbol(Rank rank) => rank switch
    {
        Rank.Jack => "J",
        Rank.Queen => "Q",
        Rank.King => "K",
        Rank.Ace => "A",
        _ => ((int)rank).ToString()
    };

    private static string SuitSymbol(Suit suit) => suit switch
    {
        Suit.Hearts => "♥",
        Suit.Diamonds => "♦",
        Suit.Clubs => "♣",
        Suit.Spades => "♠",
        _ => "?"
    };
}
