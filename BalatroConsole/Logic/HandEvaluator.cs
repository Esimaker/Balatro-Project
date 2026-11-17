using BalatroConsole.Logic.Combinations;
using BalatroConsole.Models;

namespace BalatroConsole.Logic;

// Finds which combination the player played and works out the score.
public class HandEvaluator
{
    // Strongest first: a three of a kind also contains a pair,
    // so the first match in this list is always the best one.
    private readonly List<IHandCombination> _combinations = new()
    {
        new ThreeOfAKind(),
        new Pair(),
        new HighCard()
    };

    public ScoreResult Evaluate(IReadOnlyList<ICard> playedCards)
    {
        if (playedCards.Count == 0)
            throw new ArgumentException("Play at least one card.", nameof(playedCards));

        IHandCombination combination = _combinations.First(c => c.Matches(playedCards));

        // Step 6: every played card adds its points.
        // Step 7 will change this so only the cards in the combination count.
        int points = combination.BasePoints + playedCards.Sum(card => card.Points);

        return new ScoreResult(combination, points, combination.BaseMultiplier);
    }
}
