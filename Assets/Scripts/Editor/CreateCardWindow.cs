using System;
using HarryPotter.Data.Cards;
using UnityEditor;
using UnityEngine;

// ReSharper disable once CheckNamespace
public class CreateCardWindow : EditorWindow
{
    [MenuItem("Harry Potter TCG/New Card")]
    public static void NewCard()
    {
        GetWindow<CreateCardWindow>(true, "New Card", focus: true);
    }

    private CardData _cardData;
    
    private readonly Color _successBgColor = new Color(137f/255f, 214f / 255f, 98f / 255f);
    private void OnEnable()
    {
        _cardData = CreateInstance<CardData>();
    }

    private void OnGUI()
    {
        GUILayout.BeginVertical();
        GUILayout.Label("Create New Card", EditorStyles.boldLabel);
        
        GUI.backgroundColor = _successBgColor;
        GUI.enabled = IsCardDataValid();
        if (GUILayout.Button("Create Card"))
        {
            BuildCardDataAsset();
            Close();
        }

        GUI.enabled = true;
        GUI.backgroundColor = Color.white;
        
        var editor = Editor.CreateEditor(_cardData);
        editor.OnInspectorGUI();

        DrawModifierButtons();
    }

    private void BuildCardDataAsset()
    {
        _cardData.Id = Guid.NewGuid().ToString();
        Debug.Log($"Generated card with Id: {_cardData.Id}");

        var assetDirectory = $@"Assets\GameData\Cards\{_cardData.Type}";
        var assetPath = $@"{assetDirectory}\{_cardData.CardName}.asset";


        if (!AssetDatabase.IsValidFolder(assetDirectory))
        {
            AssetDatabase.CreateFolder(@"Assets\GameData\Cards", $"{_cardData.Type}");
        }

        AssetDatabase.CreateAsset(_cardData, assetPath);
        
        foreach (var attribute in _cardData.Attributes)
        {
            AssetDatabase.AddObjectToAsset(attribute, assetPath);
        }
        
        AssetDatabase.ImportAsset(AssetDatabase.GetAssetPath(_cardData));

        Debug.Log($"Created Card Asset at: {AssetDatabase.GetAssetPath(_cardData)}");
    }

    private bool IsCardDataValid()
    {
        return !string.IsNullOrEmpty(_cardData.CardName)
               && _cardData.Image != null;
    }
    
    private void DrawModifierButtons()
    {
        GUILayout.BeginHorizontal();

        if (GUILayout.Button("Add Card Attribute"))
        {
            var window = GetWindow<AddComponentWindow>(true, "Add Card Attribute", focus: true);
            window.SetSelection<CardAttribute>(attribute =>
            {
                _cardData.Attributes.Add(attribute);
                Focus();
            });
        }
        
        GUILayout.EndHorizontal();
    }
}