using System;
using System.Linq;
using HarryPotter.Data.Cards.CardAttributes;
using HarryPotter.Enums;
using UnityEditor;
using UnityEngine;
using Utils;

// ReSharper disable once CheckNamespace
[CustomEditor(typeof(HarryPotter.Data.Cards.CardData))]
public class CardData : Editor, IEditable
{
    private HarryPotter.Data.Cards.CardData _cardData;

    public bool IsEditMode { get; set; } = false;

    private void OnEnable()
    {
        try
        {
            _cardData = (HarryPotter.Data.Cards.CardData) target;
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

        RenderCardAttributes();
        
        GUI.enabled = true;
    }

    private void RenderCardAttributes()
    {
        RenderCardAttributeHeader();
        E.DrawLine(Color.gray);

        for (var i = _cardData.Attributes.Count - 1; i >= 0; i--)
        {
            int index = i;
            var attribute = _cardData.Attributes[i];
            var editor = CreateEditor(attribute);
            
            if (editor is IEditable editable)
            {
                editable.IsEditMode = true;
            }

            GUILayout.BeginHorizontal();
            GUILayout.BeginVertical();
            
            editor.OnInspectorGUI();
            
            GUILayout.EndVertical();
            
            if (attribute.GetType() != typeof(ActionCost))
            {
                E.CloseButton(() => _cardData.Attributes.RemoveAt(index));
            }
            
            GUILayout.EndHorizontal();
            E.DrawLine(Color.gray.WithAlpha(0.8f), 1);
            
            GUILayout.Space(10);
        }
    }

    private void RenderCardAttributeHeader()
    {
        GUILayout.BeginHorizontal();
        
        GUILayout.Label("Card Attributes", EditorStyles.boldLabel);
        E.Button("Add New", E.Colors.Action, () =>
        {
            var window = EditorWindow.GetWindow<CreateAttributeWindow>(true, "New Card Attribute", focus: true);
            window.InitWindow(attribute =>
            {
                if (_cardData.Attributes.Any(attr => attr.GetType().Name == attribute.GetType().Name))
                {
                    // Do not add duplicate attributes.
                    return;
                }
                _cardData.Attributes.Insert(0, attribute);
            });
        });
        
        GUILayout.EndHorizontal();
    }
}