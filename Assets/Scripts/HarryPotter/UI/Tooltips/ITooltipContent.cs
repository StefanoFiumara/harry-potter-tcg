using UnityEngine;

namespace HarryPotter.UI.Tooltips
{
    public interface ITooltipContent
    {
        string GetDescriptionText();
        string GetActionText(MonoBehaviour context = null);
    }
}