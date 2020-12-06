using System;
using System.Linq;
using HarryPotter.Data;
using HarryPotter.Data.Cards;
using HarryPotter.Data.Cards.CardAttributes;
using HarryPotter.Data.Cards.CardAttributes.Abilities;
using HarryPotter.Enums;
using HarryPotter.Utils;
using UnityEditor;
using UnityEngine;

// ReSharper disable once CheckNamespace
public class CreateCardWindow : EditorWindow
{
    private const string CARD_LIBRARY_PATH = "Assets/GameData/CardLibrary.asset";
    
    [MenuItem("Harry Potter TCG/Build New Card")]
    public static void NewCard()
    {
        GetWindow<CreateCardWindow>(true, "Create New Card", focus: true);
    }

    private CardData _cardData;
    
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
        
        GUI.enabled = IsCardDataValid();
        
        E.Button("Build card", E.Colors.Success, () =>
        {
            BuildCardDataAsset();
            Close();
        });
        
        GUI.enabled = true;

        var editor = (CardDataEditor) Editor.CreateEditor(_cardData);
        editor.IsNewCard = true;
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

            if (attribute is Ability ability)
            {
                if (ability.TargetSelector != null)
                {
                    AssetDatabase.AddObjectToAsset(ability.TargetSelector, ability);
                }
            }
        }
        
        AssetDatabase.ImportAsset(AssetDatabase.GetAssetPath(_cardData));

        // Reset Global Editor options
        AbilityEditor.SelectedTargetSelectorName = "None"; // TODO: Encapsulate this?
        Debug.Log($"Created Card Asset at: {AssetDatabase.GetAssetPath(_cardData)}");
        
        UpdateCardLibrary();
    }

    private static void UpdateCardLibrary()
    {
        var cardLibrary = AssetDatabase.LoadAssetAtPath<CardLibrary>(CARD_LIBRARY_PATH);

        cardLibrary.Cards =
            AssetDatabase.FindAssets($"t:{nameof(CardData)}", new[] {"Assets/GameData/Cards"})
                .Select(AssetDatabase.GUIDToAssetPath)
                .Select(AssetDatabase.LoadAssetAtPath<CardData>)
                .OrderBy(c => c.Type).ThenBy(c => c.CardName)
                .ToList();

        EditorUtility.SetDirty(cardLibrary);
    }

    private bool IsCardDataValid()
    {
        var validSpell = _cardData.Attributes.OfType<Ability>().Any() || _cardData.Type != CardType.Spell;
        return !string.IsNullOrEmpty(_cardData.CardName)
               && _cardData.Image != null
            && _cardData.Attributes.OfType<ActionCost>().Count() == 1
            && validSpell;
    }
}