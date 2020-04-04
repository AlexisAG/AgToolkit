using AgToolkit.AgToolkit.Core.Helper.Events;
using UnityEngine;

namespace FramaToolkit
{
	public abstract class GameVar : ScriptableObject
	{
		public abstract void FillGameEvent(GameEvent evt);
	}

	public class GameVar<T> : GameVar
	{
		public T Value;

		public override void FillGameEvent(GameEvent evt)
		{
			GameEvent<T> tEvt = evt as GameEvent<T>;

			Debug.Assert(tEvt != null, "GameEvent type is not compatible with this GameVar");

			tEvt.Param = Value;
		}

	}
}