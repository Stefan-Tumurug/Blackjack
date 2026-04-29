# Blackjack CLI – Clean Architecture C# Project

A console-based Blackjack implementation written in C# using a clean architecture structure with separation between domain logic, UI, and tests.

The project demonstrates object-oriented design, dependency injection, deterministic testing, and maintainable game engine logic.

---

# Project Structure

The solution contains three projects:

## Blackjack.Core

Contains all domain logic:

- Card
- Deck
- Hand (Ace handling logic)
- GameEngine
- RoundResult enum
- IDeck abstraction (for testability)

Responsibilities:

- rules of Blackjack
- score calculation
- dealer behaviour
- round resolution
- deterministic gameplay logic

No UI code exists in this layer.

---

## Blackjack.Cli

Console interface for interacting with the game.

Includes:

- Program.cs (game loop)
- ConsoleInput (validated user input helpers)
- ConsoleRenderer (console output handling)

Responsibilities:

- display game state
- read player decisions
- control round flow
- replay handling

Game logic remains fully separated inside Blackjack.Core.

---

## Blackjack.Tests

Unit tests written using MSTest.

Includes:

- HandValueTests
- GameEngineTests
- FakeDeck implementation for deterministic scenarios

Tests verify:

- Ace scoring logic
- dealer draw rules (dealer stands on 17+)
- player bust scenarios
- push situations
- round outcome correctness

---

# Features

Implemented Blackjack mechanics:

- player hit / stand
- dealer automatic draw to 17
- Ace value adjustment (1 or 11)
- bust detection
- push detection
- replay support
- deterministic testing via FakeDeck

---

# Example Gameplay Flow

1. deck is created
2. player receives two cards
3. dealer receives two cards
4. player chooses hit or stand
5. dealer draws until reaching 17+
6. winner is determined
7. result is displayed
8. player can start a new round

---

# Architecture Highlights

This project demonstrates:

- separation of concerns
- dependency inversion using IDeck
- deterministic testing with fake dependencies
- domain-driven game engine structure
- reusable console UI helpers
- clean class responsibilities

GameEngine contains no console logic and is fully testable in isolation.

---

# Testing

Tests are written using MSTest.

Example scenarios:

- dealer stops drawing at 17
- player bust results in loss
- equal score results in push
- Ace adjusts dynamically between 1 and 11

Coverage is generated using:

- Coverlet
- ReportGenerator

---

# Example Build

Run from solution folder:


dotnet build


Run the CLI version:


dotnet run --project Blackjack.Cli


Run tests:


dotnet test


---

# Example Test Strategy

The FakeDeck class allows deterministic control of card order:

Example:


FakeDeck deck = new FakeDeck(
new Card(Rank.Ten, Suit.Spades),
new Card(Rank.Six, Suit.Hearts)
);


This makes GameEngine behaviour predictable and testable.

---

# Learning Goals

This project focuses on:

- object-oriented design in C#
- dependency injection principles
- deterministic unit testing
- domain logic isolation
- console UI separation
- maintainable architecture

---

# Author

Stefan Andrei Tumurug

C# console project demonstrating clean architecture, testable domain logic, and structured Blackjack game rules.
