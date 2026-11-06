using BalatroConsole.Models;

namespace BalatroConsole.Logic;

public class Round
{
    public const int HandSize = 8;

    private readonly Deck _deck;
    private readonly HandEvaluator _evaluator;

    public Hand Hand { get; } = new();
    public int Score { get; private set; }

    public Round(Deck deck, HandEvaluator? evaluator = null)
    {
        _deck = deck;
        _evaluator = evaluator ?? new HandEvaluator();
        FillHand();
    }

    // Removes the selected cards from the hand, scores them, then refills the hand.
    // Returns the points earned by this play.
    public int PlayHand()
    {
        if (Hand.SelectedCards.Count == 0)
            throw new InvalidOperationException("Select at least one card before playing.");

        List<ICard> played = Hand.RemoveSelected();
        int points = _evaluator.Evaluate(played);
        Score += points;
        FillHand();
        return points;
    }

    // The round is in charge of never letting the hand go over 8 cards.
    private void FillHand()
    {
        int missing = HandSize - Hand.Count;
        if (missing > 0)
            Hand.AddCards(_deck.Draw(missing));
    }
}
