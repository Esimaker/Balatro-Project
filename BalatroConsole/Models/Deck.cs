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
    // bonusCardCount swaps that many normal cards for a BonusCard of the same rank and suit,
    // so the deck still has 52 unique cards. The default (0) gives a plain deck.
    public Deck(Random? random = null, int bonusCardCount = 0)
    {
        if (bonusCardCount < 0 || bonusCardCount > 52)
            throw new ArgumentOutOfRangeException(nameof(bonusCardCount), "A deck has 52 cards, so 0 to 52 can be bonus cards.");

        _random = random ?? new Random();

        foreach (Suit suit in Enum.GetValues<Suit>())
        {
            foreach (Rank rank in Enum.GetValues<Rank>())
            {
                _cards.Add(new Card(rank, suit));
            }
        }

        MakeBonusCards(bonusCardCount);
    }

    // Picks 'count' different positions at random and replaces those cards by bonus versions.
    // With a count of 0 the Random is not used at all, so plain decks shuffle exactly as before.
    private void MakeBonusCards(int count)
    {
        List<int> positions = Enumerable.Range(0, _cards.Count).ToList();

        for (int i = 0; i < count; i++)
        {
            int pick = _random.Next(i, positions.Count);
            (positions[i], positions[pick]) = (positions[pick], positions[i]);

            ICard plain = _cards[positions[i]];
            _cards[positions[i]] = new BonusCard(plain.Rank, plain.Suit);
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

    // Draws 'count' cards from the top of the deck and takes them out of it.
    // Asking for more cards than are left is a mistake by the caller, so it throws a clear exception.
    public List<ICard> Draw(int count)
    {
        if (count < 0)
            throw new ArgumentOutOfRangeException(nameof(count), "Can't draw a negative number of cards.");

        if (count > _cards.Count)
            throw new InvalidOperationException($"Can't draw {count} cards, only {_cards.Count} are left in the deck.");

        List<ICard> drawn = _cards.GetRange(0, count);
        _cards.RemoveRange(0, count);
        _drawnCards.AddRange(drawn);
        return drawn;
    }
}
