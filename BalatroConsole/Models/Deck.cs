namespace BalatroConsole.Models;

// Keeps track of which cards can still be drawn and which have already been drawn.
public class Deck
{
    private readonly List<ICard> _cards = new();
    private readonly List<ICard> _drawnCards = new();
    private readonly Random _random;

    public IReadOnlyList<ICard> Cards => _cards;
    public IReadOnlyList<ICard> DrawnCards => _drawnCards;

    // Pass in a Random with a fixed seed in tests to get the same shuffle every time.
    public Deck(Random? random = null)
    {
        _random = random ?? new Random();

        foreach (Suit suit in Enum.GetValues<Suit>())
        {
            foreach (Rank rank in Enum.GetValues<Rank>())
            {
                _cards.Add(new Card(rank, suit));
            }
        }
    }

    public void Shuffle()
    {
        // Fisher-Yates shuffle
        for (int i = _cards.Count - 1; i > 0; i--)
        {
            int j = _random.Next(i + 1);
            (_cards[i], _cards[j]) = (_cards[j], _cards[i]);
        }
    }

    // Deals 'count' cards from the top of the deck and takes them out of it.
    // Asking for more cards than are left is a mistake by the caller, so it throws a clear exception.
    public List<ICard> Deal(int count)
    {
        if (count < 0)
            throw new ArgumentOutOfRangeException(nameof(count), "Can't deal a negative number of cards.");

        if (count > _cards.Count)
            throw new InvalidOperationException($"Can't deal {count} cards, only {_cards.Count} are left in the deck.");

        List<ICard> dealt = _cards.GetRange(0, count);
        _cards.RemoveRange(0, count);
        _drawnCards.AddRange(dealt);
        return dealt;
    }
}
