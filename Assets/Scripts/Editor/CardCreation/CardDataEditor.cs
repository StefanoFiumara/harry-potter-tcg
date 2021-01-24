using System;
using System.Linq;
using Core;
using HarryPotter.Data.Cards;
using HarryPotter.Data.Cards.CardAttributes;
using HarryPotter.Enums;
using HarryPotter.Utils;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace CardCreation
{
    [CustomEditor(typeof(CardData))]
    public class CardDataEditor : Editor
    {
        private CardData _cardData;

        public bool IsNewCard { get; set; } = false;

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
            if (!IsNewCard)
            {
                E.Button("Save Changes", E.Colors.Success, () =>
                {
                    EditorUtility.SetDirty(_cardData);
                    AssetDatabase.SaveAssets();
                    EditorSceneManager.SaveOpenScenes();
                });
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
                var attribute = _cardData.Attributes[i];
                var editor = CreateEditor(attribute);
            
                GUILayout.BeginHorizontal();
                GUILayout.BeginVertical();
            
                editor.OnInspectorGUI();
            
                GUILayout.EndVertical();
            
                if (attribute.GetType() != typeof(ActionCost))
                {
                    E.RemoveButton(_cardData.Attributes, i);
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
}