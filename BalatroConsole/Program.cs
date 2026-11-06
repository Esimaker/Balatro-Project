using System.Text;
using BalatroConsole.Logic;
using BalatroConsole.Models;

// Phase 1 smoke test: checks that the basic classes work together.
// This gets replaced by the real (MVVM) console UI later.
Console.OutputEncoding = Encoding.UTF8;

Deck deck = new();
Console.WriteLine($"New deck: {deck.Cards.Count} cards");

deck.Shuffle();
Round round = new(deck);
Console.WriteLine($"Hand ({round.Hand.Count}): {string.Join(", ", round.Hand.Cards)}");
Console.WriteLine($"Cards left in deck: {deck.Cards.Count}");

round.Hand.Select(0);
round.Hand.Select(1);
round.Hand.Select(2);
Console.WriteLine($"Playing: {string.Join(", ", round.Hand.SelectedCards)}");

int points = round.PlayHand();
Console.WriteLine($"Points this hand: {points}, total score: {round.Score}");
Console.WriteLine($"Hand after refill ({round.Hand.Count}): {string.Join(", ", round.Hand.Cards)}");
Console.WriteLine($"Cards left in deck: {deck.Cards.Count}");
