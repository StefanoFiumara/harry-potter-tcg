using System.Runtime.CompilerServices;
using HarryPotter.Enums;
using HarryPotter.GameActions.ActionParameters;
using UnityEditor;
using UnityEngine;

// ReSharper disable once CheckNamespace
public class DamageActionParameterEditor : IActionParameterEditor
{
    private readonly DamageActionParameter _parameter;
    
    public string SerializedValue { get; private set; }

    public DamageActionParameterEditor(DamageActionParameter parameter)
    {
        _parameter = parameter;
        SerializedValue = parameter.Serialize();
    }
    public void OnInspectorGUI()
    {
        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("\t\tDamage Amount");
        _parameter.DamageAmount = EditorGUILayout.IntField(_parameter.DamageAmount, GUILayout.Width(150));
        EditorGUILayout.EndHorizontal();
        
        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("\t\tTarget Player");
        _parameter.WhichPlayer = (Alliance) EditorGUILayout.EnumPopup(_parameter.WhichPlayer, GUILayout.Width(150));
        EditorGUILayout.EndHorizontal();
        
        SerializedValue = _parameter.Serialize();
    }
}