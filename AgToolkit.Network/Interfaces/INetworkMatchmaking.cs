using System.Collections.Generic;
using UnityEngine;

namespace AgToolkit.Network
{
	public interface INetworkMatchmaking
	{
		void JoinMainLobby();
		void CreateRoom(string name);
		void JoinRoom(string name);
		void CloseRoom(bool isOpen);
		void LeftRoom();
		void KickUser(int networkId);
		void InitSyncUserPosition(GameObject user, bool active);
		bool IsInRoom();
		void ClearRoom();
		List<string> GetRoomNames();
		RoomData GetRoomData(string roomName);
	}
}
