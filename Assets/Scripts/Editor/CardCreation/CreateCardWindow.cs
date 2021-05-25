using System;
using System.Linq;
using Core;
using HarryPotter.Data;
using HarryPotter.Data.Cards;
using HarryPotter.Data.Cards.CardAttributes;
using HarryPotter.Data.Cards.CardAttributes.Abilities;
using HarryPotter.Data.Cards.TargetSelectors;
using HarryPotter.Enums;
using HarryPotter.Utils;
using UnityEditor;
using UnityEngine;

namespace CardCreation
{
    public class CreateCardWindow : EditorWindow
    {
        private const string CARD_LIBRARY_PATH = "Assets/GameData/CardLibrary.asset";
        private Vector2 _scrollPos = Vector2.zero;
    
        [MenuItem("Harry Potter TCG/Build New Card")]
        private static void NewCard()
        {
            GetWindow<CreateCardWindow>(true, "Create New Card", focus: true);
        }

        private CardData _cardData;
    
        private void OnEnable()
        {
            _cardData = CreateInstance<CardData>();

            var actionCost = CreateInstance<ActionCost>();
            actionCost.PlayCost = 1;

            var lessonCost = CreateInstance<LessonCost>();
            
            _cardData.Attributes.Add(actionCost);
            _cardData.Attributes.Add(lessonCost);
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

        
            
            _scrollPos = EditorGUILayout.BeginScrollView(_scrollPos);
            
            var editor = (CardDataEditor) Editor.CreateEditor(_cardData);
            editor.IsNewCard = true;
            editor.OnInspectorGUI();
            
            EditorGUILayout.EndScrollView();
            GUILayout.EndVertical();
        }

        private void BuildCardDataAsset()
        {
            _cardData.Id = Guid.NewGuid().ToString();
            Debug.Log($"Building card with Id: {_cardData.Id}");

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

        [MenuItem("Harry Potter TCG/Update Card Library")]
        private static void UpdateCardLibrary()
        {
            var cardLibrary = AssetDatabase.LoadAssetAtPath<CardLibrary>(CARD_LIBRARY_PATH);

            var updatedCards =
                AssetDatabase.FindAssets($"t:{nameof(CardData)}", new[] {"Assets/GameData/Cards"})
                    .Select(AssetDatabase.GUIDToAssetPath)
                    .Select(AssetDatabase.LoadAssetAtPath<CardData>)
                    .SortCards()
                    .ToList();
            
            cardLibrary.UpdateCards(updatedCards);
            EditorUtility.SetDirty(cardLibrary);
        }

        private bool IsCardDataValid()
        {
            var validSpell = _cardData.Attributes.OfType<Ability>().Any() || _cardData.Type != CardType.Spell;
            return !string.IsNullOrEmpty(_cardData.CardName)
                   && _cardData.Image != null
                   && _cardData.Attributes.OfType<ActionCost>().Count() == 1
                   && _cardData.Type != CardType.None
                   && validSpell;
        }
    }
}