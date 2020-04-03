using UnityEngine;

namespace AgToolkit.AgToolkit.Core.GameModes.GameStates
{
	public class ExitGameStateMachineBehaviour : GameStateMachineBehaviour<ExitData>
	{
		public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			GameManager.Instance.ChangeGameMode(Data.NextGameMode);
		}
	}
}
