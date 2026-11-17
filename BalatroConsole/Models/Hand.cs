namespace BalatroConsole.Models;

// The cards the player is holding, plus which of them are selected.
public class Hand
{
    public const int MaxSelected = 5;

    private readonly List<ICard> _cards = new();
    private readonly List<ICard> _selected = new();

    public IReadOnlyList<ICard> Cards => _cards;
    public IReadOnlyList<ICard> SelectedCards => _selected;
    public int Count => _cards.Count;

    public void AddCards(IEnumerable<ICard> cards) => _cards.AddRange(cards);

    // Returns false if the index is invalid, the card is already selected, or 5 cards are already selected.
    public bool Select(int index)
    {
        if (index < 0 || index >= _cards.Count) return false;

        ICard card = _cards[index];
        if (_selected.Contains(card) || _selected.Count >= MaxSelected) return false;

        _selected.Add(card);
        return true;
    }

    public bool Deselect(int index)
    {
        if (index < 0 || index >= _cards.Count) return false;
        return _selected.Remove(_cards[index]);
    }

    public bool IsSelected(int index) =>
        index >= 0 && index < _cards.Count && _selected.Contains(_cards[index]);

    // Takes the selected cards out of the hand and gives them back to the caller.
    public List<ICard> RemoveSelected()
    {
        List<ICard> removed = new(_selected);
        foreach (ICard card in removed)
        {
            _cards.Remove(card);
        }
        _selected.Clear();
        return removed;
    }
}