using System;
using HarryPotter.Game.Data;
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
        GUI.enabled = ValidateCardData();
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

        foreach (var action in _cardData.PlayActions)
        {
            AssetDatabase.AddObjectToAsset(action, assetPath);
        }
        foreach (var attribute in _cardData.Attributes)
        {
            AssetDatabase.AddObjectToAsset(attribute, assetPath);
        }
        foreach (var condition in _cardData.PlayConditions)
        {
            AssetDatabase.AddObjectToAsset(condition, assetPath);
        }

        AssetDatabase.ImportAsset(AssetDatabase.GetAssetPath(_cardData));

        Debug.Log($"Created Card Asset at: {AssetDatabase.GetAssetPath(_cardData)}");
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
            var window = GetWindow<AddComponentWindow>(true, "Add Play Action", focus: true);
            window.SetSelection<PlayAction>(action =>
            {
                _cardData.PlayActions.Add(action);
                Focus();
            });
        }

        GUILayout.EndHorizontal();
    }
}