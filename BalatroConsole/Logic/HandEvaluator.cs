using BalatroConsole.Logic.Combinations;
using BalatroConsole.Models;

namespace BalatroConsole.Logic;

// Finds which combination the player played. The score itself is worked out by the ScoreCalculator.
// The combinations are passed in (dependency injection), so this class only knows the
// IHandCombination interface and not a single concrete combination.
public class HandEvaluator
{
    private readonly List<IHandCombination> _combinations;
    private readonly ScoreCalculator _calculator = new();

    public HandEvaluator(IEnumerable<IHandCombination> combinations)
    {
        // Strongest first, decided here by HandType and not by the order the caller used:
        // three of a kind also contains a pair, so the first match is always the best one.
        _combinations = combinations.OrderByDescending(c => c.Type).ToList();

        if (_combinations.Count == 0)
            throw new ArgumentException("At least one combination is needed.", nameof(combinations));
    }

    public ScoreResult Evaluate(IReadOnlyList<ICard> playedCards)
    {
        if (playedCards.Count == 0)
            throw new ArgumentException("Play at least one card.", nameof(playedCards));

        IHandCombination combination = _combinations.First(c => c.Matches(playedCards));

        // Only the cards in the combination score. The rest of the played cards are ignored.
        IReadOnlyList<ICard> scoringCards = combination.GetScoringCards(playedCards);
        Score score = _calculator.Calculate(combination, scoringCards);

        return new ScoreResult(combination, scoringCards, score.Points, score.Multiplier);
    }
}
