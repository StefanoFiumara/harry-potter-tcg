using System;
using System.Linq;
using HarryPotter.Data.Cards;
using HarryPotter.Data.Cards.CardAttributes;
using UnityEditor;
using UnityEngine;

// ReSharper disable once CheckNamespace
public class CreateCardWindow : EditorWindow
{
    [MenuItem("Harry Potter TCG/New Card Asset")]
    public static void NewCard()
    {
        GetWindow<CreateCardWindow>(true, "Create New Card", focus: true);
    }

    private CardData _cardData;
    
    private readonly Color _successBgColor = new Color(137f/255f, 214f / 255f, 98f / 255f);
    private void OnEnable()
    {
        _cardData = CreateInstance<CardData>();

        var actionCost = CreateInstance<ActionCost>();
        _cardData.Attributes.Add(actionCost);
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
        
        var editor = Editor.CreateEditor(_cardData) as CardDataEditor;
        editor.EditMode = true;
        editor.OnInspectorGUI();
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
               && _cardData.Image != null
            && _cardData.Attributes.OfType<ActionCost>().Count() == 1;
    }
}