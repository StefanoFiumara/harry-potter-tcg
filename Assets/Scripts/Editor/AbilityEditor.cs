using System;
using System.Collections.Generic;
using System.Linq;
using HarryPotter.Data.Cards.CardAttributes.Abilities;
using HarryPotter.Data.Cards.TargetSelectors;
using HarryPotter.Enums;
using HarryPotter.GameActions;
using UnityEditor;
using UnityEngine;
using Utils;

// ReSharper disable once CheckNamespace
[CustomEditor(typeof(Ability))]
public class AbilityEditor : Editor, IEditableEditor, IValidator
{
    private static readonly Lazy<string[]> ActionNamesLoader = new Lazy<string[]>(GetValidActionNames);
    private static readonly Lazy<string[]> TargetSelectorNamesLoader = new Lazy<string[]>(() => new []{ "None" }.Concat(TargetSelectors.Select(t => t.Name)).ToArray());
    
    public static string SelectedTargetSelectorName = "None";
    
    private static string[] ValidActions => ActionNamesLoader.Value;
    private static string[] TargetSelectorNames => TargetSelectorNamesLoader.Value;
    
    private Ability _ability;
    
    public bool IsEditMode { get; set; }

    private static string[] GetValidActionNames()
    {
        return AppDomain.CurrentDomain.GetAssemblies().SelectMany(a => a.GetTypes())
            .Where(t => t.IsClass && !t.IsAbstract && t.IsSubclassOf(typeof(GameAction)))
            .Where(t => t.GetInterfaces().Contains(typeof(IAbilityLoader)))
            .Select(t => t.Name)
            .ToArray();
    }

    private static IEnumerable<Type> TargetSelectors =>
         AppDomain.CurrentDomain.GetAssemblies().SelectMany(a => a.GetTypes())
            .Where(t => t.IsClass && !t.IsAbstract && t.IsSubclassOf(typeof(BaseTargetSelector)));

    private void OnEnable()
    {
        try
        {
            _ability = (Ability) target;
        }
        catch (Exception) { /* Do Nothing */ }
    }

    public override void OnInspectorGUI()
    {
        if (!IsEditMode)
        {
            GUI.enabled = false;
        }

        DrawDefaultInspector();
        
        // TODO: Show Target Selector options
        GUILayout.Space(5);

        if (GUI.enabled)
        {
            var selector = EditorUtils.Dropdown("Target Selector", SelectedTargetSelectorName, TargetSelectorNames);

            if (selector != SelectedTargetSelectorName)
            {
                SelectedTargetSelectorName = selector;

                if (SelectedTargetSelectorName != "None")
                {
                    _ability.TargetSelector = (BaseTargetSelector) CreateInstance(selector);
                }
                else
                {
                    _ability.TargetSelector = null;
                }
            }
        }
        
        if (_ability.TargetSelector != null)
        {
            // TODO: Custom editor for this?
            var selectorEditor = CreateEditor(_ability.TargetSelector);
            selectorEditor.OnInspectorGUI();                
        }
        
        ShowActionDefinitions("Actions", _ability.Actions);
    }
    
    private void ShowActionDefinitions(string label, IList<ActionDefinition> actionDefinitions)
    {
        GUILayout.Space(10);
        GUILayout.BeginHorizontal();
        
        GUILayout.Label(label, EditorStyles.boldLabel);
        EditorUtils.Button("Add Action", EditorColors.Action, () => _ability.Actions.Add(new ActionDefinition()));
        
        GUILayout.EndHorizontal();
        
        EditorUtils.DrawLine(Color.gray);

        for (var i = actionDefinitions.Count - 1; i >= 0; i--)
        {
            int index = i;
            var actionDef = actionDefinitions[i];
            
            GUILayout.BeginHorizontal();
            GUILayout.Label("\tAction", EditorStyles.boldLabel);
            EditorUtils.CloseButton(() =>
            {
                if (actionDefinitions.Count > 1)
                {
                    actionDefinitions.RemoveAt(index);
                }
            });
            GUILayout.EndHorizontal();
            
            GUILayout.Space(5);
            
            actionDef.ActionName = EditorUtils.Dropdown("\tAction Name", actionDef.ActionName, ValidActions);
            
            GUILayout.Space(5);
            
            GUILayout.BeginHorizontal();
            GUILayout.Label("\tParams");
            actionDef.Params = EditorGUILayout.TextField(actionDef.Params);
            GUILayout.EndHorizontal();

            if (i != 0)
            {
                EditorUtils.DrawLine(Color.gray.WithAlpha(0.7f), 1, 5);
            }
            
            GUILayout.Space(10);
        }
    }

    public bool IsValid()
    {
        return _ability.Actions.Count > 0;
    }
}