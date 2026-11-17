# Balatro Clone (C#)

Een Balatro-achtige kaartgame in C# (console, xUnit-tests). Je krijgt 8 kaarten, speelt er 1 tot 5
en krijgt punten volgens de pokercombinatie die je speelt.

## Draaien

.NET 10 SDK nodig. Open `BalatroConsole.slnx` in Visual Studio, of:

```
dotnet run --project BalatroConsole
dotnet test
```

(De console-UI is nog een korte test in `Program.cs`. De echte UI komt in fase 2C.)

## Scoreregels

Score = `(basispunten + kaartpunten) x multiplier`. Alleen de kaarten die in de combinatie zitten
tellen mee, de andere gespeelde kaarten niet. Er wordt maar een combinatie gescoord: de sterkste.

Kaartpunten: 2 tot 10 = het getal, J / Q / K = 10, Aas = 11.

| Combinatie | Basispunten | Multiplier |
|---|---|---|
| High card (alleen de hoogste kaart scoort) | 5 | x1 |
| Pair | 10 | x2 |
| Two pair | 20 | x2 |
| Three of a kind | 30 | x3 |
| Straight (Aas hoog of laag) | 35 | x4 |
| Flush (een straight flush telt als Flush) | 40 | x4 |
| Full House | 50 | x4 |
| Four of a kind | 60 | x7 |

Voorbeeld: `K K 3 7 9` is een Pair. Alleen de twee koningen scoren: `(10 + 10 + 10) x 2 = 60`.

## Gekozen uitbreidingen

Gedaan: Two pair, Four of a kind, Full House, Straight, Flush.

Gepland (staan op het Trello-bord): BonusCard, GlassCard, Wildcard, MultCard, ChanceCard,
stickers via het Decorator-patroon en een Windows Forms-interface.

Het klassendiagram en de flowcharts staan in [docs](docs).
