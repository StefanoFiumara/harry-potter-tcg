using System;
using Core;
using HarryPotter.Data;
using HarryPotter.Data.Cards;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace CardCreation
{
    [CustomEditor(typeof(GameSettings))]
    public class GameSettingsEditor : Editor
    {
        private GameSettings _settings;

        private ReorderableList _localDeck;
        private ReorderableList _enemyDeck;
        
        
        private void OnEnable()
        {
            try
            {
                _settings = (GameSettings) target;
                _localDeck = new ReorderableList(serializedObject, serializedObject.FindProperty(nameof(_settings.LocalDeck)), true, true, true, true);
                _enemyDeck = new ReorderableList(serializedObject, serializedObject.FindProperty(nameof(_settings.AIDeck)), true, true, true, true);

                _localDeck.drawElementCallback = (rect, index, active, focused) =>
                {
                    rect.y += 1;
                    var element = _localDeck.serializedProperty.GetArrayElementAtIndex(index);
                    EditorGUI.PropertyField(
                            new Rect(rect.x, rect.y, EditorGUIUtility.currentViewWidth-70, EditorGUIUtility.singleLineHeight),
                            element, GUIContent.none);
                        
                };

                _localDeck.drawHeaderCallback = rect =>
                {
                    EditorGUI.LabelField(rect, "Player Deck");
                };
                
                _enemyDeck.drawElementCallback = (rect, index, active, focused) =>
                {
                    rect.y += 1;
                    var element = _enemyDeck.serializedProperty.GetArrayElementAtIndex(index);
                    EditorGUI.PropertyField(
                        new Rect(rect.x, rect.y, EditorGUIUtility.currentViewWidth-70, EditorGUIUtility.singleLineHeight),
                        element, GUIContent.none);
                        
                };

                _enemyDeck.drawHeaderCallback = rect =>
                {
                    EditorGUI.LabelField(rect, "Enemy Deck");
                };


            }
            catch (Exception) { /* Do Nothing */ }
        }

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            if (_settings.DebugMode)
            {
                E.DrawLine(Color.gray);
                GUILayout.Label("DEBUG Options", EditorStyles.largeLabel);
                GUILayout.Label("Overrides", EditorStyles.miniBoldLabel);
                
                E.Button("Swap Decks", E.Colors.Action, SwapDebugDecks);

                _settings.OverridePlayerDeck = EditorGUILayout.Toggle("Override Player Deck", _settings.OverridePlayerDeck);

                if (_settings.OverridePlayerDeck)
                {
                    _settings.LocalStarting = (CardData) EditorGUILayout.ObjectField(
                        "Player Starting Character",
                        _settings.LocalStarting, 
                        typeof(CardData),
                        allowSceneObjects: false);
                
                    GUILayout.Space(5);
                    _localDeck.DoLayoutList();
                }
                
                _settings.OverrideAIDeck = EditorGUILayout.Toggle("Override AI Deck", _settings.OverrideAIDeck);

                if (_settings.OverrideAIDeck)
                {
                    _settings.AIStarting = (CardData) EditorGUILayout.ObjectField(
                        "AI Starting Character",
                        _settings.AIStarting, 
                        typeof(CardData),
                        allowSceneObjects: false);
                
                    GUILayout.Space(5);
                    _enemyDeck.DoLayoutList();
                }
                
                serializedObject.ApplyModifiedProperties();
            }
        }

        public void SwapDebugDecks()
        {
            var tempDeck = _settings.LocalDeck;
            _settings.LocalDeck = _settings.AIDeck;
            _settings.AIDeck = tempDeck;


            var tempChar = _settings.LocalStarting;
            _settings.LocalStarting = _settings.AIStarting;
            _settings.AIStarting = tempChar;
        }
    }
}