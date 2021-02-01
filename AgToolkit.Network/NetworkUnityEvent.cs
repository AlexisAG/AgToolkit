using UnityEngine.Events;

namespace AgToolkit.Network
{
	public class NetworkUnityEvent : UnityEvent
	{
		private readonly UnityEvent<string> _EventAction = new UnityEvent<string>();
		private readonly string _Id = null;

		private readonly UnityEvent _Event = null;
		public UnityEvent Event => _Event;

		private void ExecuteAction()
		{
			_EventAction.Invoke(_Id);
		}

		public NetworkUnityEvent(UnityEvent e, UnityAction<string> action, string actionKey)
		{
			_Event = e;
			_EventAction.AddListener(action);
			_Id = actionKey;
			base.AddListener(ExecuteAction);
		}

		public new void AddListener(UnityAction call)
		{
			_Event.AddListener(call);
		}

		public new void RemoveAllListeners()
		{
			_Event.RemoveAllListeners();
		}
	}
}