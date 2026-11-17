namespace BalatroConsole.Models;

// A normal card that is worth 10 points more when it scores (a 5 of clubs scores 15 instead of 5).
// It is a child class of Card and only overrides Points. Everything else (rank, suit, immutability) is inherited.
public class BonusCard : Card
{
    public const int ExtraPoints = 10;

    public BonusCard(Rank rank, Suit suit) : base(rank, suit)
    {
    }

    public override int Points => base.Points + ExtraPoints;

    // A "+" marks a bonus card in the console, e.g. 5♣+
    public override string ToString() => base.ToString() + "+";
}
