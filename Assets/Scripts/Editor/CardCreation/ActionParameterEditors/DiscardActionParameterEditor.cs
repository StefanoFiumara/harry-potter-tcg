using HarryPotter.GameActions.ActionParameters;
using UnityEditor;
using UnityEngine;

// ReSharper disable once CheckNamespace
public class DiscardActionParameterEditor : IActionParameterEditor
{
    private readonly DiscardActionParameter _parameter;

    public string SerializedValue { get; private set; }

    public DiscardActionParameterEditor(DiscardActionParameter parameter)
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