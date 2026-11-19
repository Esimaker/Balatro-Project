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

    public virtual int Points => PointsOf(Rank);

    // A normal card adds its points and leaves the multiplier alone.
    // BonusCard does not need to override this: it overrides Points, and this method uses it.
    public virtual Score Apply(Score current) => current.AddPoints(Points);

    public override string ToString() => SymbolOf(Rank, Suit);

    // Shared with other ICard classes that are not children of Card (like GlassCard),
    // so the point values and symbols are written down in one place only.
    internal static int PointsOf(Rank rank) => rank switch
    {
        Rank.Jack or Rank.Queen or Rank.King => 10,
        Rank.Ace => 11,
        _ => (int)rank
    };

    internal static string SymbolOf(Rank rank, Suit suit) => $"{RankSymbol(rank)}{SuitSymbol(suit)}";

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
