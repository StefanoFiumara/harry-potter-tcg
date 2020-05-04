using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using HarryPotter.Data;
using HarryPotter.Enums;
using HarryPotter.Systems;
using HarryPotter.Systems.Core;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;
using Utils;

#if UNITY_EDITOR

#endif

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

        [FormerlySerializedAs("Zones")] public Zones Zone;
        
        public bool FaceDown;
        public bool Horizontal;

        public bool ShrinkOnLargeCount;

        public int Columns = 1;
        
        public float VerticalSpacing;
        public float HorizontalSpacing;

        private IContainer Game { get; set; }

        private Player _owner;
        
        public List<CardView> Cards { get; private set; }
        
        private void Start()
        {
            var gameView = GetComponentInParent<GameViewSystem>();
            Game = gameView.Container;
            _owner = GetComponentInParent<PlayerView>().Player;

            Cards = new List<CardView>();
            
            for (var i = 0; i < _owner[Zone].Count; i++)
            {
                var card = _owner[Zone][i];
                
                foreach (var a in card.Data.Attributes)
                {
                    a.InitAttribute();
                }

                var targetRotation = GetRotationForIndex(i);
                var cardView = Instantiate(gameView.CardPrefab, GetPositionForIndex(i), Quaternion.Euler(targetRotation), transform);
                
                cardView.Card = card;

                cardView.transform.name = card.Data.CardName;
                Cards.Add(cardView);
            }
        }

        public Sequence DoZoneLayoutAnimation()
        {
            var sequence = DOTween.Sequence();

            for (var i = 0; i < Cards.Count; i++)
            {
                var cardView = Cards[i];
                // TODO: If cards are going to have a canvas to show modified attributes, set the sorting layer here in case cards overlap.
                sequence.Join(cardView.Move(GetPositionForIndex(i), GetRotationForIndex(i)));
            }

            return sequence;
        }
        
        public Vector3 GetNextPosition() => GetPositionForIndex(Cards.Count);

        private Vector3 GetPositionForIndex(int index)
        {
            var cardSize = GetCardSize();

            var horizontalSpacing = HorizontalSpacing;
            var columnCount = Columns;
            
            if (ShrinkOnLargeCount && Cards?.Count > columnCount)
            {
                horizontalSpacing = HorizontalSpacing / 1.5f;
                columnCount = Columns * 2;
            }
            
            var offset = new Vector3
            {
                x = index % columnCount * horizontalSpacing * cardSize.x,
                
                // *** Intentional loss of fraction ***
                // ReSharper disable RedundantCast
                y = (int)(index / columnCount) * VerticalSpacing * cardSize.y * -1f,
                z = (int)(index / columnCount) * -STACK_DEPTH
                // ReSharper restore RedundantCast
            };

            return transform.position + offset;
        }

        public Vector3 GetRotationForIndex(int index)
        {
            var targetY = FaceDown ? 0f : 180f;
            var targetZ = Horizontal ? 90f : 0f;

            if (_owner.Index == Game.GameState.EnemyPlayer.Index)
            {
                targetZ += 180f;
            }
            
            return new Vector3(0f, targetY, targetZ);
        }

        private Vector3 GetCardSize()
        {
            return Horizontal
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
            // if (EditorApplication.isPlaying) return;

            Gizmos.color = _zoneColors[Zone].WithAlpha(0.8f);

            var size = GetCardSize();
            size.z = STACK_DEPTH;

            for (int i = 0; i < DebugCardCount; i++)
            {
                var center = GetPositionForIndex(i);
                Gizmos.DrawCube(center, size);
                Gizmos.DrawWireCube(center, size);
            }
            
        }
#endif
        
    }
}