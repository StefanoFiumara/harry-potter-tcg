using DG.Tweening;

namespace HarryPotter.Game.ActionSystem
{
    public interface IGameAction
    {
        Sequence Execute();
    }
}