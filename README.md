Harry Potter Trading Card Game
==============================

Introduction
------------
I am once again restarting this project for what is now the third time. This time around I am focused on tooling and extensibility. I am writing a few editor extensions to assist in the card creation process. I am also re-writing game logic from the ground up in order to allow some behaviors that were missing from the previous implementation.

What is it?
-----------
HPTCG is a recreation of the old Harry Potter Trading Card Game developed (and sadly discontinued) by Wizards of the Coast.
The initial focus of this project was a single player experience through AI opponents and random deck generation for replayability, the goal has since shifted to implementing a multiplayer client with random matchmaking for players to connect to each other from anywhere. This was made possible by the switch to the Unity3D game engine and the Photon Cloud service.

Once the main game is implemented the plan is to create a tutorial scene that introduces players to the game and allows them to try out many deck combinations before hopping into the matchmaking service.

Goals for Initial Prototype
---------------------------
* Playable Lesson and Creature cards
* Full game cycle implementation (game over when decks run out of cards)
* Simple Single Player AI to play against

Long Term Goals
---------------
* Robust Single Player AI
* Multiplayer Matchmaking & Lobby System
* Fully functional Deck Editor (Save/Load from JSON)
* Progression System (profiles, stat tracking, achievements)
