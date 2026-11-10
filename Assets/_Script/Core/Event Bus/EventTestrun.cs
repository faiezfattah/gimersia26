using System;
using Oneiric.Core.EventBus;
using UnityEngine;

public class EventTestrun : MonoBehaviour {
	IDisposable[] eventBinding;
	void OnEnable() {
        eventBinding = new IDisposable[] {
            EventBus<TestEvent>
                .Bind(HandleTestEvent)
                .Add(HandleTestEventNoArgs),
            EventBus<PlayerEvent>
                .Bind(HandlePlayerEvent)
                .Add(HandlePlayerEventNoArgs)
        };

	}
	void OnDisable() {
		foreach (var binding in eventBinding) binding.Dispose();
	}
	void Update() {
		if (Input.GetKeyDown(KeyCode.T)) {
			EventBus.Raise(new TestEvent());
		}
		if (Input.GetKeyDown(KeyCode.P)) {
			EventBus.Raise(new PlayerEvent("Alice"));
		}
	}
	void HandleTestEvent(TestEvent e) {
		Debug.Log("Test event received");
	}
	void HandleTestEventNoArgs() {
		Debug.Log("Test event received (no args)");
	}
	void HandlePlayerEvent(PlayerEvent e) {
		Debug.Log($"Player event received: {e.Name}");
	}
	void HandlePlayerEventNoArgs() {
		Debug.Log("Player event received (no args)");
	}

}