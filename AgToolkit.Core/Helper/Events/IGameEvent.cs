using AgToolkit.Core.Helper.Events.Listeners;

namespace AgToolkit.Core.Helper.Events
{
	public interface IGameEvent
	{
		void Raise();

		void RegisterListener(IGameEventListener listener);
		void UnregisterListener(IGameEventListener listener);
	}

	public interface IGameEvent<T> : IGameEvent
	{
		T Param { get; set; }
		void RegisterListener(IGameEventListener<T> listener);
		void UnregisterListener(IGameEventListener<T> listener);
	}
}
