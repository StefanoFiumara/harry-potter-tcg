using System.Linq;
using HarryPotter.Data;
using HarryPotter.Data.Cards;
using HarryPotter.Data.Cards.CardAttributes;
using HarryPotter.Data.Cards.CardAttributes.Abilities;
using HarryPotter.Data.Cards.TargetSelectors;
using UnityEditor;
using UnityEditor.SceneManagement;

public static class MigrationScripts
{
        [MenuItem("Harry Potter TCG/Migrate Manual Target Data")]
        public static void MigrateManualTargetData()
        {
                var cardLibrary = AssetDatabase.LoadAssetAtPath<CardLibrary>("Assets/GameData/CardLibrary.asset");

                foreach (var card in cardLibrary.Cards)
                {
                        UpdateManualTargetData(card);
                }
                
                EditorSceneManager.SaveOpenScenes();
        }

        private static void UpdateManualTargetData(CardData card)
        {
                var oldManualTarget = card.Attributes.OfType<ManualTarget>().SingleOrDefault();
                var ability = card.Attributes.OfType<Ability>().SingleOrDefault(a => a.TargetSelector is ManualTargetSelector);

                if (oldManualTarget != null && ability != null)
                {
                        var selector = ability.TargetSelector as ManualTargetSelector;
                        if (selector != null)
                        {
                                selector.Allowed = oldManualTarget.Allowed;
                                selector.RequiredAmount = oldManualTarget.RequiredAmount;
                                selector.MaxAmount = oldManualTarget.MaxAmount;
                                
                                EditorUtility.SetDirty(card);
                        }
                }
        }
}