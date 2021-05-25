using System;
using System.Collections.Generic;
using System.Text;
using HarryPotter.Data.Cards.CardAttributes;
using HarryPotter.Enums;
using UnityEngine;

namespace HarryPotter.Data.Cards
{
    public class CardData : ScriptableObject
    {
        [HideInInspector]
        public string Id;

        [Space(10)]
        public string CardName;

        [TextArea]
        [Space(10)]
        public string CardDescription;
        
        [Space(10)]
        public Tag Tags;
        
        [Space(10)]
        [Tooltip("This field is used to compare cards for uniqueness when cards have the 'Unique' tag. If this field is empty, the Card Name will be used instead.")]
        public string UniquenessKey;
        
        [HideInInspector]
        public Sprite Image;

        [HideInInspector]
        public List<CardAttribute> Attributes = new List<CardAttribute>();

        [HideInInspector]
        public CardType Type;
        
        public Lazy<string> TooltipText { get; }

        // TEMP: Remove when all images are using HQ graphics
        public bool IsHqGraphics => Image.rect.width > 500;

        public CardData()
        {
            TooltipText = new Lazy<string>(() => SplitDescriptionText());
        }
        
        private string SplitDescriptionText(int wordsPerLine = 6)
        {
            var words = CardDescription.Split(' ');
            var splitText = new StringBuilder();

            int wordCount = 0;

            for (var i = 0; i < words.Length; i++)
            {
                string word = words[i];
                splitText.Append($"{word} ");

                // NOTE: Quick hack to prevent line breaks from being inserted inside <sprite> tags.
                if (word.StartsWith("<"))
                {
                    //IMPORTANT: Does not account for tags not separated by a space, does this matter?
                    while (!word.EndsWith(">"))
                    {
                        i++;
                        word = words[i];
                        splitText.Append($"{word} ");
                    }
                }
                
                wordCount++;

                if (wordCount > wordsPerLine)
                {
                    splitText.AppendLine();
                    wordCount = 0;
                }
            }
    
            return splitText.ToString().TrimEnd(' ', '\n');
        }
    }
}

