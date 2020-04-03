using System.Collections.Generic;
using UnityEngine;

namespace AgToolkit.AgToolkit.Network.Interfaces
{
	public interface INetworkMatchmaking
	{
		void JoinMainLobby();
		void CreateRoom(string name);
		void JoinRoom(string name);
		void CloseRoom(bool isOpen);
		void LeftRoom();
		void KickUser(int networkId);
		void SyncUserPosition(GameObject user, bool active);
		bool IsInRoom();
		void ClearRoom();
		List<string> GetRoomNames();
		RoomData GetRoomData(string roomName);
	}
}
