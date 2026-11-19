using BalatroConsole.Logic.Combinations;
using BalatroConsole.Models;

namespace BalatroConsole.Logic;

// Works out the Score of a combination. It starts from the base score of the combination and lets
// every scoring card change it, in the order they were played. It only uses ICard.Apply, so it
// never needs to know about BonusCard, GlassCard or any card that gets added later.
public class ScoreCalculator
{
    public Score Calculate(IHandCombination combination, IReadOnlyList<ICard> scoringCards)
    {
        Score score = new(combination.BasePoints, combination.BaseMultiplier);

        foreach (ICard card in scoringCards)
        {
            score = card.Apply(score);
        }

        return score;
    }
}
