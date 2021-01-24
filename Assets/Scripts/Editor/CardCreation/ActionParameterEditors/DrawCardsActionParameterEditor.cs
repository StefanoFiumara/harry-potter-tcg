using HarryPotter.Enums;
using HarryPotter.GameActions.ActionParameters;
using UnityEditor;
using UnityEngine;

namespace CardCreation.ActionParameterEditors
{
    public class DrawCardsActionParameterEditor : IActionParameterEditor
    {
        private readonly DrawCardsActionParameter _parameter;

        public string SerializedValue { get; private set; }

        public DrawCardsActionParameterEditor(DrawCardsActionParameter parameter)
        {
            _parameter = parameter;
        }

        public void OnInspectorGUI()
        {
            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("\t\tAmount");
            _parameter.Amount = EditorGUILayout.IntField(_parameter.Amount, GUILayout.Width(150));
            EditorGUILayout.EndHorizontal();
        
            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("\t\tTarget Player");
            _parameter.WhichPlayer = (Alliance) EditorGUILayout.EnumPopup(_parameter.WhichPlayer, GUILayout.Width(150));
            EditorGUILayout.EndHorizontal();
        
            SerializedValue = _parameter.Serialize();
        }
    }
}