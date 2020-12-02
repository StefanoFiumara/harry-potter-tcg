using UnityEngine;

namespace HarryPotter.Views.UI.Tooltips
{
    public interface ITooltipContent
    {
        string GetDescriptionText();
        string GetActionText(MonoBehaviour context = null);
    }
}