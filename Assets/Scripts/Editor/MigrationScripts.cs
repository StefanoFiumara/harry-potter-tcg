using HarryPotter.Data;
using HarryPotter.Data.Cards;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

public static class MigrationScripts
{
        [MenuItem("Harry Potter TCG/Migrations/Update Card Data")]
        private static void ReSaveCardData()
        {
                var cardLibrary = AssetDatabase.LoadAssetAtPath<CardLibrary>("Assets/GameData/CardLibrary.asset");
                
                foreach (var card in cardLibrary.Cards)
                { 
                        EditorUtility.SetDirty(card);
                }
                
                AssetDatabase.SaveAssets();
                EditorSceneManager.SaveOpenScenes();
        }

        [MenuItem("Harry Potter TCG/Migrations/Print Missing Sprites")]
        private static void PrintMissingSprites()
        {
                var cardLibrary = AssetDatabase.LoadAssetAtPath<CardLibrary>("Assets/GameData/CardLibrary.asset");
                
                foreach (var card in cardLibrary.Cards)
                {
                        if (card.Image == null)
                        {
                                Debug.LogWarning(card.CardName);
                        }
                }
                
                //AssetDatabase.SaveAssets();
                //EditorSceneManager.SaveOpenScenes();
        }
        
        // [MenuItem("Harry Potter TCG/Migrate Manual Target Data")]
        private static void MigrateManualTargetData()
        {
                // NOTE: Migration Complete
                
                // var cardLibrary = AssetDatabase.LoadAssetAtPath<CardLibrary>("Assets/GameData/CardLibrary.asset");
                //
                // foreach (var card in cardLibrary.Cards)
                // {
                //         UpdateManualTargetData(card);
                // }
                //
                // AssetDatabase.SaveAssets();
                // EditorSceneManager.SaveOpenScenes();
        }

        private static void UpdateManualTargetData(CardData card)
        {
                // var oldManualTarget = card.Attributes.OfType<ManualTarget>().SingleOrDefault();
                // var ability = card.Attributes.OfType<Ability>().SingleOrDefault(a => a.TargetSelector is ManualTargetSelector);
                //
                // if (oldManualTarget != null && ability != null)
                // {
                //         var selector = ability.TargetSelector as ManualTargetSelector;
                //         if (selector != null)
                //         {
                //                 selector.Allowed = oldManualTarget.Allowed;
                //                 selector.RequiredAmount = oldManualTarget.RequiredAmount;
                //                 selector.MaxAmount = oldManualTarget.MaxAmount;
                //
                //
                //                 card.Attributes.Remove(oldManualTarget);
                //                 AssetDatabase.RemoveObjectFromAsset(oldManualTarget);
                //                 EditorUtility.SetDirty(card);
                //         }
                // }
        }
}