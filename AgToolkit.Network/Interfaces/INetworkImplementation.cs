namespace AgToolkit.AgToolkit.Network.Interfaces
{
	public interface INetworkImplementation : INetworkMatchmaking, INetworkRemoteAction
	{
		void Init();
		void Connect();
		void Disconnect();
		bool IsConnected();
	}
}
