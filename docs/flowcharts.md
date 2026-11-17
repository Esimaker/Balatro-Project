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

## 2. Scoring met alle speciale kaarten (ontwerp voor fase 2B)

De basisscore (combinatie zoeken, kaartpunten, totaal) werkt al. De stappen over speciale kaarten (BonusCard,
GlassCard) zijn het **ontwerp voor fase 2B** en zijn nog niet gebouwd.

```mermaid
flowchart TD
    A([Gespeelde kaarten]) --> B[Zoek de sterkste combinatie<br/>van sterk naar zwak]
    B --> C[Welke kaarten zitten in de combinatie?<br/>Alleen die scoren]
    C --> D[Punten = basispunten van de combinatie<br/>Multiplier = basismultiplier]
    D --> E{Volgende scorende kaart}
    E --> F[Kaartpunten erbij optellen<br/>2-10 = nummer, J Q K = 10, A = 11]
    F --> G{Heeft de kaart een effect?<br/>ontwerp fase 2B}
    G -- geen --> E
    G -- BonusCard --> G1[+10 punten] --> E
    G -- GlassCard --> G2[Multiplier x2] --> G3{Breekt de kaart?<br/>kans, met seed in tests}
    G3 -- ja --> G4[Kaart wordt verwijderd] --> E
    G3 -- nee --> E
    E -- geen kaarten meer --> H[Totaal = punten x multiplier]
    H --> I[Totaal bij de score van de ronde optellen]
    I --> J([ScoreResult naar de UI])
```

Zo wordt gescoord: `(basispunten + kaartpunten) x multiplier`.
