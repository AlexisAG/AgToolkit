using System.Collections;
using AgToolkit.AgToolkit.Core.GameModes.GameStates;
using UnityEngine;

namespace AgToolkit.AgToolkit.Core.GameModes
{
	public abstract class GameMode : MonoBehaviour
	{
        [SerializeField]
		private EnumGameMode _id = null;

		public EnumGameMode Id => _id;

		public virtual void Awake()
		{
			Debug.Assert(Id != null, "no EnumGameMode asset assigned to GameMode.");
			GameManager.Instance.SetGameMode(this);
		}

		/// <summary>
		/// do async loading during loading screen while entering gamemode
		/// </summary>
		/// <returns></returns>
		public virtual IEnumerator OnLoad()
		{
            //load service for gamestates
			BaseGameStateMonoBehaviour[] gameStates = GetComponents<BaseGameStateMonoBehaviour>();
			foreach (BaseGameStateMonoBehaviour gameState in gameStates)
			{
				yield return gameState.OnLoad();
			}
		}

		/// <summary>
		/// do async unload during loading screen while exiting gamemode
		/// </summary>
		public virtual IEnumerator OnUnload()
		{
			//unload for gamestates
			BaseGameStateMonoBehaviour[] gameStates = GetComponents<BaseGameStateMonoBehaviour>();
			foreach (BaseGameStateMonoBehaviour gameState in gameStates)
			{
				yield return gameState.OnUnload();
			}

		}
	}
}
