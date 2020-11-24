using System.Linq;
using HarryPotter.Data;
using HarryPotter.Data.Cards;
using UnityEditor;

// ReSharper disable once CheckNamespace
public static class CardLibraryGeneration
{
    private const string CARD_LIBRARY_PATH = "Assets/GameData/CardLibrary.asset";
    
    [MenuItem("Harry Potter TCG/Update Card Library")]
    public static void GenerateCardLibrary()
    {
        var cardLibrary = AssetDatabase.LoadAssetAtPath<CardLibrary>(CARD_LIBRARY_PATH);

        cardLibrary.Cards =
            AssetDatabase.FindAssets($"t:{nameof(CardData)}", new[] {"Assets/GameData/Cards"})
                .Select(AssetDatabase.GUIDToAssetPath)
                .Select(AssetDatabase.LoadAssetAtPath<CardData>)
                .OrderBy(c => c.Type)
                .ToList();
        
        EditorUtility.SetDirty(cardLibrary);
    }
}