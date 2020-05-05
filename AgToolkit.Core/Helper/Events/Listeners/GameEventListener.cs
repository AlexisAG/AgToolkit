using UnityEngine;
using UnityEngine.Events;

namespace AgToolkit.Core.Helper.Events.Listeners
{
	public class GameEventListener : MonoBehaviour, IGameEventListener
	{
		[SerializeField]
		private IGameEvent _Event = null;

		public IGameEvent Event => _Event;

		public UnityEvent Callbacks;

        /// <summary>
        /// Call the UnityEvent if the GameEvent raise is the GameEvent listened. 
        /// </summary>
        public void OnEventRaised(IGameEvent gameEvent)
        {
            if (gameEvent != Event) return;
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
