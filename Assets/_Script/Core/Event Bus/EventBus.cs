using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Pool;

namespace Oneiric.Core.EventBus {
    /// <summary>
    /// Static class that provides cleaner API for calling EventBus methods without specifying the generic type when raising events.
    /// It serves as a facade to the generic EventBus<T> class
    /// </summary>
    public static class EventBus {
        public static void Raise<T>(T @event) where T : IEvent {
            EventBus<T>.Raise(@event);
        }
        public static void Register<T>(EventBinding<T> binding) where T : IEvent => EventBus<T>.Register(binding);
        public static void Unregister<T>(EventBinding<T> binding) where T : IEvent => EventBus<T>.Unregister(binding);
        public static EventBinding<T> Bind<T>(Action<T> onEvent) where T : IEvent => EventBus<T>.Bind(onEvent);
    }
	/// <summary>
	/// Static event dispatcher for a single event type.
	/// </summary>
	#nullable enable
	public static class EventBus<T> where T : IEvent {
	    static UniTaskCompletionSource<T>? tcs;
		static readonly List<IEventBinding<T>> bindings = new ();

		/// <summary>
		/// Registers a binding so it receives raised events.
		/// </summary>
		/// <param name="binding">Binding to add.</param>
		public static void Register(EventBinding<T> binding) => bindings.Add(binding);

		/// <summary>
		/// Removes a binding from this event bus.
		/// </summary>
		/// <param name="binding">Binding to remove.</param>
		public static void Unregister(EventBinding<T> binding) => bindings.Remove(binding);
		public static EventBinding<T> Bind(Action<T> onEvent) {
			var binding = new EventBinding<T>(onEvent);
			Register(binding);
			return binding;
		}
        public static EventBinding<T> Bind(Action onEventNoArgs) {
            var binding = new EventBinding<T>(onEventNoArgs);
            Register(binding);
            return binding;
        }

        public static UniTask<T> Wait() {
            if (tcs == null) {
                tcs = new UniTaskCompletionSource<T>();
            }
            return tcs.Task;
        }

		/// <summary>
		/// Raises an event to every registered binding.
		/// </summary>
		/// <param name="@event">Event payload to dispatch.</param>
		public static void Raise(T @event) {
			for (int i = bindings.Count - 1; i >= 0; i--) { // eliminate collection modification issues
				var binding = bindings[i];
				binding.OnEvent(@event);
				binding.OnEventNoArgs();
			}
            
            if (tcs != null) {
                var currentTcs = tcs; // avoid race condition when multiple events are raised simultaneously
                tcs = null;
                currentTcs.TrySetResult(@event);
            }
		}
		static void Clear() {
			bindings.Clear();
		}
	}
}