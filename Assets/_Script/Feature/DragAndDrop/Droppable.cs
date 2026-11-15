using System;
using Oneiric.Core.EventBus;
using UnityEngine;

namespace Gimersia.Feature.DragAndDrop
{
    [RequireComponent(typeof(Collider2D))]
    public class Droppable : MonoBehaviour
    {
        public void AcceptDrop(Draggable draggable)
        {
            draggable.transform.position = transform.position;
            EventBus.Raise(new DropEvent(draggable, this));
        }
    }
}
