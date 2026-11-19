using System.Text;
using BalatroConsole.Logic;
using BalatroConsole.Models;

// Phase 1 smoke test: checks that the basic classes work together.
// This gets replaced by the real (MVVM) console UI later.
Console.OutputEncoding = Encoding.UTF8;

Deck deck = new(bonusCardCount: 5, glassCardCount: 5);   // bonus cards are marked with + (5♣+), glass cards with * (5♣*)
Console.WriteLine($"New deck: {deck.Cards.Count} cards");

deck.Shuffle();
Round round = new(deck);
Console.WriteLine($"Hand ({round.Hand.Count}): {string.Join(", ", round.Hand.Cards)}");
Console.WriteLine($"Cards left in deck: {deck.Cards.Count}");

round.Hand.Select(0);
round.Hand.Select(1);
round.Hand.Select(2);
Console.WriteLine($"Playing: {string.Join(", ", round.Hand.SelectedCards)}");

ScoreResult result = round.PlayHand();
Console.WriteLine($"{result.Combination.Name} (scoring: {string.Join(", ", result.ScoringCards)})");
Console.WriteLine($"{result.Points} points x {result.Multiplier} mult = {result.Total}");
Console.WriteLine($"Total score: {round.Score}");
Console.WriteLine($"Hand after refill ({round.Hand.Count}): {string.Join(", ", round.Hand.Cards)}");
Console.WriteLine($"Cards left in deck: {deck.Cards.Count}");
