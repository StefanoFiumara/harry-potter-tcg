using System;
using System.Collections.Generic;
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
        
        //TODO: Temporary?
        private static readonly Vector3 CARD_SIZE = new Vector3
        {
            x = 5f,
            y = 7f,
            z = 0.25f
        };

        [FormerlySerializedAs("Zones")] public Zones Zone;
        
        public bool FaceDown;
        public bool Horizontal;

        public float Columns = 1;
        
        public float VerticalSpacing;
        public float HorizontalSpacing;

        private IContainer Game { get; set; }

        private Player _owner;
        
        private void Start()
        {
            Game = GetComponentInParent<GameViewSystem> ().Container;

            _owner = GetComponentInParent<PlayerView>().Player;

            if (_owner[Zone].Count > 0)
            {
                Debug.Log($"Player: {_owner.Id}, Zone: {Zone}, CardCount: {_owner[Zone].Count}");
            }
            
            //TODO: Instantiate cards from: _owner[Zone]
        }

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
        
#if UNITY_EDITOR
        public int DebugCardCount = 10;
        private readonly Dictionary<Zones, Color> _zoneColors = new Dictionary<Zones, Color>
        {
            {Zones.Deck,       Color.gray },
            {Zones.Discard,    Color.black },
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
    }
}