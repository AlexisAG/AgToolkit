using System;
using System.Collections.Generic;
using UnityEngine;

namespace AgToolkit.AgToolkit.Network.Interfaces
{
	public interface INetworkRemoteAction
	{
		void SetupRemoteAction(GameObject obj, Dictionary<string, Action<object[]>> actions);
		void ExecuteRemoteAction(GameObject obj, string id, params object[] parameters);
		void ExecuteRemoteAction(GameObject obj, string id, bool executeOnServerForAll, params object[] parameters);
	}
}
