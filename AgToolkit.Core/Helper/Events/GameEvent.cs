using System;
using System.Collections.Generic;
using AgToolkit.Core.Helper.Events.Listeners;
using UnityEngine;

namespace AgToolkit.Core.Helper.Events
{
	public class GameEvent : ScriptableObject, IGameEvent
	{
		[NonSerialized]
		internal List<IGameEventListener> Listeners = new List<IGameEventListener>();

		public virtual void Raise()
		{
			for (int i = 0; i < Listeners.Count; ++i)
			{
				Listeners[i].OnEventRaised(this);
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
