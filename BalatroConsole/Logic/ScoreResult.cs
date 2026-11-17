using BalatroConsole.Logic.Combinations;

namespace BalatroConsole.Logic;

// The outcome of scoring one played hand.
public class ScoreResult
{
    public IHandCombination Combination { get; }
    public int Points { get; }
    public int Multiplier { get; }
    public int Total => Points * Multiplier;

    public ScoreResult(IHandCombination combination, int points, int multiplier)
    {
        Combination = combination;
        Points = points;
        Multiplier = multiplier;
    }
}
