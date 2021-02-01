using UnityEngine;

namespace AgToolkit.Network
{
	public interface INetworkImplementation : INetworkMatchmaking, INetworkRemoteAction
	{
		void Init();
		void Connect();
		void Disconnect();
		void CleanNetworkComponent(GameObject go);
		bool IsConnected();
	}
}
