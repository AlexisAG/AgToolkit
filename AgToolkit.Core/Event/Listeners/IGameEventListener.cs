namespace AgToolkit.Core.Event
{
	public interface IGameEventListener
	{
		IGameEvent Event { get; }

		void OnEventRaised(IGameEvent gameEvent);
	}



	public interface IGameEventListener<T>
	{
		IGameEvent<T> Event { get; }

		void OnEventRaised(IGameEvent<T> gameEvent);
	}
}
