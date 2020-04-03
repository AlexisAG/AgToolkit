using UnityEngine;
using UnityEngine.Events;

namespace AgToolkit.AgToolkit.Core.Helper.Events.Listeners
{
	public class GameEventListener : MonoBehaviour, IGameEventListener
	{
		[SerializeField]
		private GameEvent _event = null;

		public IGameEvent Event => _event;

		[SerializeField]
		public UnityEvent Callbacks;
		//keep public otherwise unity editor does not isplay dynamic calls...


		public void OnEventRaised(IGameEvent gameEvent)
		{
			Callbacks.Invoke();
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
