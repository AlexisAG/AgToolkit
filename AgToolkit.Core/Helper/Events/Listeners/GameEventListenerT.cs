using UnityEngine;
using UnityEngine.Events;

namespace AgToolkit.Core.Helper.Events.Listeners
{
	public class GameEventListener<T, GE, UE> : MonoBehaviour, IGameEventListener<T>
		where GE : IGameEvent<T>
		where UE : UnityEvent<T>
	{
		[SerializeField]
		private GE _Event = default;

        private UE _Callbacks;

		public IGameEvent<T> Event => _Event;
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

        /// <summary>
        /// Call the UnityEvent if the GameEvent raise is the GameEvent listened. 
        /// </summary>
        public void OnEventRaised(IGameEvent<T> gameEvent)
		{
            if (gameEvent != Event) return;
            _Callbacks.Invoke(gameEvent.Param);
		}

        /// <summary>
        /// Set the GameEvent & the UnityEvent callback
        /// </summary>
        public void Init(GE e, UE callback) {
            _Event = e;
            _Callbacks = callback;
        }
    }
}
