using HarryPotter.Data.Cards.CardAttributes.Abilities;
using HarryPotter.GameActions.ActionParameters;
using HarryPotter.GameActions.Actions;

// ReSharper disable once CheckNamespace
// TODO: Refactor into generic interface?
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
            case nameof(DamagePlayerAction):
                return new DamageActionParameterEditor(DamageActionParameter.FromString(actionDef.Params));
            case nameof(DrawCardsAction):
                return new DrawCardsActionParameterEditor(DrawCardsActionParameter.FromString(actionDef.Params));
            case nameof(DamagePlayerOrCreatureAction):
                return new DamagePlayerOrCreatureActionParameterEditor(DamagePlayerOrCreatureParameter.FromString(actionDef.Params));
            case nameof(ShuffleDeckAction):
                return new ShuffleDeckActionParameterEditor(ShuffleDeckActionParameter.FromString(actionDef.Params));
        }

        return null;
    }
}