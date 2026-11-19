# Flowcharts

GitHub tekent deze diagrammen automatisch (Mermaid).

## 1. Hand validity check (een hand spelen)

Wat er gebeurt vanaf het selecteren van kaarten tot de score. Dit is hoe de code nu werkt
(`Hand.Select`, `Round.PlayHand`, `HandEvaluator.Evaluate`).

```mermaid
flowchart TD
    A([Speler selecteert een kaart]) --> B{Bestaat de kaart<br/>in de hand?}
    B -- nee --> B1[Selectie genegeerd:<br/>Select geeft false]
    B -- ja --> C{Al geselecteerd?}
    C -- ja --> C1[Genegeerd:<br/>Select geeft false]
    C -- nee --> D{Al 5 kaarten<br/>geselecteerd?}
    D -- ja --> D1[Geweigerd:<br/>maximaal 5 kaarten]
    D -- nee --> E[Kaart wordt geselecteerd]

    E --> F([Speler kiest: hand spelen])
    B1 --> F
    C1 --> F
    D1 --> F

    F --> G{Minstens 1 kaart<br/>geselecteerd?}
    G -- nee --> G1[/InvalidOperationException:<br/>selecteer minstens een kaart/]
    G -- ja --> H[Geselecteerde kaarten uit de hand halen]
    H --> I[HandEvaluator zoekt de sterkste combinatie]
    I --> J[Score berekenen en bij de ronde optellen]
    J --> K{Staan er nog kaarten<br/>in het deck?}
    K -- ja --> L[Hand aanvullen tot 8 kaarten]
    K -- nee --> M[Hand blijft kleiner]
    L --> N([Klaar: ScoreResult naar de UI])
    M --> N
```

## 2. Scoring met speciale kaarten

Dit is hoe `ScoreCalculator` werkt. Het enige wat de calculator van een kaart weet is `ICard.Apply(Score)`:
de kaart krijgt de huidige score en geeft een nieuwe terug. Het breken van een GlassCard is een could-have en
is **niet gebouwd** (de stippellijn).

```mermaid
flowchart TD
    A([Gespeelde kaarten]) --> B[Zoek de sterkste combinatie<br/>van sterk naar zwak]
    B --> C[Welke kaarten zitten in de combinatie?<br/>Alleen die scoren]
    C --> D[Score = basispunten van de combinatie<br/>en basismultiplier]
    D --> E{Volgende scorende kaart?}
    E -- ja --> F[score = kaart.Apply score]
    F --> F1[Card: punten erbij<br/>BonusCard: punten + 10 erbij<br/>GlassCard: punten erbij en multiplier x2]
    F1 --> E
    F -.-> G[Could-have: GlassCard breekt met een kans]
    E -- nee --> H[Totaal = punten x multiplier]
    H --> I[Totaal bij de score van de ronde optellen]
    I --> J([ScoreResult naar de UI])
```

Zo wordt gescoord: `(basispunten + kaartpunten) x multiplier`, waarbij een GlassCard de multiplier verdubbelt.
