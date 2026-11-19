# Klassendiagram (UML)

Dit diagram toont de klassen van `BalatroConsole` en hoe ze samenhangen. GitHub tekent het
automatisch (Mermaid). In Visual Studio kun je de tekst plakken op https://mermaid.live.

```mermaid
classDiagram
    direction LR

    %% ---------- Models ----------
    class ICard {
        <<interface>>
        +Rank Rank
        +Suit Suit
        +int Points
        +Apply(Score current) Score
    }
    class Card {
        +Rank Rank
        +Suit Suit
        +int Points
        +Apply(Score current) Score
        +ToString() string
    }
    class BonusCard {
        +Points int
    }
    class GlassCard {
        +Rank Rank
        +Suit Suit
        +int Points
        +Apply(Score current) Score
    }
    class Score {
        <<record>>
        +int Points
        +int Multiplier
        +int Total
        +AddPoints(int) Score
        +MultiplyBy(int) Score
    }
    class Deck {
        +Cards IReadOnlyList~ICard~
        +DrawnCards IReadOnlyList~ICard~
        +Shuffle()
        +Draw(int count) List~ICard~
    }
    class Hand {
        +MaxSelected int = 5
        +Cards IReadOnlyList~ICard~
        +SelectedCards IReadOnlyList~ICard~
        +AddCards(cards)
        +Select(int index) bool
        +Deselect(int index) bool
        +RemoveSelected() List~ICard~
    }
    class Suit {
        <<enumeration>>
        Hearts
        Diamonds
        Clubs
        Spades
    }
    class Rank {
        <<enumeration>>
        Two_to_Ten
        Jack
        Queen
        King
        Ace
    }

    %% ---------- Logic ----------
    class Round {
        +HandSize int = 8
        +Hand Hand
        +Score int
        +PlayHand() ScoreResult
    }
    class HandEvaluator {
        +Evaluate(playedCards) ScoreResult
    }
    class ScoreCalculator {
        +Calculate(combination, scoringCards) Score
    }
    class ScoreResult {
        +Combination IHandCombination
        +ScoringCards IReadOnlyList~ICard~
        +Points int
        +Multiplier int
        +Total int
    }
    class HandCombinations {
        <<static>>
        +CreateAll() List~IHandCombination~
    }
    class IHandCombination {
        <<interface>>
        +Type HandType
        +Name string
        +BasePoints int
        +BaseMultiplier int
        +Matches(cards) bool
        +GetScoringCards(cards) IReadOnlyList~ICard~
    }
    class OfAKindCombination {
        <<abstract>>
        #RequiredCount int*
    }
    class HighCard
    class Pair
    class TwoPair
    class ThreeOfAKind
    class FourOfAKind
    class Straight
    class Flush
    class FullHouse
    class HandType {
        <<enumeration>>
        HighCard
        Pair
        TwoPair
        ThreeOfAKind
        Straight
        Flush
        FullHouse
        FourOfAKind
    }

    %% ---------- Relations ----------
    ICard <|.. Card : implementeert
    ICard <|.. GlassCard : implementeert (geen kind van Card)
    Card <|-- BonusCard : override Points
    ICard ..> Score : Apply geeft een nieuwe Score
    ScoreCalculator ..> Score : maakt
    ScoreCalculator --> ICard : roept alleen Apply aan
    Card --> Rank
    Card --> Suit
    Deck o-- ICard : bevat
    Hand o-- ICard : bevat

    Round --> Deck : deelt uit
    Round *-- Hand : bezit
    Round --> HandEvaluator : gebruikt
    HandEvaluator --> IHandCombination : kent alleen het interface
    HandEvaluator --> ScoreCalculator : laat de score uitrekenen
    HandEvaluator ..> ScoreResult : maakt
    ScoreResult --> IHandCombination
    HandCombinations ..> IHandCombination : maakt de lijst

    IHandCombination <|.. OfAKindCombination
    IHandCombination <|.. HighCard
    IHandCombination <|.. TwoPair
    IHandCombination <|.. Straight
    IHandCombination <|.. Flush
    IHandCombination <|.. FullHouse
    OfAKindCombination <|-- Pair
    OfAKindCombination <|-- ThreeOfAKind
    OfAKindCombination <|-- FourOfAKind
    IHandCombination --> HandType
```

## Waarom zo?

| Keuze | Reden |
|---|---|
| `IHandCombination` (interface) | `HandEvaluator` kent alleen het interface. Een nieuwe combinatie is een nieuwe klasse plus een regel in `HandCombinations`, zonder `switch` aan te passen. |
| `OfAKindCombination` (abstract) | Pair, Three of a kind en Four of a kind zoeken allemaal "N kaarten met dezelfde rank". Die code staat een keer, de kinderen geven alleen `RequiredCount` en hun scorewaarden. |
| `HighCard`, `TwoPair`, `Straight`, `Flush` en `FullHouse` implementeren het interface direct | Ze passen niet bij "een rank, N kaarten" (twee groepen, een reeks, een suit), dus erven ze niet van de abstracte klasse. |
| `ICard` los van `Card` | Kaartvarianten kunnen het interface implementeren zonder dat de score-logica verandert. `GlassCard` doet dat direct en is dus geen kind van `Card`. |
| `ICard.Apply(Score)` | Elke kaart krijgt de huidige `Score` en geeft een nieuwe terug. De evaluator en `ScoreCalculator` hoeven daardoor niet te weten welk soort kaart scoort: een nieuwe kaartsoort is een nieuwe klasse, geen extra `if` in de logica. |
| `BonusCard` overschrijft alleen `Points` | `Card.Apply` telt `Points` op, dus de bonus (+10) werkt zonder dat `Apply` hoeft te veranderen. |
| `Score` als `record` | Punten en multiplier horen bij elkaar. Een record is onveranderlijk: `AddPoints` en `MultiplyBy` geven een nieuwe `Score`, dus een kaart kan de score van een andere kaart niet per ongeluk aanpassen. |
| `ScoreCalculator` los van `HandEvaluator` | De evaluator zoekt de combinatie, de calculator rekent. Elk heeft een taak en de calculator is los te testen. |
| `ScoreResult` onveranderlijk | Het resultaat van een gespeelde hand verandert niet meer, dus de UI kan het veilig tonen. |
| `Models` en `Logic` kennen de UI niet | Logica en visualisatie zijn gescheiden: de console-UI (en later WinForms) kan dezelfde klassen gebruiken. |
