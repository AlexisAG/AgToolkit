using AgToolkit.Core.GameMode;
using AgToolkit.Core.Manager;
using UnityEngine;

namespace AgToolkit.Core.Misc
{
	public class ExitGameStateMachineBehaviour : GameStateMachineBehaviour<ExitData>
	{
		public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			GameManager.Instance.ChangeGameMode(Data.NextGameMode);
		}
	}
}
