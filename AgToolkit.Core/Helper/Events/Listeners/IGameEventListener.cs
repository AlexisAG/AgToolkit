namespace AgToolkit.AgToolkit.Core.Helper.Events.Listeners
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
