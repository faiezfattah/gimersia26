using UnityEngine;

namespace Gimersia.Feature.DragAndDrop
{
    [RequireComponent(typeof(SpriteRenderer), typeof(Collider2D))]
    public class Draggable : MonoBehaviour
    {
        SpriteRenderer _spriteRenderer;
        Vector3 _originalPosition;
        Quaternion _defaultRotation;

        [Header("Drag Swing")]
        [Min(0f)] [SerializeField] float maximumDragAngle = 50f;
        [Min(0f)] [SerializeField] float dragAngleSensitivity = 10f;
        [Min(0f)] [SerializeField] float mouseVelocitySmoothTime = 0.05f;
        [Min(0f)] [SerializeField] float dragSpringStrength = 100f;
        [Min(0f)] [SerializeField] float dragDamping = 6f;

        public bool IsDragging { get; private set; }
        public Quaternion DefaultRotation => _defaultRotation;
        public float MaximumDragAngle => maximumDragAngle;
        public float DragAngleSensitivity => dragAngleSensitivity;
        public float MouseVelocitySmoothTime => mouseVelocitySmoothTime;
        public float DragSpringStrength => dragSpringStrength;
        public float DragDamping => dragDamping;

        void Awake()
        {
            _spriteRenderer = GetComponent<SpriteRenderer>();
            _defaultRotation = transform.rotation;
        }

        public Vector3 GetTopCenterLocalPosition()
        {
            var bounds = _spriteRenderer.sprite.bounds;
            return new Vector3(bounds.center.x, bounds.max.y, 0f);
        }

        public void SetTopCenterPosition(Vector3 worldPosition)
        {
            transform.position = worldPosition - transform.TransformVector(GetTopCenterLocalPosition());
        }

        public void BeginDrag()
        {
            _originalPosition = transform.position;
            IsDragging = true;
        }

        public void ResetRotation()
        {
            transform.rotation = _defaultRotation;
        }

        public void EndDrag(Droppable dropTarget)
        {
            IsDragging = false;

            if (dropTarget == null)
            {
                transform.position = _originalPosition;
            }
            else
            {
                dropTarget.AcceptDrop(this);
            }
        }
    }
}
