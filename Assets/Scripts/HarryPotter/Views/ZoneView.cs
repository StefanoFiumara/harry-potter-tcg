using System.Collections.Generic;
using System.Linq;
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
        private const float STACK_DEPTH = 0.03f;
        
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
        
        private GameView _gameView;
        private Vector2 _cardSpacing;
        private MatchData _match;
        
        public List<CardView> Cards { get; private set; }
        
        private void Start()
        {
            _cardSpacing = new Vector2(HorizontalSpacing, VerticalSpacing);
            
            _gameView = GetComponentInParent<GameView>();
            _match = _gameView.Container.GetMatch();
            
            Cards = new List<CardView>();
            
            for (var i = 0; i < Owner[Zone].Count; i++)
            {
                var card = Owner[Zone][i];
   
                foreach (var a in card.Attributes)
                {
                    a.Owner = card;
                }

                var targetRotation = GetRotation();
                var cardView = Instantiate(_gameView.CardPrefab, GetPosition(i), Quaternion.Euler(targetRotation), transform);
                
                cardView.Card = card;

                cardView.transform.name = card.Data.CardName;
                Cards.Add(cardView);
            }
        }

        public Sequence GetZoneLayoutSequence(float duration = 0.5f)
        {
            var sequence = DOTween.Sequence();
            
            foreach (var cardView in Cards)
            {
                var realIndex = Owner[Zone].IndexOf(cardView.Card);
                if (realIndex != -1) // TODO: Weird...why is this necessary again?
                {
                    sequence.Join(cardView
                            .Move(GetPosition(realIndex), GetRotation(), duration)
                            .AppendCallback(() => cardView.SetSortingLayer(realIndex))
                        );
                }
            }
            
            return sequence;
        }
        
        public Sequence GetPreviewSequence(float duration = 0.5f, PreviewSortOrder sortOrder = PreviewSortOrder.Original)
        {
            var sequence = DOTween.Sequence();

            var cardList = Cards.ToList();

            if (sortOrder == PreviewSortOrder.Shuffle)
            {
                cardList.Shuffle();
            }
            else if (sortOrder == PreviewSortOrder.ByType)
            {
                cardList = cardList
                    .OrderBy(c => c.Card.Data.Type)
                    .ThenBy(c => c.Card.GetLessonType())
                    .ToList();
            }
            
            for (var i = 0; i < cardList.Count; i++)
            {
                var cardView = cardList[i];
                
                var targetPos = GetPosition(PilePreviewPosition, i, PilePreviewSpacing, PilePreviewColumnCount);
                var targetRot = GetRotation(isFaceDown: false, isHorizontal: false, isEnemy: false);
                
                cardView.SetSortingLayer(9999); // TODO: don't hardcode this?
                sequence.Join(cardView.Move(targetPos, targetRot, duration));
            }
            
            return sequence;
        }

        public Vector3 GetPosition(CardView cardView)
        {
            var cardIndex = Cards.IndexOf(cardView);
            if (cardIndex == -1)
            {
                Debug.LogWarning($"Attempted to GetPosition on ZoneView {Zone} with card that wasn't contained in this zone.");
            }

            return GetPosition(cardIndex);
        }
        
        public Vector3 GetPosition(int index)
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

        public Vector3 GetNextPosition()
        {
            return GetPosition(Cards.Count - 1);
        }

        public static Vector3 GetPosition(Vector3 startPosition, int index, Vector2 spacing, int columnCount, bool isHorizontal = false, bool shrinkOnLargeCount = false, int currentCardCount = 0)
        {
            var cardSize = GetCardSize(isHorizontal);

            bool isShrunk = false;
            if (shrinkOnLargeCount && currentCardCount > columnCount)
            {
                spacing.x /= 1.5f;
                columnCount *= 2;
                isShrunk = true;
            }
            
            var offset = new Vector3
            {
                x = index % columnCount * spacing.x * cardSize.x,
                
                // *** Intentional loss of fraction ***
                // ReSharper disable RedundantCast
                y = (int)(index / columnCount) * spacing.y * cardSize.y * -1f,
                z = (int)(index / (isShrunk ? 1 : columnCount)) * -STACK_DEPTH,
                //z = (int)(index / columnCount) * -STACK_DEPTH
                // ReSharper restore RedundantCast
            };

            return startPosition + offset;
        }

        public Vector3 GetRotation()
        {
            var isEnemy = Owner.Index == _match.EnemyPlayer.Index;
            
            return GetRotation(FaceDown, Horizontal, isEnemy);
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