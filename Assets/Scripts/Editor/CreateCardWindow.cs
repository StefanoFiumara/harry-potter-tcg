using System;
using System.Linq;
using HarryPotter.Data.Cards;
using HarryPotter.Data.Cards.CardAttributes;
using HarryPotter.Data.Cards.CardAttributes.Abilities;
using HarryPotter.Enums;
using UnityEditor;
using UnityEngine;
using Utils;

// ReSharper disable once CheckNamespace
public class CreateCardWindow : EditorWindow
{
    [MenuItem("Harry Potter TCG/New Card Asset")]
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
        
        GUI.backgroundColor = EditorColors.Success;
        GUI.enabled = IsCardDataValid();
        if (GUILayout.Button("Create Card"))
        {
            BuildCardDataAsset();
            Close();
        }

        GUI.enabled = true;
        GUI.backgroundColor = Color.white;
        
        var editor = (CardDataEditor) Editor.CreateEditor(_cardData);
        editor.IsEditMode = true;
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

            if (attribute is Ability abilityAttribute)
            {
                if (abilityAttribute.TargetSelector != null)
                {
                    AssetDatabase.AddObjectToAsset(abilityAttribute.TargetSelector, abilityAttribute);
                }
            }
        }
        
        AssetDatabase.ImportAsset(AssetDatabase.GetAssetPath(_cardData));

        Debug.Log($"Created Card Asset at: {AssetDatabase.GetAssetPath(_cardData)}");
    }

    private bool IsCardDataValid()
    {
        var validSpell = _cardData.GetAttributes<Ability>().Count > 0 || _cardData.Type != CardType.Spell;
        return !string.IsNullOrEmpty(_cardData.CardName)
               && _cardData.Image != null
            && _cardData.Attributes.OfType<ActionCost>().Count() == 1
            && validSpell;
    }
}