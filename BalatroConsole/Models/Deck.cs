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
    // bonusCardCount and glassCardCount swap that many normal cards for a BonusCard / GlassCard of the
    // same rank and suit, so the deck still has 52 unique cards. The defaults (0) give a plain deck.
    public Deck(Random? random = null, int bonusCardCount = 0, int glassCardCount = 0)
    {
        if (bonusCardCount < 0 || bonusCardCount > 52)
            throw new ArgumentOutOfRangeException(nameof(bonusCardCount), "A deck has 52 cards, so 0 to 52 can be bonus cards.");

        if (glassCardCount < 0 || bonusCardCount + glassCardCount > 52)
            throw new ArgumentOutOfRangeException(nameof(glassCardCount), "Bonus and glass cards together can't be more than the 52 cards of the deck.");

        _random = random ?? new Random();

        foreach (Suit suit in Enum.GetValues<Suit>())
        {
            foreach (Rank rank in Enum.GetValues<Rank>())
            {
                _cards.Add(new Card(rank, suit));
            }
        }

        MakeSpecialCards(bonusCardCount, glassCardCount);
    }

    // Picks different positions at random: the first 'bonusCount' become bonus cards and the next
    // 'glassCount' become glass cards, so no card is both. With 0 and 0 the Random is not used at all,
    // so plain decks shuffle exactly as before (and bonus-only decks keep the same picks as before).
    private void MakeSpecialCards(int bonusCount, int glassCount)
    {
        List<int> positions = Enumerable.Range(0, _cards.Count).ToList();

        for (int i = 0; i < bonusCount + glassCount; i++)
        {
            int pick = _random.Next(i, positions.Count);
            (positions[i], positions[pick]) = (positions[pick], positions[i]);

            ICard plain = _cards[positions[i]];
            _cards[positions[i]] = i < bonusCount
                ? new BonusCard(plain.Rank, plain.Suit)
                : new GlassCard(plain.Rank, plain.Suit);
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
