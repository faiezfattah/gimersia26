using System;
using UnityEngine;

namespace Gimersia.Feature.DragAndDrop
{
    [RequireComponent(typeof(Collider2D))]
    public class Droppable : MonoBehaviour
    {
        public event Action<Draggable> OnDropped;

        [SerializeField] bool snapToDropTarget = true;

        public void AcceptDrop(Draggable draggable)
        {
            if (snapToDropTarget)
            {
                draggable.transform.position = transform.position;
            }

            OnDropped?.Invoke(draggable);
        }
    }
}
