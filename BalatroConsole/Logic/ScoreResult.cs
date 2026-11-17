using BalatroConsole.Logic.Combinations;
using BalatroConsole.Models;

namespace BalatroConsole.Logic;

// The outcome of scoring one played hand.
public class ScoreResult
{
    public IHandCombination Combination { get; }

    // The cards that were part of the combination (the only ones that scored).
    public IReadOnlyList<ICard> ScoringCards { get; }

    public int Points { get; }
    public int Multiplier { get; }
    public int Total => Points * Multiplier;

    public ScoreResult(IHandCombination combination, IReadOnlyList<ICard> scoringCards, int points, int multiplier)
    {
        Combination = combination;
        ScoringCards = scoringCards;
        Points = points;
        Multiplier = multiplier;
    }
}
