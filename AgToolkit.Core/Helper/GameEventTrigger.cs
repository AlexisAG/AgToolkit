using AgToolkit.Core.Helper.Events;
using AgToolkit.Core.Helper.GameVars;
using UnityEngine;

namespace AgToolkit.Core.Helper
{

	/// <summary>
	/// simple monobehaviour service to trigger a gameevent with optional param
	/// </summary>
	public class GameEventTrigger : MonoBehaviour
	{
		[SerializeField]
		private GameEvent Event = null;

		[SerializeField]
		private GameVar _Param = null;

        /// <summary>
        /// Fill the GameEvent with the GameVar and trigger the GameEvent.
        /// </summary>
		public void Trigger()
		{
			Debug.Assert(Event != null, "No GameEvent To Trigger");

			if (_Param != null)
			{
				_Param.FillGameEvent(Event);
			}
			Event.Raise();
		}

	}
}
