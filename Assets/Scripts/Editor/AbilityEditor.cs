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
    private static string[] ValidActions => ActionNamesLoader.Value;
    
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

    private static List<Type> GetTargetSelectors()
    {
        return AppDomain.CurrentDomain.GetAssemblies().SelectMany(a => a.GetTypes())
            .Where(t => t.IsClass && !t.IsAbstract && t.IsSubclassOf(typeof(BaseTargetSelector)))
            .ToList();
    }

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
            EditorUtils.Button("X", EditorColors.Error, () => actionDefinitions.RemoveAt(index), GUILayout.Width(20), GUILayout.Height(20));
            GUILayout.EndHorizontal();
            
            GUILayout.Space(5);
            
            GUILayout.BeginHorizontal();
            actionDef.ActionName = EditorUtils.Dropdown("\tAction Name", actionDef.ActionName, ValidActions);
            GUILayout.EndHorizontal();
            
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