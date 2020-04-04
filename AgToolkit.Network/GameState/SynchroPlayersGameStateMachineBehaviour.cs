using AgToolkit.AgToolkit.Core.GameModes.GameStates;
using AgToolkit.AgToolkit.Core.Helper;
using FramaToolkit;
using UnityEngine;

namespace AgToolkit.AgToolkit.Network.GameState
{
	public class BaseSynchroPlayersGameStateMachineBehaviour<T> : GameStateMachineBehaviour<T> where T : SynchroPlayersData
	{
		public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			if (!NetworkManager.IsInstanced && !UserManager.IsInstanced)
			{
				UserManager.Instance.LocalUser.IsReady = true;
				animator.SetTrigger(Data.TriggerName);
				return;
			}
			NetworkManager.Instance.PlayersSynchronizedEvent.RemoveAllListeners();
			NetworkManager.Instance.PlayersSynchronizedEvent.AddListener((() => animator?.SetTrigger(Data.TriggerName)));
			NetworkManager.Instance.StartSynchronize();
			UserManager.Instance.LocalUser.IsReady = true;

			if (UserManager.Instance.LocalUser.UserRole == EnumUserRole.Master)
			{
				CoroutineManager.Instance.StartCoroutine(NetworkManager.Instance.WaitOtherUsers());
			}
		}
	}

	public class SynchroPlayersGameStateMachineBehaviour : GameStateMachineBehaviour<SynchroPlayersData>
	{
	}
}
