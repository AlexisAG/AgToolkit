using UnityEngine;

namespace AgToolkit.Core.Event
{

	/// <summary>
	/// simple monobehaviour service to trigger a gameevent with optional param
	/// </summary>
	public class GameEventTrigger : MonoBehaviour
	{
		[SerializeField]
		private GameEvent _Event = null;

		[SerializeField]
		private GameVar _Param = null;

        public void Init(GameEvent ge, GameVar param)
        {
            _Event = ge;
            _Param = param;
        }

        /// <summary>
        /// Fill the GameEvent with the GameVar and trigger the GameEvent.
        /// </summary>
		public void Trigger()
		{
			Debug.Assert(_Event != null, "No GameEvent To Trigger");

			if (_Param != null)
			{
				_Param.FillGameEvent(_Event);
			}
            _Event.Raise();
		}

	}
}
