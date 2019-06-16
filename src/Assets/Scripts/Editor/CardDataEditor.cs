using System;
using System.Collections.Generic;
using HarryPotter.Enums;
using HarryPotter.Game.Cards;
using UnityEditor;
using UnityEngine;

// ReSharper disable once CheckNamespace
[CustomEditor(typeof(CardData))]
public class CardDataEditor : Editor
{
    private CardData _cardData;

    private readonly Color _errorBgColor = new Color(1f, 91f / 255f, 91f / 255f);
    
    private void OnEnable()
    {
        try
        {
            _cardData = (CardData) target;
        }
        catch (Exception) { /* Ignore */ }
    }

    public override void OnInspectorGUI()
    {
        GUILayout.Space(10);

        if (!string.IsNullOrEmpty(_cardData.Id))
        {
            GUILayout.Label(_cardData.Id, EditorStyles.miniLabel);   
        }
            
        DrawDefaultInspector();
        
        GUILayout.Space(10);
        
        GUILayout.BeginHorizontal();
        GUILayout.Label("Image");
        _cardData.Image = (Sprite) EditorGUILayout.ObjectField(_cardData.Image, typeof(Sprite), false,
                                                  GUILayout.Width(75), GUILayout.Height(105));
        GUILayout.EndHorizontal();
        
        GUILayout.Space(10);
        
        GUILayout.BeginHorizontal();
        GUILayout.Label("Type");
        _cardData.Type = (CardType) EditorGUILayout.EnumPopup(_cardData.Type, GUILayout.Width(150));
        GUILayout.EndHorizontal();

        GUILayout.Space(10);

        ShowComponents("Card Attributes:", _cardData.Attributes);
        ShowComponents("Play Conditions:", _cardData.PlayConditions);
        ShowComponents("Play Actions:", _cardData.PlayActions);

    }

    private void ShowComponents<T>(string label, IList<T> components) where T : ScriptableObject
    {
        if (components.Count == 0) return;

        GUILayout.Label(label, EditorStyles.boldLabel);

        for (var i = components.Count - 1; i >= 0; i--)
        {
            var c = components[i];
            var editor = Editor.CreateEditor(c);

            GUILayout.BeginHorizontal();

            GUI.backgroundColor = _errorBgColor;
            if (GUILayout.Button("X", GUILayout.Width(25), GUILayout.Height(25)))
            {
                components.RemoveAt(i);
            }
            GUI.backgroundColor = Color.white;

            GUILayout.BeginVertical();
            editor.OnInspectorGUI();
            GUILayout.EndVertical();

            GUILayout.EndHorizontal();
            GUILayout.Space(10);
        }

        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
    }
}