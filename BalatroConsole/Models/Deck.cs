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

    // Draws up to 'count' cards from the top. Returns fewer if the deck runs out.
    public List<ICard> Draw(int count)
    {
        if (count < 0)
            throw new ArgumentOutOfRangeException(nameof(count), "Can't draw a negative number of cards.");

        int amount = Math.Min(count, _cards.Count);
        List<ICard> drawn = _cards.GetRange(0, amount);
        _cards.RemoveRange(0, amount);
        _drawnCards.AddRange(drawn);
        return drawn;
    }
}
