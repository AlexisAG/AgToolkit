using System;
using System.Collections.Generic;
using UnityEngine;

namespace AgToolkit.Core.Event
{
	public abstract class GameEvent<T> : GameEvent, IGameEvent<T>
    {
        public T Param { get; set; } = default;

		[NonSerialized]
		internal List<IGameEventListener<T>> ParamListeners = new List<IGameEventListener<T>>();

		public override void Raise()
		{
            Debug.Assert(!EqualityComparer<T>.Default.Equals(Param, default));
			foreach (var listener in ParamListeners)
            {
                listener.OnEventRaised(this);
            }
			Param = default;//reset, to check at next call if Param is still set correctly(can have 1 game event with multiple different calls)
        }

		public void RegisterListener(IGameEventListener<T> listener)
		{
			ParamListeners.Add(listener);
		}
		public void UnregisterListener(IGameEventListener<T> listener)
		{
			ParamListeners.Remove(listener);
		}
	}
}
