using System;
using UnityEngine;

namespace Gimersia.Feature.DragAndDrop
{
    [RequireComponent(typeof(Collider2D))]
    public class Droppable : MonoBehaviour
    {
        public event Action<Draggable> OnDropped;

        [SerializeField] bool snapToDropTarget = true;
        [SerializeField] ParticleSystem dropParticles;

        public void AcceptDrop(Draggable draggable)
        {
            if (snapToDropTarget)
            {
                draggable.transform.position = transform.position;
            }

            if (dropParticles != null)
            {
                dropParticles.Play();
            }

            OnDropped?.Invoke(draggable);
        }
    }
}
