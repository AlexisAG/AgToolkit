using UnityEngine;
using UnityEngine.Events;

namespace AgToolkit.Core.Helper.Events.Listeners
{
	public class GameEventListener : MonoBehaviour, IGameEventListener
	{
		[SerializeField]
		private IGameEvent _Event = null;

		private UnityEvent _Callbacks;

		public IGameEvent Event => _Event;

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

        /// <summary>
        /// Call the UnityEvent if the GameEvent raise is the GameEvent listened. 
        /// </summary>
        public void OnEventRaised(IGameEvent gameEvent)
        {
            if (gameEvent != Event) return;
            _Callbacks.Invoke();
		}


        /// <summary>
        /// Set the GameEvent & the UnityEvent callback
        /// </summary>
        public void Init(IGameEvent e, UnityEvent callback)
        {
            _Event = e;
            _Callbacks = callback;
        }
    }
}
