using System;
using System.Collections.Generic;
using AgToolkit.Core.Helper.Events.Listeners;
using UnityEngine;

namespace AgToolkit.Core.Helper.Events
{
    [CreateAssetMenu(menuName = "AgToolkit/GameEvents/GameEvent", fileName = "NewGameEvent")]
    public class GameEvent : ScriptableObject, IGameEvent
	{
		[NonSerialized]
		internal List<IGameEventListener> Listeners = new List<IGameEventListener>();

		public virtual void Raise()
        {
            foreach (IGameEventListener listener in Listeners)
            {
                listener.OnEventRaised(this);
            }
        }

		public void RegisterListener(IGameEventListener listener)
		{
			Listeners.Add(listener);
		}
		public void UnregisterListener(IGameEventListener listener)
		{
			Listeners.Remove(listener);
		}
	}
}
