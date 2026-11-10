using System;

namespace Oneiric.Core.EventBus {

	internal interface IEventBinding<T> {
		public Action<T> OnEvent { get; set; }
		public Action OnEventNoArgs { get; set; }
	}
	public class EventBinding<T> : IDisposable, IEventBinding<T> where T : IEvent {
		public Action<T> onEvent = _ => { };
		public Action onEventNoArgs = () => { };

		Action<T> IEventBinding<T>.OnEvent {
			get => onEvent;
			set => onEvent = value;
		}
		Action IEventBinding<T>.OnEventNoArgs {
			get => onEventNoArgs;
			set => onEventNoArgs = value;
		}
		public EventBinding(Action<T> onEvent) {
			this.onEvent = onEvent;
		}
		public EventBinding(Action onEventNoArgs) {
			this.onEventNoArgs = onEventNoArgs;
		}

		public EventBinding<T> Bind(Action onEvent) {
			onEventNoArgs += onEvent;
			return this;
		}
		public EventBinding<T> Bind(Action<T> onEvent) {
			this.onEvent += onEvent;
			return this;
		}
		public void Remove(Action onEvent) => onEventNoArgs -= onEvent;
		public void Remove(Action<T> onEvent) => this.onEvent -= onEvent;

		public void Dispose() {
			EventBus<T>.Unregister(this);
		}
	}
}
