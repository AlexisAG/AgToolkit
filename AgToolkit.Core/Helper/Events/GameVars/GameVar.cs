using System;
using AgToolkit.Core.Helper.Events;
using UnityEngine;

namespace AgToolkit.Core.Helper.GameVars
{
	public abstract class GameVar
	{
		public abstract void FillGameEvent(GameEvent evt);
	}

    [Serializable]
	public class GameVar<T> : GameVar
	{
		public T Value;

        /// <summary>
        /// Fill the game event with this GameVar
        /// </summary>
        /// <param name="evt">GameEvent to fill</param>
		public override void FillGameEvent(GameEvent evt)
		{
			GameEvent<T> tEvt = evt as GameEvent<T>;

			Debug.Assert(tEvt != null, "GameEvent type is not compatible with this GameVar");

			tEvt.Param = Value;
		}

	}
}