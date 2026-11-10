namespace Oneiric.Core.EventBus {
	public interface IEvent { }

	public struct TestEvent : IEvent { }

	public struct PlayerEvent : IEvent {
		public string Name;
		public PlayerEvent(string name) {
			Name = name;
		}
	}
}