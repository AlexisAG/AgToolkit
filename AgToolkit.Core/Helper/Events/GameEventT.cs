using System;
using System.Collections.Generic;
using AgToolkit.AgToolkit.Core.Helper.Events.Listeners;
using UnityEngine;

namespace AgToolkit.AgToolkit.Core.Helper.Events
{
	public abstract class GameEvent<T> : GameEvent, IGameEvent<T>
	{
		[NonSerialized]
		private T _param = default;

		public T Param
		{
			get => _param;
			set => _param = value;
		}

		[NonSerialized]
		internal List<IGameEventListener<T>> ParamListeners = new List<IGameEventListener<T>>();

		public override void Raise()
		{
			Debug.Assert(Param != default);
			for (int i = 0; i < ParamListeners.Count; ++i)
			{
				ParamListeners[i].OnEventRaised(this);
			}
			Param = default;//reset, to check at next call if Param is still set correctly(can have 1 game event with multiple different calls)

			Debug.Assert(Listeners.Count == 0);//should not have register listeners through the wrong list, or is it wanted ? should we call base.Raise() ?
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
