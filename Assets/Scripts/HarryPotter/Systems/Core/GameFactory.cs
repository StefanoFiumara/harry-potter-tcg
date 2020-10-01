using HarryPotter.Data;
using HarryPotter.StateManagement;
using HarryPotter.StateManagement.GameStates;

namespace HarryPotter.Systems.Core
{
    public static class GameFactory
    {
        public static Container Create(MatchData match)
        {
            var game = new Container(match);

            game.AddSystem<ActionSystem>();
            game.AddSystem<TurnSystem>();
            game.AddSystem<PlayerSystem>();
            game.AddSystem<VictorySystem>();
            game.AddSystem<PlayerActionSystem>();
            game.AddSystem<LessonSystem>();
            game.AddSystem<DamageSystem>();
            game.AddSystem<TargetSystem>();
            game.AddSystem<CardSystem>();
            game.AddSystem<AbilitySystem>();
            game.AddSystem<BoardSystem>();
            game.AddSystem<SpellSystem>();
            
            
            // TODO: Only add AISystem when in Single Player Mode, otherwise, add NetworkSystem for multiplayer
            game.AddSystem<AISystem>();
            
            game.AddSystem<StateMachine>();
            game.AddSystem<GlobalGameStateSystem>();

            return game;
        }
    }
}