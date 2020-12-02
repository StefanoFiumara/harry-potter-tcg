using System.Collections.Generic;
using DG.Tweening;
using HarryPotter.Data;
using HarryPotter.Enums;
using HarryPotter.Systems;
using HarryPotter.Systems.Core;
using HarryPotter.Utils;
using UnityEngine;

namespace HarryPotter.Views
{
    public class ZoneView : MonoBehaviour
    {
        private const float STACK_DEPTH = 0.05f;
        
        private static readonly Vector3 CardSize = new Vector3
        {
            x = 5f,
            y = 7f,
            z = 0.25f
        };
        
        // TODO: May not want this hardcoded? link scene object for its transform.
        private static readonly Vector3 PilePreviewPosition = new Vector3
        {
            x = -30.2f,
            y = 15.6f,
            z = 62.7f
        };
        
        private static readonly Vector2 PilePreviewSpacing = new Vector2
        {
            x = 1.1f,
            y = 1.1f
        };

        private static readonly int PilePreviewColumnCount = 12; 
        
        public Zones Zone;
        
        public Player Owner;
        
        public bool FaceDown;
        public bool Horizontal;

        public bool ShrinkOnLargeCount;

        public int Columns = 1;
        
        public float VerticalSpacing;
        public float HorizontalSpacing;

        private IContainer _game;
        private Vector2 _cardSpacing;
        
        public List<CardView> Cards { get; private set; }
        
        private void Start()
        {
            _cardSpacing = new Vector2(HorizontalSpacing, VerticalSpacing);
            
            var gameView = GetComponentInParent<GameViewSystem>();
            _game = gameView.Container;
            
            Cards = new List<CardView>();
            
            for (var i = 0; i < Owner[Zone].Count; i++)
            {
                var card = Owner[Zone][i];
   
                foreach (var a in card.Attributes)
                {
                    a.Owner = card;
                }

                var targetRotation = GetRotation();
                var cardView = Instantiate(gameView.CardPrefab, GetPosition(i), Quaternion.Euler(targetRotation), transform);
                
                cardView.Card = card;

                cardView.transform.name = card.Data.CardName;
                Cards.Add(cardView);
            }
        }

        public Sequence GetZoneLayoutSequence(float duration = 0.5f)
        {
            var sequence = DOTween.Sequence();

            for (var i = 0; i < Cards.Count; i++)
            {
                var cardView = Cards[i];
                // TODO: If cards are going to have a canvas to show modified attributes, set the sorting layer here in case cards overlap.
                sequence.Join(cardView.Move(GetPosition(i), GetRotation(), duration));
            }

            return sequence;
        }
        
        public Sequence GetPreviewSequence(float duration = 0.5f)
        {
            var sequence = DOTween.Sequence();

            
            for (var i = 0; i < Cards.Count; i++)
            {
                var cardView = Cards[i];
                
                var targetPos = GetPosition(PilePreviewPosition, i, PilePreviewSpacing, PilePreviewColumnCount);
                var targetRot = GetRotation(isFaceDown: false, isHorizontal: false, isEnemy: false);
                
                sequence.Join(cardView.Move(targetPos, targetRot, duration));
            }
            
            return sequence;
        }
        
        private Vector3 GetPosition(int index)
        {
            return GetPosition(
                transform.position, 
                index,
                _cardSpacing, 
                Columns,
                Horizontal,
                ShrinkOnLargeCount,
                Cards.Count);
        }

        private Vector3 GetRotation()
        {
            var isEnemy = Owner.Index == _game.Match.EnemyPlayer.Index;
            
            return GetRotation(FaceDown, Horizontal, isEnemy);
        }

        private static Vector3 GetPosition(Vector3 startPosition, int index, Vector2 spacing, int columnCount, bool isHorizontal = false, bool shrinkOnLargeCount = false, int currentCardCount = 0)
        {
            var cardSize = GetCardSize(isHorizontal);

            if (shrinkOnLargeCount && currentCardCount > columnCount)
            {
                spacing.x /= 1.5f;
                columnCount *= 2;
            }
            
            var offset = new Vector3
            {
                x = index % columnCount * spacing.x * cardSize.x,
                
                // *** Intentional loss of fraction ***
                // ReSharper disable RedundantCast
                y = (int)(index / columnCount) * spacing.y * cardSize.y * -1f,
                z = (int)(index / columnCount) * -STACK_DEPTH
                // ReSharper restore RedundantCast
            };

            return startPosition + offset;
        }
        
        public static Vector3 GetRotation(bool isFaceDown, bool isHorizontal, bool isEnemy)
        {
            var targetY = isFaceDown ? 0f : 180f;
            var targetZ = isHorizontal ? 90f : 0f;

            if (isEnemy)
            {
                targetZ += 180f;
            }
            
            return new Vector3(0f, targetY, targetZ);
        }

        private static Vector3 GetCardSize(bool isHorizontal)
        {
            return isHorizontal
                ? new Vector3
                {
                    x = CardSize.y,
                    y = CardSize.x
                }
                : new Vector3
                {
                    x = CardSize.x,
                    y = CardSize.y,
                };
        }

#if UNITY_EDITOR
        public int DebugCardCount = 10;
        private readonly Dictionary<Zones, Color> _zoneColors = new Dictionary<Zones, Color>
        {
            {Zones.None,       Color.white },
            {Zones.Deck,       Color.white },
            {Zones.Discard,    Color.gray },
            {Zones.Hand,       Color.green },
            {Zones.Characters, Color.magenta },
            {Zones.Lessons,    Color.blue },
            {Zones.Creatures,  Color.red },
            {Zones.Items,      Color.cyan},
            {Zones.Location,   Color.white},
            {Zones.Match,      Color.yellow },
            {Zones.Adventure,  Color.white},
        };

        private void OnDrawGizmos()
        {
            Gizmos.color = _zoneColors[Zone].WithAlpha(0.5f);

            var size = GetCardSize(Horizontal);
            size.z = STACK_DEPTH;
            
            var cardSpacing = new Vector2(HorizontalSpacing, VerticalSpacing);

            for (int i = 0; i < DebugCardCount; i++)
            {
                var center = GetPosition(
                    transform.position, 
                    i,
                    cardSpacing, 
                    Columns,
                    Horizontal,
                    ShrinkOnLargeCount,
                    DebugCardCount);
                
                Gizmos.DrawCube(center, size);
                Gizmos.DrawWireCube(center, size);
            }
            
        }
#endif
    }
}