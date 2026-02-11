Blackjack (C# CLI)

  Simple Blackjack implementation in C# with clean architecture, unit tests, and code coverage.



Project Overview

  This project implements a standard Blackjack round between one player and a dealer using a console interface (CLI).
  The focus of the project is clean separation of concerns, testable game logic, and documentation of development decisions.



The solution is divided into three projects:
  
  Blackjack.Core
  Contains all domain models and game logic (Card, Deck, Hand, GameEngine).
  
  Blackjack.Cli
  Console application responsible for user input and rendering output.
  
  Blackjack.Tests
  MSTest project containing unit tests for core Blackjack rules and game flow.





Implemented Features


  Standard 52-card deck with shuffle and draw
  
  Correct hand value calculation with Ace as 1 or 11
  
  Player hit / stand
  
  Dealer automatically draws until at least 17
  
  Win / lose / push outcome logic
  
  Play again / exit flow
  
  Robust input validation
  
  Separation of UI and game logic
  
  Unit tests for:
  
  Hand value (Ace logic)
  
  Bust detection
  
  Dealer draw rule
  
  Game outcome logic
  
  HTML code coverage report





Architecture


The solution follows basic Clean Architecture principles:

  Core logic is isolated from UI
  
  GameEngine depends on abstractions (IDeck)
  
  CLI acts as composition root
  
  Tests use FakeDeck for deterministic scenarios




Folder structure:


Blackjack.Core
  
  Domain
  Card, Deck, Hand
  
  Game
  GameEngine, RoundResult
  
  Abstractions
  IDeck

Blackjack.Cli
  
  UI
  ConsoleInput, ConsoleRenderer
  
  Program.cs

  
Blackjack.Tests
  
  Domain
  HandValueTests
  
  Game
  GameEngineTests, FakeDeck

  

Running the Application

  Open the solution in Visual Studio.
  
  Set Blackjack.Cli as startup project.


Run:

  F5


Running Tests

  In Visual Studio:
  
  Test → Run All Tests

  

Code Coverage

  Coverage is generated using:
  
  MSTest
  
  Coverlet
  
  ReportGenerator
  
  An HTML coverage report can be generated via the provided script and opened in a browser.



Development Notes

  Unit tests were implemented early to validate core Blackjack rules before building the CLI.
  
  The game engine is fully testable using dependency injection through IDeck, allowing deterministic tests via FakeDeck.
  
  This approach minimizes regressions during further development and keeps UI concerns separated from business logic.



Author

Stefan Andrei Tumurug
