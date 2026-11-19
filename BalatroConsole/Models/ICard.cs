namespace BalatroConsole.Models;

// Every card (normal, bonus, glass, ...) implements this, so the rest of the game
// never has to know which kind of card it is dealing with.
public interface ICard
{
    Rank Rank { get; }
    Suit Suit { get; }

    // How many points this card adds when it scores (2-10 = face value, J/Q/K = 10, A = 11).
    int Points { get; }

    // What this card does to the score when it scores: takes the current Score and returns a new one.
    // A normal card adds its Points. A special card can also change the multiplier.
    Score Apply(Score current);
}
