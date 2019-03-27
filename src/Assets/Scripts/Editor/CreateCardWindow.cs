using System.Collections.Generic;
using System.Reflection;
using HarryPotter.Enums;
using HarryPotter.Game.Data;
using HarryPotter.Game.Data.PlayConditions;
using UnityEditor;
using UnityEngine;
using Object = System.Object;

// ReSharper disable once CheckNamespace
public class CreateCardWindow : EditorWindow
{
    [MenuItem("Harry Potter TCG/New Card")]
    public static void NewCard()
    {
        GetWindow<CreateCardWindow>(true, "New Card", focus: true);
    }

    private CardData _cardData;
    
    private readonly Color _errorBgColor = new Color(1f, 91f / 255f, 91f / 255f);
    private readonly Color _successBgColor = new Color(137f/255f, 214f / 255f, 98f / 255f);
    private void OnEnable()
    {
        _cardData = CreateInstance<CardData>();
    }

    private void OnGUI()
    {
        GUILayout.BeginVertical();
        GUILayout.Label("Create New Card", EditorStyles.boldLabel);
        
        //TODO: Validate before showing button
        GUI.backgroundColor = _successBgColor;
        GUI.enabled = ValidateCardData();
        if (GUILayout.Button("Create Card"))
        {
            //TODO: Create card and save asset file 
        }

        GUI.enabled = true;
        GUI.backgroundColor = Color.white;
        
        var editor = Editor.CreateEditor(_cardData);
        editor.OnInspectorGUI();

        DrawModifierButtons();
        
        ShowComponents("Card Attributes:", _cardData.Attributes);
        ShowComponents("Play Conditions:", _cardData.PlayConditions);
        ShowComponents("Play Actions:", _cardData.PlayActions);
    }

    private bool ValidateCardData()
    {
        return !string.IsNullOrEmpty(_cardData.CardName)
               && _cardData.Image != null
               && _cardData.PlayActions.Count > 0;
    }
    
    private void DrawModifierButtons()
    {
        GUILayout.BeginHorizontal();

        if (GUILayout.Button("Add Play Condition"))
        {
            var window = GetWindow<AddComponentWindow>(true, "Add Play Condition", focus: true);
            window.SetSelection<PlayCondition>(condition =>
            {
                _cardData.PlayConditions.Add(condition);
                Focus();
            });
        }

        if (GUILayout.Button("Add Card Attribute"))
        {
            var window = GetWindow<AddComponentWindow>(true, "Add Card Attribute", focus: true);
            window.SetSelection<CardAttribute>(attribute =>
            {
                _cardData.Attributes.Add(attribute);
                Focus();
            });
        }

        if (GUILayout.Button("Add Play Action"))
        {
            var window = GetWindow<AddComponentWindow>(true, "Add Card Attribute", focus: true);
            window.SetSelection<PlayAction>(action =>
            {
                _cardData.PlayActions.Add(action);
                Focus();
            });
        }

        GUILayout.EndHorizontal();
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
            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
        }
    }
}