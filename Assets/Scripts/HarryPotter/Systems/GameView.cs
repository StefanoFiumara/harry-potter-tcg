using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using HarryPotter.Data;
using HarryPotter.Data.Cards;
using HarryPotter.Enums;
using HarryPotter.GameActions;
using HarryPotter.GameActions.GameFlow;
using HarryPotter.Input.Controllers;
using HarryPotter.Systems.Core;
using HarryPotter.Utils;
using HarryPotter.Views;
using HarryPotter.Views.UI.ParticleSystemUtils;
using UnityEngine;

namespace HarryPotter.Systems
{
    [RequireComponent(typeof(InputSystem))]
    [RequireComponent(typeof(BoardView))]
    [RequireComponent(typeof(HandView))]
    [RequireComponent(typeof(HealingView))]
    public class GameView : MonoBehaviour, IGameSystem
    {
        public MatchData Match;
        public GameSettings Settings;
        public CardView CardPrefab;
        
        private ParticleSystemController _particlesController;
        private ActionSystem _actionSystem;
        
        private Dictionary<(int PlayerIndex, Zones Zone), ZoneView> _zoneViews;
        
        // Animation Constants
        private static readonly Vector3 SingleRevealPosition = new Vector3(0f, 0f, 40f);
        private static readonly Vector3 RevealRotation = new Vector3(0f, 180f, 0f);
        
        private static readonly Vector3 MultipleRevealPosition = new Vector3
        {
            x = -17.5f,
            y = 0f,
            z = 65f
        };
        
        private static readonly Vector2 MultipleRevealSpacing = new Vector2
        {
            x = 1.1f,
            y = 1.1f
        };

        private const int MULTIPLE_REVEAL_COLUMN_COUNT = 7;
        
        // Game State Container
        private IContainer _container;
        public IContainer Container
        {
            get
            {
                if (_container == null)
                {
                    _container = GameFactory.Create(Match, Settings);
                    _container.AddSystem(this);
                }

                return _container;
            }
            
            set => _container = value;
        }
        
        //NOTE: We may want to use a different kind of input system in the future, extract interface?
        public InputSystem Input { get; private set; }
        public bool IsIdle => !_actionSystem.IsActive && !Container.IsGameOver();
        
        private void Awake()
        {
            DOTween.Init().SetCapacity(50, 10);
            DOTween.timeScale = Settings.TweenTimescale;
            
            Input = GetComponent<InputSystem>();

            _zoneViews = GetComponentsInChildren<ZoneView>()
                .GroupBy(z => (z.Owner.Index, z.Zone))
                .ToDictionary(g => g.Key, g => g.Single());

            _particlesController = GetComponentInChildren<ParticleSystemController>();

            _actionSystem = Container.GetSystem<ActionSystem>();
            
            if (Match == null || Input == null || _particlesController == null)
            {
                Debug.LogError("ERROR: GameView is missing some dependencies!");
                return;
            }
            
            Container.Awake();
        }

        private void Start()
        {
            SetupSinglePlayer();
        }
        
        private void SetupSinglePlayer() 
        {
            Debug.Log("*** BEGIN GAME ***");
            
            var beginGame = new BeginGameAction();
            _container.Perform(beginGame);
        }

        private void Update()
        {
            _actionSystem.Update();
        }

        private void OnDestroy()
        {
            Container.Destroy();
        }
        
        public ZoneView FindZoneView(Player player, Zones zone) => _zoneViews[(player.Index, zone)];
        
        public CardView FindCardView(Card card) => _zoneViews.Values
                                                        .Where(z => z.Owner == card.Owner)
                                                        .SelectMany(z => z.Cards)
                                                        .Single(cv => cv.Card == card);
        
        public List<CardView> FindCardViews(List<Card> cards) => _zoneViews.Values
                                                                    .SelectMany(z => z.Cards)
                                                                    .Where(cv => cards.Contains(cv.Card))
                                                                    .OrderBy(cv => cv.ZoneIndex)
                                                                    .ToList();

        public Sequence GetParticleSequence(GameAction action, Card target)
        {
            return GetParticleSequence(action, new List<Card> {target});
        }

        public Sequence GetParticleSequence(GameAction action, List<Card> targets)
        {
            var particleSequence = DOTween.Sequence();

            if (action.SourceCard == null)
            {
                Debug.LogWarning("Called GetParticleSequence with action.SourceCard == null");
            }

            if (targets.Count > 1)
            {
                if (targets.All(t => t.Zone == Zones.Deck))
                {
                    return GetParticleSequence(action, Zones.Deck);
                }

                if (targets.All(t => t.Zone == Zones.Discard))
                {
                    return GetParticleSequence(action, Zones.Discard);
                }
            }

            foreach (var target in targets)
            {
                // NOTE: This check needs to be done so that spell cards that discard themselves don't do the particle animation
                if (target == action.SourceCard)
                {
                    continue;
                }
                
                var particleType = action.SourceCard?.GetLessonType() ?? LessonType.None;
                
                var sequence = action.SourceCard?.Data.Type.IsHorizontal() == true
                    ? GetParticleSequence(action.SourceCard, target)
                    : GetParticleSequence(action.Player, target, particleType);

                particleSequence.Append(sequence);
            }
            
            return particleSequence;
        }

        public Sequence GetParticleSequence(GameAction action, Zones target)
        {
            return GetParticleSequence(action, action.Player[target].Last());
        }

        private Sequence GetParticleSequence(Player source, Card target, LessonType particleColorType)
        {
            var startPosLocal = new Vector3(0f, -18.5f, 50f); // For targeting enemy
            var startPosEnemy = new Vector3(0f, 21.5f, 50f); // For targeting local

            var startPos = source == Match.LocalPlayer
                ? startPosLocal
                : startPosEnemy;

            var targetPos = CalculateParticleTargetPos(target);

            return GetParticleSequence(startPos, targetPos, particleColorType);
        }
        
        private Sequence GetParticleSequence(Card source, Card target)
        {
            var sourceView = FindCardView(source);
            
            var startPos  = sourceView.transform.position + 0.5f * Vector3.back;
            var targetPos = CalculateParticleTargetPos(target);

            var particleColorType = sourceView.Card.GetLessonType();

            return GetParticleSequence(startPos, targetPos, particleColorType);
        }

        private Sequence GetParticleSequence(Vector3 startPos, Vector3 endPos, LessonType particleColorType)
        {
            _particlesController.SetParticleColor(particleColorType);

            return DOTween.Sequence()
                .AppendCallback(() => _particlesController.Play())
                .Append(_particlesController.transform.DOMove(startPos, 0f))
                .Append(_particlesController.transform.DOMove(endPos, 1.5f).SetEase(Ease.OutQuint))
                .AppendCallback(() => _particlesController.Stop());
        }

        private Vector3 CalculateParticleTargetPos(Card target)
        {
            var zoneView = FindZoneView(target.Owner, target.Zone);
            var targetView = FindCardView(target);

            return target.Zone.HasZone(Zones.Deck | Zones.Discard) 
                ? zoneView.GetNextPosition() + 0.5f * Vector3.back 
                : zoneView.GetPosition(targetView) + 0.5f * Vector3.back;
        }
        
        public Sequence GetRevealSequence(CardView target, Zones to, Zones from, float duration = 0.5f)
        {
            var endZoneView = FindZoneView(target.Card.Owner, to);
            
            target.SetSortingLayer(9999);
            
            var previewSequence = DOTween.Sequence()
                .Append(target.Move(SingleRevealPosition, RevealRotation, duration));

            ChangeZoneView(target, to, from);
            
            if (from != Zones.None)
            {
                var startZoneView = FindZoneView(target.Card.Owner, from);
                previewSequence.Join(startZoneView.GetZoneLayoutSequence(duration));
            }

            var finalPos = endZoneView.GetNextPosition();
            var finalRot = endZoneView.GetRotation();
            
            return previewSequence
                .AppendInterval(duration)
                .Append(target.Move(finalPos, finalRot, duration))
                .Join(endZoneView.GetZoneLayoutSequence(duration));
        }

        // TODO: Very similar code in HealingView.GetHealingSequence, possible to consolidate?
        public Sequence GetRevealSequence(List<CardView> targets, Zones to, List<Zones> from, float duration = 0.5f)
        {
            var revealSequence = DOTween.Sequence();
            var animationTime = 0f;
            
            // TODO: Do we need to support multiple players in this function?
            var player = targets[0].Card.Owner;
            
            var affectedViews = new List<ZoneView>()
            {
                FindZoneView(player, to)
            };

            var revealPos = targets.Count > 1 ? MultipleRevealPosition : SingleRevealPosition;
            
            for (var i = 0; i < targets.Count; i++)
            {
                var target = targets[i];
                var fromZone = from[i];

                var targetPos = ZoneView.GetPosition(revealPos, i, MultipleRevealSpacing, MULTIPLE_REVEAL_COLUMN_COUNT);
                var targetRot = ZoneView.GetRotation(isFaceDown: false, isHorizontal: false, isEnemy: false);
                
                revealSequence.Insert(animationTime, target.Move(targetPos, targetRot, duration));
                
                ChangeZoneView(target, to, fromZone);
                animationTime += 0.5f;
            }

            revealSequence
                .AppendInterval(0.25f)
                .Append(affectedViews.First().GetZoneLayoutSequence(duration));
            
            foreach (var zoneView in affectedViews.Skip(1))
            {
                revealSequence.Join(zoneView.GetZoneLayoutSequence(duration));
            }
            
            return revealSequence;
        }
        
        public Sequence GetMoveToZoneSequence(CardView cardView, Zones to, Zones from)
        {
            var pairs = new List<(CardView, Zones)>
            {
                (cardView, to)
            };
            
            return GetMoveToZoneSequence(pairs, from);
        }

        public Sequence GetMoveToZoneSequence(List<(CardView, Zones)> cardViewPairs, Zones from)
        {
            var affectedZones = new HashSet<ZoneView>();
            
            foreach (var (card, zone) in cardViewPairs)
            {
                if (zone == Zones.None)
                {
                    break;
                }
                
                var affected = ChangeZoneView(card, zone, from);

                foreach (var zoneView in affected)
                {
                    affectedZones.Add(zoneView);
                }
            }
            
            var sequence = DOTween.Sequence();
            
            foreach (var zoneView in affectedZones)
            {
                sequence = sequence.Join(zoneView.GetZoneLayoutSequence());
            }

            return sequence;
        }

        public List<ZoneView> ChangeZoneView(CardView card, Zones to, Zones from)
        {
            var result = new List<ZoneView>();
            var actualFrom = from != Zones.None ? from : card.Card.Zone;

            if (actualFrom != Zones.None)
            {
                var fromZone = FindZoneView(card.Card.Owner, actualFrom);
                if (!fromZone.Cards.Remove(card))
                {
                    Debug.LogWarning($"{card.Card.Data.CardName} was not removed from zone {actualFrom}");
                    Debug.Break();
                }
                result.Add(fromZone);
            }

            var toZone = FindZoneView(card.Card.Owner, to);
            if (!toZone.Cards.Contains(card))
            {
                toZone.Cards.Add(card);
            }
            
            result.Add(toZone);
            card.transform.SetParent(toZone.transform);
            
            return result;
        }
    }
}