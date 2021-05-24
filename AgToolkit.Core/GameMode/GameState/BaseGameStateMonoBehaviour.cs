using System.Collections;
using UnityEngine;

namespace AgToolkit.Core.GameMode
{
	/// <summary>
	/// prefer using GameStateMonoBehaviour<T> which includes Data
	/// </summary>
	[RequireComponent(typeof(Animator)), RequireComponent(typeof(GameMode))]
	public abstract class BaseGameStateMonoBehaviour : MonoBehaviour
	{
		public virtual IEnumerator OnLoad()
		{
			yield break;
		}

		public virtual IEnumerator OnUnload()
		{
			yield break;
		}
	}
}