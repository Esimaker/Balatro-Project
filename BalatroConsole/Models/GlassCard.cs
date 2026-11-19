namespace BalatroConsole.Models;

// A card that doubles the multiplier when it scores.
// It implements ICard directly instead of inheriting from Card: the evaluator only knows ICard,
// so this proves a new kind of card does not need to be a child of Card.
// (Breaking a glass card, like in the real game, is a could-have and not built yet.)
public class GlassCard : ICard
{
    public const int MultiplierFactor = 2;

    public Rank Rank { get; }
    public Suit Suit { get; }

    public GlassCard(Rank rank, Suit suit)
    {
        Rank = rank;
        Suit = suit;
    }

    public int Points => Card.PointsOf(Rank);

    public Score Apply(Score current) => current.AddPoints(Points).MultiplyBy(MultiplierFactor);

    public override string ToString() => Card.SymbolOf(Rank, Suit) + "*";
}
