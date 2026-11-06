using BalatroConsole.Models;

namespace BalatroConsole.Logic;

// Phase 1, step 4: just adds up the points of the played cards.
// Step 6 adds combinations (high card, pair, three of a kind) with points + multiplier,
// step 7 filters the played cards down to only the ones that are part of the combination.
public class HandEvaluator
{
    public int Evaluate(IReadOnlyList<ICard> playedCards)
    {
        return playedCards.Sum(card => card.Points);
    }
}
