using UnityEngine;
using UnityEngine.Events;

namespace AgToolkit.Core.Helper.Events.Listeners
{
	public class GameEventListener : MonoBehaviour, IGameEventListener
	{
		[SerializeField]
		private GameEvent _event = null;

		public IGameEvent Event => _event;

		public UnityEvent Callbacks;

        public void OnEventRaised(IGameEvent gameEvent)
		{
			Callbacks.Invoke();
		}

		private void OnEnable()
		{
			if (Event == null)
			{
				Debug.LogWarning($"[{GetType().Name}] no GameEvent defined.");
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
