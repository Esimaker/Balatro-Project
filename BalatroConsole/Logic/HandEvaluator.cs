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
        new FourOfAKind(),
        new ThreeOfAKind(),
        new TwoPair(),
        new Pair(),
        new HighCard()
    };

    public ScoreResult Evaluate(IReadOnlyList<ICard> playedCards)
    {
        if (playedCards.Count == 0)
            throw new ArgumentException("Play at least one card.", nameof(playedCards));

        IHandCombination combination = _combinations.First(c => c.Matches(playedCards));

        // Only the cards in the combination score. The rest of the played cards are ignored.
        IReadOnlyList<ICard> scoringCards = combination.GetScoringCards(playedCards);
        int points = combination.BasePoints + scoringCards.Sum(card => card.Points);

        return new ScoreResult(combination, scoringCards, points, combination.BaseMultiplier);
    }
}
