using HarryPotter.Enums;
using HarryPotter.GameActions.ActionParameters;
using UnityEditor;
using UnityEngine;

namespace CardCreation.ActionParameterEditors
{
    public class ShuffleDeckActionParameterEditor : IActionParameterEditor
    {
        private readonly ShuffleDeckActionParameter _parameter;

        public string SerializedValue { get; private set; }

        public ShuffleDeckActionParameterEditor(ShuffleDeckActionParameter parameter)
        {
            _parameter = parameter;
        }

        public void OnInspectorGUI()
        {
            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("\t\tTarget Player");
            _parameter.WhichPlayer = (Alliance) EditorGUILayout.EnumPopup(_parameter.WhichPlayer, GUILayout.Width(150));
            EditorGUILayout.EndHorizontal();
        
            SerializedValue = _parameter.Serialize();
        }
    }
}