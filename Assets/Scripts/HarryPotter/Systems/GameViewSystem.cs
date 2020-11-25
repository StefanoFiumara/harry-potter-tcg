using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using HarryPotter.Data;
using HarryPotter.Data.Cards;
using HarryPotter.Data.Cards.CardAttributes;
using HarryPotter.Enums;
using HarryPotter.GameActions.GameFlow;
using HarryPotter.Input.Controllers;
using HarryPotter.Systems.Core;
using HarryPotter.UI.Cursor;
using HarryPotter.UI.Tooltips;
using HarryPotter.Utils;
using HarryPotter.Views;
using UnityEngine;

namespace HarryPotter.Systems
{
    [RequireComponent(typeof(BoardView))]
    [RequireComponent(typeof(HandView))]
    public class GameViewSystem : MonoBehaviour, IGameSystem
    {
        public MatchData Match;
        public CardView CardPrefab;
        
        // TODO: Put this into a Config ScriptableObject so it can be configured by the user when we build the options menu
        public float TweenTimescale = 4f; 
        public TooltipController Tooltip { get; private set; }
        
        public CursorController Cursor { get; private set; }
        
        //NOTE: We may want to use a different kind of input controller in the future
        public ClickToPlayCardController Input { get; private set; }

        private ParticleSystem _particleSystem;
        private ActionSystem _actionSystem;
        
        private Dictionary<(int PlayerIndex, Zones Zone), ZoneView> _zoneViews;

        private IContainer _container;
        public IContainer Container
        {
            get
            {
                if (_container == null)
                {
                    _container = GameFactory.Create(Match);
                    _container.AddSystem(this);
                }

                return _container;
            }
            
            set => _container = value;
        }
        
        public bool IsIdle => !_actionSystem.IsActive && !Container.IsGameOver();

        private void Awake()
        {
            DOTween.Init().SetCapacity(50, 10);
            DOTween.timeScale = TweenTimescale;
            
            Tooltip = GetComponentInChildren<TooltipController>();
            Cursor = GetComponentInChildren<CursorController>();
            
            Input = GetComponent<ClickToPlayCardController>();

            _zoneViews = GetComponentsInChildren<ZoneView>()
                .GroupBy(z => (z.Owner.Index, z.Zone))
                .ToDictionary(g => g.Key, g => g.Single());

            _particleSystem = GetComponentInChildren<ParticleSystem>();
            _particleSystem.Stop();
            
            _actionSystem = Container.GetSystem<ActionSystem>();
            
            if (Match == null || Tooltip == null || Cursor == null || Input == null || _particleSystem == null)
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
            Match.Players[0].ControlMode = ControlMode.Local;
            Match.Players[1].ControlMode = ControlMode.Computer;
            
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
        
        public CardView FindCardView(Card card) => FindCardViews(new List<Card> { card }).Single();
        
        public List<CardView> FindCardViews(List<Card> cards) => _zoneViews.Values.SelectMany(z => z.Cards).Where(cv => cards.Contains(cv.Card)).ToList();
        
        public Sequence GetParticleSequence(Player source, Card target, LessonType particleColorType)
        {
            var targetView = FindCardView(target);
            
            var startPosLocal = new Vector3(0f, -18.5f, 50f); // For targeting enemy
            var startPosEnemy = new Vector3(0f, 21.5f, 50f); // For targeting local

            var startPos = source == Match.LocalPlayer
                ? startPosLocal
                : startPosEnemy;

            var targetPos = targetView.transform.position + 0.5f * Vector3.back;

            return GetParticleSequence(startPos, targetPos, particleColorType);
        }
        
        public Sequence GetParticleSequence(Card source, Card target)
        {
            var sourceView = FindCardView(source);
            var targetView = FindCardView(target);

            var startPos = sourceView.transform.position + 0.5f * Vector3.back;
            var targetPos = targetView.transform.position + 0.5f * Vector3.back;

            var particleColorType = sourceView.Card.GetAttribute<LessonCost>().Type;

            return GetParticleSequence(startPos, targetPos, particleColorType);

        }

        private Sequence GetParticleSequence(Vector3 startPos, Vector3 endPos, LessonType particleColorType)
        {
            _particleSystem.SetParticleColorGradient(particleColorType);

            return DOTween.Sequence()
                .AppendCallback(() => _particleSystem.Play())
                .Append(_particleSystem.transform.DOMove(startPos, 0f))
                .Append(_particleSystem.transform.DOMove(endPos, 1.25f).SetEase(Ease.OutQuint))
                .AppendCallback(() => _particleSystem.Stop());
        }
        
        public IEnumerator MoveToZoneAnimation(CardView cardView, Zones to, Zones from = Zones.None)
        {
            var pairs = new List<(CardView, Zones)>
            {
                (cardView, to)
            };
            
            return MoveToZoneAnimation(pairs, from);
        }

        public IEnumerator MoveToZoneAnimation(List<(CardView, Zones)> cardViewPairs, Zones from = Zones.None)
        {
            var affectedZones = new HashSet<ZoneView>();
            
            foreach (var (card, zone) in cardViewPairs)
            {
                if (zone == Zones.None)
                {
                    yield break;
                }
                
                var affected = ChangeZoneView(card, zone, from);

                foreach (var zoneView in affected)
                {
                    affectedZones.Add(zoneView);
                }
            }
            
            // TODO: We might not want to rely on DoZoneLayoutAnimation to move cards between zones.
            //       It makes it difficult to do more custom animations from one zone to the other.
            var sequence = DOTween.Sequence();
            
            foreach (var zoneView in affectedZones)
            {
                sequence = sequence.Join(zoneView.DoZoneLayoutAnimation());
            }

            yield return true;
            
            while (sequence.IsPlaying())
            {
                yield return null;
            }
        }

        public List<ZoneView> ChangeZoneView(CardView card, Zones to, Zones from = Zones.None)
        {
            var result = new List<ZoneView>();
            var fromTemp = from != Zones.None ? from : card.Card.Zone;

            if (fromTemp != Zones.None)
            {
                var fromZone = FindZoneView(card.Card.Owner, fromTemp);
                fromZone.Cards.Remove(card);
                result.Add(fromZone);
            }
            
            var toZone = FindZoneView(card.Card.Owner, to);
            toZone.Cards.Add(card);
            result.Add(toZone);
            
            card.transform.SetParent(toZone.transform);
            return result;
        }
    }
}