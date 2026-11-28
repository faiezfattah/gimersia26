using LitMotion;
using LitMotion.Extensions;
using UnityEngine;
using UnityEngine.Rendering;

namespace Gimersia.Feature.DragAndDrop
{
    [RequireComponent(typeof(SpriteRenderer), typeof(Collider2D))]
    public class Draggable : MonoBehaviour
    {
        public SortingGroup SortingGroup;
        public int SortingOrderOnDrag = 1000;
        int _defaultSortingOrder;
        
        Collider2D _collider;
        SpriteRenderer _spriteRenderer;
        Vector3 _originalPosition;
        Quaternion _defaultRotation;

        float _dragAngleRad;
        float _dragAngularVelocity;
        Vector2 _smoothedMouseVelocity;
        Vector2 _prevMouseWorldPosition;
        bool _hasPrevMousePosition;

        [Header("Drag Anchor")]
        [SerializeField] Vector2 dragAnchorOffset;

        [Header("Drag Swing")]
        [Min(0f)] [SerializeField] float maximumDragAngle = 50f;
        [Min(0f)] [SerializeField] float dragAccelerationSensitivity = 1f;
        [Min(0f)] [SerializeField] float dragAirDrag = 0.8f;
        [Min(0f)] [SerializeField] float dragVerticalGravityInfluence = 0.05f;
        [Min(0f)] [SerializeField] float mouseVelocitySmoothTime = 0.05f;
        [Min(0f)] [SerializeField] float dragSpringStrength = 100f;
        [Min(0f)] [SerializeField] float dragDamping = 6f;

        public bool IsDragging { get; private set; }
        public Quaternion DefaultRotation => _defaultRotation;
        public float MaximumDragAngle => maximumDragAngle;
        public float DragAccelerationSensitivity => dragAccelerationSensitivity;
        public float DragAirDrag => dragAirDrag;
        public float MouseVelocitySmoothTime => mouseVelocitySmoothTime;
        public float DragSpringStrength => dragSpringStrength;
        public float DragDamping => dragDamping;
        MotionHandle _currentMotion;

        void Awake()
        {
            _collider = GetComponent<Collider2D>();
            _spriteRenderer = GetComponent<SpriteRenderer>();
            _defaultRotation = transform.rotation;
            _defaultSortingOrder = SortingGroup.sortingOrder;
        }

        public Vector3 GetTopCenterLocalPosition()
        {
            var spriteBounds = _spriteRenderer.sprite.bounds;
            var topCenter = new Vector2(spriteBounds.center.x, spriteBounds.max.y);
            return new Vector3(
                topCenter.x + dragAnchorOffset.x,
                topCenter.y + dragAnchorOffset.y,
                0f);
        }

        public void SetTopCenterPosition(Vector3 worldPosition)
        {
            var localAnchor = GetTopCenterLocalPosition();
            var anchorWorldOffset = transform.TransformPoint(localAnchor) - transform.position;
            transform.position = worldPosition - anchorWorldOffset;
        }

        public void BeginDrag()
        {
            _originalPosition = transform.position;
            IsDragging = true;
            SortingGroup.sortingOrder = 1000;
            ResetSwingState();

            Feedback();
        }
        void Feedback()
        {
            _currentMotion.TryComplete();

            _currentMotion = LMotion.Punch
                .Create(transform.localScale, Vector3.one * 0.5f, 0.2f)
                .WithDampingRatio(2)
                .WithFrequency(2)
                .WithOnComplete(() => transform.localScale = Vector3.one)
                .BindToLocalScale(transform);
        }

        public void UpdateDrag(Vector3 mouseWorldPosition, float deltaTime)
        {
            var dt = Mathf.Min(deltaTime, 0.05f);
            if (dt <= 0.0001f)
            {
                SetTopCenterPosition(mouseWorldPosition);
                return;
            }

            if (!_hasPrevMousePosition)
            {
                _prevMouseWorldPosition = mouseWorldPosition;
                _hasPrevMousePosition = true;
                SetTopCenterPosition(mouseWorldPosition);
                return;
            }

            var currentMousePos = (Vector2)mouseWorldPosition;
            var rawVelocity = (currentMousePos - _prevMouseWorldPosition) / dt;
            var prevSmoothedVelocity = _smoothedMouseVelocity;

            var smoothAlpha = mouseVelocitySmoothTime > 0f
                ? 1f - Mathf.Exp(-dt / mouseVelocitySmoothTime)
                : 1f;
            _smoothedMouseVelocity = Vector2.Lerp(_smoothedMouseVelocity, rawVelocity, smoothAlpha);

            var deltaVelocity = _smoothedMouseVelocity - prevSmoothedVelocity;
            var effectiveLength = GetEffectivePendulumLength();
            var cosAngle = Mathf.Cos(_dragAngleRad);

            // Pivot horizontal acceleration impulse (deltaV = a_x * dt, so dt cancels cleanly)
            _dragAngularVelocity -= (deltaVelocity.x / effectiveLength) * dragAccelerationSensitivity * cosAngle;

            // Vertical pivot acceleration modulates effective gravity
            var verticalAccel = deltaVelocity.y / dt;
            var gravityMultiplier = Mathf.Clamp(
                1f + (verticalAccel / effectiveLength) * dragVerticalGravityInfluence,
                0.35f,
                2.5f);

            // Subtle velocity-based air drag so steady dragging trails slightly
            var airDragAccel = -(_smoothedMouseVelocity.x / effectiveLength) * dragAirDrag * cosAngle;
            var springAccel = -(dragSpringStrength * gravityMultiplier) * Mathf.Sin(_dragAngleRad);
            var dampingAccel = -dragDamping * _dragAngularVelocity;

            // Semi-implicit Euler integration
            _dragAngularVelocity += (springAccel + dampingAccel + airDragAccel) * dt;
            _dragAngleRad += _dragAngularVelocity * dt;

            var maxAngleRad = maximumDragAngle * Mathf.Deg2Rad;
            if (_dragAngleRad > maxAngleRad)
            {
                _dragAngleRad = maxAngleRad;
                if (_dragAngularVelocity > 0f)
                    _dragAngularVelocity = 0f;
            }
            else if (_dragAngleRad < -maxAngleRad)
            {
                _dragAngleRad = -maxAngleRad;
                if (_dragAngularVelocity < 0f)
                    _dragAngularVelocity = 0f;
            }

            transform.rotation = _defaultRotation * Quaternion.Euler(0f, 0f, _dragAngleRad * Mathf.Rad2Deg);
            SetTopCenterPosition(mouseWorldPosition);
            _prevMouseWorldPosition = currentMousePos;
        }

        public void AddAngularImpulse(float impulseDegreesPerSecond)
        {
            _dragAngularVelocity += impulseDegreesPerSecond * Mathf.Deg2Rad;
        }

        public void ResetRotation()
        {
            ResetSwingState();
            transform.rotation = _defaultRotation;
        }

        public void EndDrag(Droppable dropTarget)
        {
            IsDragging = false;
            ResetSwingState();

            if (dropTarget == null)
            {
                transform.position = _originalPosition;
            }
            else
            {
                dropTarget.AcceptDrop(this);
            }

            SortingGroup.sortingOrder = _defaultSortingOrder;

            Feedback();
        }

        void ResetSwingState()
        {
            _dragAngleRad = 0f;
            _dragAngularVelocity = 0f;
            _smoothedMouseVelocity = Vector2.zero;
            _hasPrevMousePosition = false;
        }

        float GetEffectivePendulumLength()
        {
            if (_spriteRenderer == null || _spriteRenderer.sprite == null)
                return 1f;

            var spriteBounds = _spriteRenderer.sprite.bounds;
            var localAnchor = GetTopCenterLocalPosition();
            var localOffset = localAnchor - spriteBounds.center;
            var worldOffset = transform.TransformVector(localOffset);
            return Mathf.Max(worldOffset.magnitude, 0.25f);
        }
    }
}
