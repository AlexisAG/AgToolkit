using System;
using System.Collections.Generic;

namespace AgToolkit.Network
{
	public interface INetworkRemoteActionImplementation
	{
		void AddRemoteAction(Dictionary<string, Action<object[]>> actions);
		void InvokeActionLocal(string id, params object[] parameters);
		void InvokeActionViaServer(string id, params object[] parameters);
		void RpcMethod(string id, params object[] parameters);
	}
}
