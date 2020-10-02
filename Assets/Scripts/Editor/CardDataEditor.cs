using System;
using System.Collections.Generic;
using System.Linq;
using HarryPotter.Data.Cards;
using HarryPotter.Data.Cards.CardAttributes;
using HarryPotter.Enums;
using UnityEditor;
using UnityEngine;
using Utils;

// ReSharper disable once CheckNamespace
[CustomEditor(typeof(CardData))]
public class CardDataEditor : Editor, IEditableEditor
{
    private CardData _cardData;

    public bool IsEditMode { get; set; } = false;

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
        if (!IsEditMode)
        {
            GUI.enabled = false;
        }
        GUILayout.Space(10);

        if (!string.IsNullOrEmpty(_cardData.Id))
        {
            GUILayout.Label(_cardData.Id, EditorStyles.miniLabel);   
            GUILayout.Space(10);
        }
            
        DrawDefaultInspector();
        
        GUILayout.Space(10);
        
        GUILayout.BeginHorizontal();
        GUILayout.Label("Image");
        _cardData.Image = (Sprite) EditorGUILayout.ObjectField(
            _cardData.Image, 
            typeof(Sprite), 
            allowSceneObjects: false, 
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
        EditorUtils.DrawLine(Color.gray);
        // EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
        
        for (var i = components.Count - 1; i >= 0; i--)
        {
            var c = components[i];
            var editor = CreateEditor(c);
            if (editor is IEditableEditor editable)
            {
                editable.IsEditMode = true;
            }

            GUILayout.BeginHorizontal();

            GUILayout.BeginVertical();
            editor.OnInspectorGUI();
            GUILayout.EndVertical();
            
            if (c.GetType() != typeof(ActionCost))
            {
                GUI.backgroundColor = EditorColors.Error;
                if (GUILayout.Button("X", GUILayout.Width(20), GUILayout.Height(20)))
                {
                    components.RemoveAt(i);
                }
                GUI.backgroundColor = Color.white;
            }
            

            GUILayout.EndHorizontal();
            EditorUtils.DrawLine(Color.gray.WithAlpha(0.8f), 1);
            
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