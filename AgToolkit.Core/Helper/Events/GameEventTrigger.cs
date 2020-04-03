using FramaToolkit;
using UnityEngine;

namespace AgToolkit.AgToolkit.Core.Helper.Events
{

	/// <summary>
	/// simple monobehaviour service to trigger a gameevent with optional param
	/// </summary>
	public class GameEventTrigger : MonoBehaviour
	{
		[SerializeField]
		private GameEvent Event = null;

		[SerializeField]
		public GameVar EventParam = null;


		public void Trigger()
		{
			Debug.Assert(Event != null, "No GameEvent To Trigger");

			if (EventParam != null)
			{
				EventParam.FillGameEvent(Event);
			}
			Event.Raise();
		}

	}
}
