using System.Collections;
using System.Collections.Generic;
using Gimersia.Feature.DragAndDrop;
using Gimersia.Feature.Occupancy;
using Oneiric.Core.EventBus;
using UnityEngine;

namespace Gimersia.Gameplay
{
    [RequireComponent(typeof(Draggable))]
    [RequireComponent(typeof(Entity))]
    [RequireComponent(typeof(SpriteRenderer))]
    public sealed class EntityEmotionController : MonoBehaviour
    {
        [Header("Sprites")]
        public Sprite NormalSprite;
        public Sprite PickupSprite;
        public Sprite SatisfiedSprite;
        public Sprite DissatisfiedSprite;

        [Header("Timing")]
        [Min(0f)]
        public float EmotionDuration = 1.5f;

        Draggable _draggable;
        Entity _entity;
        SpriteRenderer _spriteRenderer;
        Coroutine _resetCoroutine;
        bool _wasDragging;
        EventBinding<EntityDropEvent> _dropBinding;

        void Awake()
        {
            _draggable = GetComponent<Draggable>();
            _entity = GetComponent<Entity>();
            _spriteRenderer = GetComponent<SpriteRenderer>();

            if (NormalSprite == null)
                NormalSprite = _spriteRenderer.sprite;
        }

        void OnEnable()
        {
            _dropBinding = EventBus.Bind<EntityDropEvent>(OnEntityDropped);
        }

        void OnDisable()
        {
            _dropBinding?.Dispose();
            _dropBinding = null;

            if (_resetCoroutine != null)
            {
                StopCoroutine(_resetCoroutine);
                _resetCoroutine = null;
            }
        }

        void Update()
        {
            if (_draggable.IsDragging && !_wasDragging)
                SetEmotion(PickupSprite);

            _wasDragging = _draggable.IsDragging;
        }

        void OnEntityDropped(EntityDropEvent dropEvent)
        {
            if (dropEvent.Entity != _entity)
                return;

            if (!dropEvent.Accepted || dropEvent.Destination == null)
            {
                ShowTemporaryEmotion(DissatisfiedSprite);
                return;
            }

            var violations = new List<Violation>();
            if (dropEvent.Destination.Grid.GetViolations(_entity.occupant, violations))
                ShowTemporaryEmotion(DissatisfiedSprite);
            else
                ShowTemporaryEmotion(SatisfiedSprite);
        }

        void ShowTemporaryEmotion(Sprite emotionSprite)
        {
            SetEmotion(emotionSprite);

            if (_resetCoroutine != null)
                StopCoroutine(_resetCoroutine);

            _resetCoroutine = StartCoroutine(ResetAfterDelay());
        }

        IEnumerator ResetAfterDelay()
        {
            yield return new WaitForSeconds(EmotionDuration);
            SetEmotion(NormalSprite);
            _resetCoroutine = null;
        }

        void SetEmotion(Sprite emotionSprite)
        {
            _spriteRenderer.sprite = emotionSprite != null
                ? emotionSprite
                : NormalSprite != null
                    ? NormalSprite
                    : _spriteRenderer.sprite;
        }
    }
}
