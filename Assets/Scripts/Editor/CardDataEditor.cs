using System;
using System.Collections.Generic;
using System.Linq;
using HarryPotter.Data.Cards;
using HarryPotter.Data.Cards.CardAttributes;
using HarryPotter.Enums;
using UnityEditor;
using UnityEngine;

// ReSharper disable once CheckNamespace
[CustomEditor(typeof(CardData))]
public class CardDataEditor : Editor
{
    private CardData _cardData;

    private readonly Color _errorBgColor = new Color(1f, 91f / 255f, 91f / 255f);
    public bool EditMode { get; set; } = false;

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
        if (!EditMode)
        {
            GUI.enabled = false;
        }
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
        
        DrawModifierButtons();
        
        GUI.enabled = true;
    }

    private void ShowComponents<T>(string label, IList<T> components) where T : ScriptableObject
    {
        if (components.Count == 0) return;

        GUILayout.Label(label, EditorStyles.boldLabel);

        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
        
        for (var i = components.Count - 1; i >= 0; i--)
        {
            var c = components[i];
            var editor = CreateEditor(c);

            GUILayout.BeginHorizontal();

            GUILayout.BeginVertical();
            editor.OnInspectorGUI();
            GUILayout.EndVertical();
            
            if (c.GetType() != typeof(ActionCost))
            {
                GUI.backgroundColor = _errorBgColor;
                if (GUILayout.Button("X", GUILayout.Width(20), GUILayout.Height(20)))
                {
                    components.RemoveAt(i);
                }
                GUI.backgroundColor = Color.white;
            }
            

            GUILayout.EndHorizontal();
            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
            
            GUILayout.Space(10);
        }
    }
    
    private void DrawModifierButtons()
    {
        GUILayout.BeginHorizontal();

        if (GUILayout.Button("Add Attribute"))
        {
            var window = EditorWindow.GetWindow<AddComponentWindow>(true, "Add Attribute", focus: true);
            window.SetSelection<CardAttribute>(attribute =>
            {
                if (_cardData.Attributes.Any(attr => attr.GetType().Name == attribute.GetType().Name))
                {
                    // Do not add duplicate attributes.
                    return;
                }
                _cardData.Attributes.Insert(0, attribute);
            });
        }
        
        GUILayout.EndHorizontal();
    }
}