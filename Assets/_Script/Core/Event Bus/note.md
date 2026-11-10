# Event Bus

This event bus is for broadcasting facts after the main work is already done.
Use it for side effects only: UI refresh, audio, VFX, logging, analytics, save triggers, and other decoupled reactions.

DO NOT use this as part of the main execution path.
PREFER explicitness through glue code.
The caller must keep control of gameplay flow, validation, and state changes.
Listeners should react to what happened, not decide whether it happens.

## Usage

Register listeners with `EventBus<T>.Bind(...)` or `EventBus.Register(...)`.
Unregister them when the owner is disabled or disposed so the bus does not keep stale bindings alive.

Raise events with `EventBus.Raise(new YourEvent(...))` once the action is complete.

Example:
```csharp
private IDisposable _binding;

void OnEnable()
{
    _binding = EventBus<TestEvent>
        .Bind(OnTestEvent)
        .Add(OnTestEventNoArgs);
}

void OnDisable()
{
    _binding.Dispose();
}

void Something()
{
    // Main logic happens here.
    EventBus.Raise(new TestEvent());
}
```

Keep event payloads small and specific.
If the code cannot function correctly without a listener, it does not belong on this bus.

For multiple subscription we can combine it into a single IDisposable with R3's Disposable.Combine
```csharp
_binding = Disposable.Combine(
	EventBus<Event1>.Bind(OnEven1),
	EventBus<Event2>.Bind(OnEvent2),
);

_binding.Dispose();
```

To raise event it is as simple as 
```csharp
EventBus.Raise(new Event());
```

## Creating Events
Create it on the Core/Events folder unless it is a domain specific events that uses domain types. In that case always put it on an Events.cs inside the module and always use namespace. 

Structure
> <Subject><PastTenseVerb><Object?>Event Inspired by Elm

Elm events are plain English facts: PlayerJumpedEvent, EnemyDiedEvent. They describe what already happened, not what should happen next. Listeners decide what to do with the fact. Always a struct, always suffix Event. The Event suffix makes greping easy and distinguishes from Unity Messages, Actions, and UnityEvents.

