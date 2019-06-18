using System.Collections.Generic;
using System.Linq;
using HarryPotter.Enums;
using HarryPotter.Game.Cards;
using UnityEditor;
using UnityEngine;
using Utils;
#if UNITY_EDITOR

#endif

namespace HarryPotter.Game.Player
{
    public class ZoneView : MonoBehaviour
    {
        private const float STACK_DEPTH = 0.1f;

        public Zone Zone;

        public bool FaceDown;
        public bool Horizontal;

        public float Columns = 1;
        
        public float VerticalSpacing;
        public float HorizontalSpacing;

        private PlayerView _player;
        public List<CardView> Cards;

        private void Awake()
        {
            _player = GetComponentInParent<PlayerView>();

            if (_player == null)
            {
                throw new UnityException($"ZoneView for {Zone} could not find PlayerView in parent.");
            }

            Cards = new List<CardView>();
        }

        #if UNITY_EDITOR
        public int DebugCardCount = 10;
        private readonly Dictionary<Zone, Color> _zoneColors = new Dictionary<Zone, Color>
        {
            {Zone.Deck,       Color.gray },
            {Zone.Discard,    Color.gray },
            {Zone.Hand,       Color.green },
            {Zone.Characters, Color.magenta },
            {Zone.Lessons,    Color.blue },
            {Zone.Creatures,  Color.red },
            {Zone.Items,      Color.cyan},
            {Zone.Location,   Color.white},
            {Zone.Match,      Color.yellow },
            {Zone.Adventure,  Color.white},
        };

        private void OnDrawGizmos()
        {
            if (EditorApplication.isPlaying) return;

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

        private Vector3 GetCardSize()
        {
            return Horizontal
                ? new Vector3
                {
                    x = CardView.CARD_SIZE.y,
                    y = CardView.CARD_SIZE.x
                }
                : new Vector3
                {
                    x = CardView.CARD_SIZE.x,
                    y = CardView.CARD_SIZE.y,
                };
        }

        public Vector3 GetNextPosition()
        {
            var nextIndex = Cards.Count;

            return GetPositionForIndex(nextIndex);
        }

        public Vector3 GetPositionForIndex(int index)
        {
            var cardSize = GetCardSize();

            var offset = new Vector3
            {
                x = index % Columns * HorizontalSpacing * cardSize.x,
                y = (int)(index / Columns) * VerticalSpacing * cardSize.y * -1f,
                z = (int)(index / Columns) * -STACK_DEPTH
            };

            return transform.position + offset;
        }

        public Vector3 GetTargetRotation()
        {
            var targetY = FaceDown ? 180f : 0f;
            var targetZ = Horizontal ? 270f : 0f;

            //TODO: See if the rotation can happen in local space so that we can eliminate the reference to PlayerView
            if (_player.transform.rotation != Quaternion.identity)
            {
                targetZ = Horizontal ? 90f : 180f;
            }

            return new Vector3(0f, targetY, targetZ);
        }
    }
}