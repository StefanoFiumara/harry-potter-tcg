using HarryPotter.Data;
using HarryPotter.StateManagement;

namespace HarryPotter.Systems.Core
{
    public static class GameFactory
    {
        public static Container Create(MatchData match, GameSettings settings)
        {
            var game = new Container();
            
            match.Initialize();
            
            game.AddSystem<PlayerSettingsSystem>().Settings = settings;
            game.AddSystem<MatchSystem>().Match = match;

            game.AddSystem<ActionSystem>();
            
            game.AddSystem<StateMachine>();
            game.AddSystem<GlobalGameStateSystem>();
            
            game.AddSystem<PlayerSystem>();
            game.AddSystem<PlayerActionSystem>();
            game.AddSystem<TurnSystem>();
            
            // TODO: Only add AISystem when in Single Player Mode, otherwise, add NetworkSystem for multiplayer
            game.AddSystem<AISystem>();
            
            game.AddSystem<CardSystem>();
            game.AddSystem<TargetSystem>();
            
            game.AddSystem<HandSystem>();
            game.AddSystem<BoardSystem>();
            game.AddSystem<DiscardSystem>();
            
            game.AddSystem<LessonSystem>();
            game.AddSystem<CreatureSystem>();
            game.AddSystem<SpellSystem>();
            
            game.AddSystem<UniquenessSystem>();
            
            game.AddSystem<AbilitySystem>();
            game.AddSystem<DamageSystem>();
            game.AddSystem<HealingSystem>();
            
            game.AddSystem<VictorySystem>();
            
            return game;
        }
    }
}