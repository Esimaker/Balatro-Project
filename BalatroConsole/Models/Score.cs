namespace BalatroConsole.Models;

// Points and Multiplier together. Every card gets a Score and hands back a new one,
// so the evaluator never needs to know which kind of card it is scoring.
// A record is immutable: Add/Multiply never change this Score, they return a new one.
public record Score(int Points, int Multiplier)
{
    public int Total => Points * Multiplier;

    public Score AddPoints(int extraPoints) => this with { Points = Points + extraPoints };

    public Score MultiplyBy(int factor) => this with { Multiplier = Multiplier * factor };
}
