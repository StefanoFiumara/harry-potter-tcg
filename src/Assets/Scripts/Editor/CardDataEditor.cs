using System;
using HarryPotter.Enums;
using HarryPotter.Game.Data;
using UnityEditor;
using UnityEngine;

// ReSharper disable once CheckNamespace
[CustomEditor(typeof(CardData))]
public class CardDataEditor : Editor
{
    private CardData _cardData;
    
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
        
        DrawDefaultInspector();
        
        GUILayout.Space(10);
        
        GUILayout.BeginHorizontal();
        GUILayout.Label("Image");
        _cardData.Image = (Sprite) EditorGUILayout.ObjectField(_cardData.Image, typeof(Sprite), false, GUILayout.Width(75), GUILayout.Height(105));
        GUILayout.EndHorizontal();
        
        GUILayout.Space(10);
        
        GUILayout.BeginHorizontal();
        GUILayout.Label("Type");
        _cardData.Type = (CardType) EditorGUILayout.EnumPopup(_cardData.Type, GUILayout.Width(150));
        GUILayout.EndHorizontal();
        
        GUILayout.Space(10);
    }
}