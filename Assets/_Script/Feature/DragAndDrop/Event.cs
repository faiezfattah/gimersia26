
using Oneiric.Core.EventBus;

namespace Gimersia.Feature.DragAndDrop
{
    public readonly struct DropEvent : IEvent
    {
        public readonly Draggable Draggable;
        public readonly Droppable Droppable;

        public DropEvent(Draggable draggable, Droppable droppable)
        {
            Draggable = draggable;
            Droppable = droppable;
        }
    }
}
