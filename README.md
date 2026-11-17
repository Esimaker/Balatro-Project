# Balatro Clone (C#)

Een Balatro-achtige kaartgame in C#, gemaakt als individueel schoolproject (Techniek College
Rotterdam, leerjaar 2). Je krijgt een hand van 8 kaarten, selecteert er maximaal 5, speelt ze en
krijgt punten volgens de pokercombinatie die je gespeeld hebt.

> **Status:** fase 1 (basis) en fase 2A (combinaties) zijn klaar. De console-UI is nog een
> korte test in `Program.cs`. De echte (MVVM) console-UI komt in fase 2C. Zie [Voortgang](#voortgang).

## Draaien

Je hebt de [.NET 10 SDK](https://dotnet.microsoft.com/download) nodig.

```
dotnet build
dotnet test                      # alle unit tests
dotnet run --project BalatroConsole
```

In Visual Studio: open `BalatroConsole.slnx` en gebruik **Test > Run All Tests**.

> Op sommige computers blokkeert Windows Application Control een net gebouwd bestand. De
> tests kunnen dan een keer allemaal falen met foutcode `0x800711C7`. Bouw opnieuw en draai ze nog
> een keer. Blokkeert hij `dotnet run`, start dan met `dotnet BalatroConsole/bin/Debug/net10.0/BalatroConsole.dll`.

## Spelregels en scoring

1. Je krijgt 8 kaarten uit een geschud deck van 52 kaarten.
2. Selecteer 1 tot 5 kaarten en speel ze.
3. Het spel zoekt de **sterkste combinatie** in je gespeelde kaarten. Er wordt maar een combinatie gescoord.
4. Alleen de kaarten die **in de combinatie zitten** tellen mee. De andere gespeelde kaarten scoren niet.
5. Score = `(basispunten + kaartpunten van de scorende kaarten) x multiplier`.
6. De hand wordt weer aangevuld tot 8 kaarten (zolang het deck genoeg kaarten heeft).

**Kaartpunten:** 2 tot 10 = het getal, Boer/Vrouw/Heer (J, Q, K) = 10, Aas = 11.

| Combinatie | Wat is het | Basispunten | Multiplier |
|---|---|---|---|
| High card | Geen combinatie. Alleen de hoogste kaart scoort | 5 | x1 |
| Pair | Precies 2 kaarten met dezelfde rank | 10 | x2 |
| Two pair | Twee verschillende ranks, elk precies 2 kaarten | 20 | x2 |
| Three of a kind | Precies 3 kaarten met dezelfde rank | 30 | x3 |
| Straight | 5 kaarten met opeenvolgende ranks (Aas telt hoog en laag, geen rondje Q K A 2 3) | 35 | x4 |
| Flush | 5 kaarten van dezelfde suit | 40 | x4 |
| Full House | Precies 3 kaarten van een rank plus precies 2 van een andere rank | 50 | x4 |
| Four of a kind | 4 kaarten met dezelfde rank | 60 | x7 |

Een straight flush (5 opeenvolgende kaarten van een suit) telt als Flush, omdat Flush sterker is dan Straight.
De basispunten stijgen steeds, dus een sterkere combinatie levert altijd meer basispunten op.

Voorbeeld: `K K 3 7 9` is een Pair. Alleen de twee koningen scoren: `(10 + 10 + 10) x 2 = 60`.
De 3, 7 en 9 doen niet mee.

## Projectopbouw

```
BalatroConsole/
  Models/              Card, Deck, Hand, ICard, Suit, Rank
  Logic/               Round, HandEvaluator, ScoreResult
  Logic/Combinations/  IHandCombination, OfAKindCombination, HighCard, Pair, TwoPair,
                       ThreeOfAKind, Straight, Flush, FullHouse, FourOfAKind, HandType, HandCombinations
  Program.cs           (tijdelijke) test van de basisklassen
BalatroConsole.Tests/  xUnit tests
docs/                  klassendiagram en flowcharts
```

`Models` en `Logic` weten niets van de UI. Zo is de logica los te testen en kan dezelfde logica
later onder een Windows Forms-UI.

## OOP-keuzes

- **Interface:** `IHandCombination` (elke combinatie), `ICard` (elke kaart). De `HandEvaluator` kent alleen het interface.
- **Abstracte klasse:** `OfAKindCombination` bevat de gedeelde code voor Pair, Three of a kind en Four of a kind.
- **Polymorfisme:** de evaluator loopt door een lijst `IHandCombination` (van sterk naar zwak) zonder `switch`.
- **Dependency injection:** de lijst combinaties wordt in `HandEvaluator` gestopt. `HandCombinations.CreateAll()` is de enige plek die de concrete klassen noemt.
- **Encapsulatie:** `Card` is onveranderlijk, `Deck` en `Hand` geven alleen `IReadOnlyList` terug, `ScoreResult` is onveranderlijk.
- **Testbare willekeur:** `Deck` krijgt optioneel een `Random`. Tests gebruiken een vaste seed, dus geven elke keer hetzelfde resultaat.

Het klassendiagram en de flowcharts staan in [docs](docs):
[klassendiagram](docs/klassendiagram.md), [flowcharts](docs/flowcharts.md).

## Tests

`dotnet test` draait alle xUnit-tests (arrange, act, assert). Ze dekken de kaart, het deck
(52 unieke kaarten, `Deal`, een fout bij te veel kaarten), de hand (selecteren, maximaal 5), de ronde,
elke combinatie los (ook de gevallen waarin die niet matcht) en de evaluator (sterkste combinatie
wint, alleen scorende kaarten tellen).

Extra controles op de hele evaluator:

- **Alle 2.598.960 handen van 5 kaarten** worden beoordeeld en de aantallen per combinatie kloppen met de
  bekende pokeraantallen (bijvoorbeeld 624 keer Four of a kind en 3.744 keer Full House).
- **20.000 willekeurige handen** (vaste seed) moeten de scoreregels volgen: alleen gespeelde kaarten scoren,
  de formule klopt en er matcht nooit een sterkere combinatie dan de gekozen.
- Een **tabel met handen** en de exacte verwachte punten en totalen, plus de voorbeelden uit de user stories.
- Een **heel deck uitspelen** via `Round`: elke kaart precies een keer, de scores tellen op.

De grote controle duurt ongeveer 20 seconden en heeft het label `Slow`. Zonder die test:
`dotnet test --filter "Category!=Slow"`.

## Voortgang

| Fase | Inhoud | Status |
|---|---|---|
| 1 | Basis: Card, Deck, Hand, Round, HighCard / Pair / Three of a kind, scoring, tests | klaar |
| 2A | Meer combinaties: Two pair, Four of a kind, Full House, Straight, Flush | klaar |
| 2B | Kaarttypes: BonusCard, GlassCard via een interface | gepland |
| 2C | Console-UI volgens MVVM | gepland |
| 3 en later | Opruimen, should-haves en could-haves (o.a. stickers via het Decorator-patroon, Windows Forms) | gepland |

De taken en user stories staan op het Trello-bord van dit project.
