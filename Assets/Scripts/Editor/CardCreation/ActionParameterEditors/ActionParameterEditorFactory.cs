using HarryPotter.Data.Cards.CardAttributes.Abilities;
using HarryPotter.GameActions.ActionParameters;
using HarryPotter.GameActions.Actions;

// ReSharper disable once CheckNamespace
public interface IActionParameterEditor
{
    void OnInspectorGUI();
    
    string SerializedValue { get; }
}

public static class ActionParameterEditorFactory
{
    public static IActionParameterEditor CreateEditor(ActionDefinition actionDef)
    {
        switch (actionDef.ActionName)
        {
            case nameof(DamageAction):
                return new DamageActionParameterEditor(DamageActionParameter.FromString(actionDef.Params));
            case nameof(DrawCardsAction):
                return new DrawCardsActionParameterEditor(DrawCardsActionParameter.FromString(actionDef.Params));
        }

        return null;
    }
}