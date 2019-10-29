using System.Collections.Generic;
using HarryPotter.Enums;
using HarryPotter.Game.Cards;
using UnityEditor;
using UnityEngine;
using Utils;
#if UNITY_EDITOR

#endif

namespace HarryPotter.Game.Player
{
    public class CardZone : MonoBehaviour
    {
        private const float STACK_DEPTH = 0.05f;
        
        //TODO: Temporary?
        private static readonly Vector3 CARD_SIZE = new Vector3
        {
            x = 5f,
            y = 7f,
            z = 0.25f
        };

        public ZoneType ZoneType;

        public bool FaceDown;
        public bool Horizontal;

        public float Columns = 1;
        
        public float VerticalSpacing;
        public float HorizontalSpacing;

#if UNITY_EDITOR
        public int DebugCardCount = 10;
        private readonly Dictionary<ZoneType, Color> _zoneColors = new Dictionary<ZoneType, Color>
        {
            {ZoneType.Deck,       Color.gray },
            {ZoneType.Discard,    Color.gray },
            {ZoneType.Hand,       Color.green },
            {ZoneType.Characters, Color.magenta },
            {ZoneType.Lessons,    Color.blue },
            {ZoneType.Creatures,  Color.red },
            {ZoneType.Items,      Color.cyan},
            {ZoneType.Location,   Color.white},
            {ZoneType.Match,      Color.yellow },
            {ZoneType.Adventure,  Color.white},
        };

        private void OnDrawGizmos()
        {
            if (EditorApplication.isPlaying) return;

            Gizmos.color = _zoneColors[ZoneType].WithAlpha(0.8f);

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
                    x = CARD_SIZE.y,
                    y = CARD_SIZE.x
                }
                : new Vector3
                {
                    x = CARD_SIZE.x,
                    y = CARD_SIZE.y,
                };
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
            
            return new Vector3(0f, targetY, targetZ);
        }
    }
}