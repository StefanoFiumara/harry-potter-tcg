# Harry Potter Trading Card Game

## Introduction
This game is a digital recreation of the old Harry Potter Trading Card Game developed (and sadly discontinued) by Wizards of the Coast.

Unlike more generic tabletop card game platforms, this one is geared towards the Harry Potter TCG, and aims to facilitate the aspect of building decks and playing the game online through a client that automatically enforces all game rules.
     

---
I am once again restarting this project for what is now the third time (Hooray!). This time around I am more focused on the extensibility of the underlying game logic, there were many flaws in the way the previous iteration of this client that was causing many card interactions to be difficult to code around.

At it's core, the new engine decouples game data & logic from animation, and provides a robust game action system that can bridge the two together to form very extensible chains of actions that drive the game loop.


## Road Map

#### Initial Prototype
* [x] Playable Lesson and Creature cards
* [x] Full game cycle implementation (game over when decks run out of cards)
* [x] Simple (Dumb) Single Player AI to play against

#### Version 1
* [ ] Basic Spells
* [ ] Basic Items

#### Version 2
* [ ] Deck Editor
    * [ ] Save/Load/Manage multiple decks
* [ ] 90% Base Set cards available

#### Version 3
* [ ] Multiplayer Matchmaking & Lobby System

#### Version X
* [ ] How-to-play tutorial
* [ ] AI examines game state to decide best card to play
* [ ] Progression System (profiles, stat tracking, achievements)
* [ ] Draft Mode
* [ ] Implementation of community made cards and expansions.