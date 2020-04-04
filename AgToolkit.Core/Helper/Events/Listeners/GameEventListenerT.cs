using UnityEngine;
using UnityEngine.Events;

namespace AgToolkit.Core.Helper.Events.Listeners
{
	public class GameEventListener<T, GE, UE> : MonoBehaviour, IGameEventListener<T>
		where GE : IGameEvent<T>
		where UE : UnityEvent<T>
	{
		[SerializeField]
		private GE _event = default;

		public IGameEvent<T> Event => _event;

		[SerializeField]
		public UE Callbacks;
		//keep public otherwise unity editor does not isplay dynamic calls...


		public void OnEventRaised(IGameEvent<T> gameEvent)
		{
			Callbacks.Invoke(gameEvent.Param);
		}

		private void OnEnable()
		{
			if (Event == null)
			{
				Debug.LogWarning($"[GameEventListener] no GameEvent defined.");
			}
			else
			{
				Event.RegisterListener(this);
			}
		}

		private void OnDisable()
		{
			Event?.UnregisterListener(this);
		}
	}


}
