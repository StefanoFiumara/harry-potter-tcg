using System;
using System.Collections.Generic;
using System.Linq;
using CardCreation.ActionParameterEditors;
using Core;
using HarryPotter.Data.Cards.CardAttributes.Abilities;
using HarryPotter.Data.Cards.TargetSelectors;
using HarryPotter.GameActions;
using HarryPotter.Utils;
using UnityEditor;
using UnityEngine;

namespace CardCreation
{
    [CustomEditor(typeof(Ability))]
    public class AbilityEditor : Editor, IValidator
    {
        private static readonly Lazy<string[]> ActionNamesLoader = new Lazy<string[]>(GetValidActionNames);
        private static readonly Lazy<string[]> TargetSelectorNamesLoader = new Lazy<string[]>(() => new []{ "None" }.Concat(TargetSelectors.Select(t => t.Name)).ToArray());

        public static string SelectedTargetSelectorName = "None";

        private static string[] ValidActions => ActionNamesLoader.Value;
        private static string[] TargetSelectorNames => TargetSelectorNamesLoader.Value;

        private Ability _ability;

        private static string[] GetValidActionNames()
        {
            return AppDomain.CurrentDomain.GetAssemblies().SelectMany(a => a.GetTypes())
                .Where(t => t.IsClass && !t.IsAbstract && t.IsSubclassOf(typeof(GameAction)))
                .Where(t => t.GetInterfaces().Contains(typeof(IAbilityLoader)))
                .Select(t => t.Name)
                .ToArray();
        }

        private static IEnumerable<Type> TargetSelectors =>
            AppDomain.CurrentDomain
                .GetAssemblies()
                .SelectMany(a => a.GetTypes())
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
            DrawDefaultInspector();

            GUILayout.Space(5);
            if (GUI.enabled)
            {
                E.Dropdown("Target Selector", ref SelectedTargetSelectorName, TargetSelectorNames, OnTargetSelectorChanged);
            }

            if (_ability.TargetSelector != null)
            {
                var selectorEditor = CreateEditor(_ability.TargetSelector);
                selectorEditor.OnInspectorGUI();
            }

            RenderActionDefinitions("Actions", _ability.Actions);
        }

        private void RenderActionDefinitions(string label, IList<ActionDefinition> actionDefinitions)
        {
            GUILayout.Space(10);
            GUILayout.BeginHorizontal();

            GUILayout.Label(label, EditorStyles.boldLabel);
            E.Button("Add Action", E.Colors.Action, () => _ability.Actions.Add(new ActionDefinition()));

            GUILayout.EndHorizontal();

            E.DrawLine(Color.gray);

            for (var i = 0; i < actionDefinitions.Count; i++)
            {
                var actionDef = actionDefinitions[i];

                GUILayout.BeginHorizontal();
                GUILayout.Label("\tAction", EditorStyles.boldLabel);

                var removed = E.RemoveButton(actionDefinitions, i);
                if (removed) i--;

                GUILayout.EndHorizontal();

                GUILayout.Space(5);

                E.Dropdown("\tAction Name", ref actionDef.ActionName, ValidActions, (curValue, newValue) => actionDef.Params = string.Empty);

                GUILayout.Space(5);

                actionDef.Params = RenderActionParameters(actionDef);

                if (i != 0)
                {
                    E.DrawLine(Color.gray.WithAlpha(0.7f), 1, 5);
                }

                GUILayout.Space(10);
            }
        }

        private string RenderActionParameters(ActionDefinition actionDefinition)
        {
            if (!ValidActions.Contains(actionDefinition.ActionName))
            {
                return string.Empty;
            }

            var editor = ActionParameterEditorFactory.CreateEditor(actionDefinition);
            if (editor != null)
            {
                GUILayout.Label("\tAction Parameters");
                editor.OnInspectorGUI();
                return editor.SerializedValue;
            }

            GUILayout.Label("\tNo Parameters Needed");
            return string.Empty;
        }

        private void OnTargetSelectorChanged(string oldValue, string newValue)
        {
            if (newValue != "None")
            {
                _ability.TargetSelector = (BaseTargetSelector) CreateInstance(newValue);
            }
            else
            {
                _ability.TargetSelector = null;
            }
        }

        public bool IsValid()
        {
            return _ability.Actions.Count > 0;
        }
    }
}
