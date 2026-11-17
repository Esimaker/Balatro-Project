using BalatroConsole.Logic.Combinations;
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
        _evaluator = evaluator ?? new HandEvaluator(HandCombinations.CreateAll());
        FillHand();
    }

    // Removes the selected cards from the hand, scores them, then refills the hand.
    // Returns the result of this play so the UI can show what happened.
    public ScoreResult PlayHand()
    {
        if (Hand.SelectedCards.Count == 0)
            throw new InvalidOperationException("Select at least one card before playing.");

        List<ICard> played = Hand.RemoveSelected();
        ScoreResult result = _evaluator.Evaluate(played);
        Score += result.Total;
        FillHand();
        return result;
    }

    // The round is in charge of never letting the hand go over 8 cards.
    // Near the end the deck may have fewer cards than the hand is missing, so only ask for what is left
    // (Deck.Deal throws if you ask for more).
    private void FillHand()
    {
        int missing = HandSize - Hand.Count;
        int toDeal = Math.Min(missing, _deck.Cards.Count);
        if (toDeal > 0)
            Hand.AddCards(_deck.Deal(toDeal));
    }
}
