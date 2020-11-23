using HarryPotter.GameActions.ActionParameters;
using UnityEditor;
using UnityEngine;

// ReSharper disable once CheckNamespace
public class DamagePlayerOrCreatureActionParameterEditor : IActionParameterEditor
{
    private readonly DamagePlayerOrCreatureParameter _parameter;

    public string SerializedValue { get; private set; }

    public DamagePlayerOrCreatureActionParameterEditor(DamagePlayerOrCreatureParameter parameter)
    {
        _parameter = parameter;
    }

    public void OnInspectorGUI()
    {
        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("\t\tAmount");
        _parameter.Amount = EditorGUILayout.IntField(_parameter.Amount, GUILayout.Width(150));
        EditorGUILayout.EndHorizontal();
        
        SerializedValue = _parameter.Serialize();
    }
}