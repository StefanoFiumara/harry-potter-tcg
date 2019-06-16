﻿using System.Collections.Generic;
using System.Linq;
using HarryPotter.Enums;
using UnityEngine;
using Utils;
using Quaternion = UnityEngine.Quaternion;
using Vector3 = UnityEngine.Vector3;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace HarryPotter.UI
{
    public class ZoneUI : MonoBehaviour
    {
        private const float STACK_DEPTH = 0.1f;

        public Zone Zone;

        public bool FaceDown;
        public bool Horizontal;

        public float Columns = 1;
        
        public float VerticalSpacing;
        public float HorizontalSpacing;

        private PlayerUI _player;

        private void Awake()
        {
            _player = GetComponentInParent<PlayerUI>();

            if (_player == null)
            {
                throw new UnityException($"ZoneUI for {Zone} could not find PlayerUI in parent.");
            }
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
                    x = CardUI.CARD_SIZE.y,
                    y = CardUI.CARD_SIZE.x
                }
                : new Vector3
                {
                    x = CardUI.CARD_SIZE.x,
                    y = CardUI.CARD_SIZE.y,
                };
        }

        public Vector3 GetNextPosition()
        {
            var nextIndex = _player.Cards.Count(c => c.State.Zone == Zone);

            return GetPositionForIndex(nextIndex);
        }

        private Vector3 GetPositionForIndex(int index)
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

        public Quaternion GetTargetRotation()
        {
            var targetY = FaceDown ? 180f : 0f;
            var targetZ = Horizontal ? 270f : 0f;

            return Quaternion.Euler(0f, targetY, targetZ);
        }
    }
}