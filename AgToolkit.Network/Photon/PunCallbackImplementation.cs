using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using PhotonNetwork = Photon.Pun.PhotonNetwork;

namespace AgToolkit.Network
{
	/// This class have to be added on a gameobject (MonoBehaviour)
	public class PunCallbackImplementation : MonoBehaviourPunCallbacks
	{
		private PhotonNetworkImplementation _PhotonNetworkImplementation = null;

		public void Init(PhotonNetworkImplementation pni)
		{
			_PhotonNetworkImplementation = pni; // Need a ref to PhotonNerworkImplementation for Updating the room list
		}

		#region PUNCallback

		/// <summary>OnConnectedToMaster is a method in the PunCallbackImplementation class.</summary>
		/// <remarks>Join the main lobby and stop broadcasting.</remarks>
		public override void OnConnectedToMaster()
		{
			base.OnConnectedToMaster();
			NetworkManager.Instance.OnConnect();
		}

		/// <summary>OnCreatedRoom is a method in the PunCallbackImplementation class.</summary>
		/// <remarks>Set LocalUser info to {Role: Actor, Type: Network, NetworkID: 1}.</remarks>
		public override void OnCreatedRoom()
		{
			base.OnCreatedRoom();
			NetworkManager.Instance.OnRoomCreated();
		}

		/// <summary>OnJoinedRoom is a method in the PunCallbackImplementation class.</summary>
		/// <remarks>If the LocalUser is not the master of the room, set the LocalUser info to {Role: Spectator, Type: Network, NetworkID: int}.</remarks>
		public override void OnJoinedRoom()
		{
			base.OnJoinedRoom();

			_PhotonNetworkImplementation.AddRoom(new RoomData(PhotonNetwork.CurrentRoom.Name, PhotonNetwork.CurrentRoom.PlayerCount, PhotonNetwork.CurrentRoom.IsOpen));
			List<int> otherNetworkId = new List<int>(UserManager.Instance.MaxUsers);

			foreach (Player p in PhotonNetwork.PlayerListOthers)
			{
				otherNetworkId.Add(p.ActorNumber);
			}

			NetworkManager.Instance.OnRoomJoined(PhotonNetwork.LocalPlayer.ActorNumber, otherNetworkId, PhotonNetwork.CurrentRoom.Name);
		}

		/// <summary>OnJoinedLobby is a method in the PunCallbackImplementation class.</summary>
		/// <remarks>Set the LocalUser info to {Role: Spectator, Type: Network, NetworkID: 0}.</remarks>
		public override void OnJoinedLobby()
		{
			base.OnJoinedLobby();
			NetworkManager.Instance.OnMainLobbyJoined();
		}

		/// <summary>OnRoomListUpdate is a method in the PunCallbackImplementation class.</summary>
		/// <remarks>Called when the LocalUser has joined the main lobby or an another player create a room.</remarks>
		public override void OnRoomListUpdate(List<RoomInfo> roomList)
		{
			base.OnRoomListUpdate(roomList);
			_PhotonNetworkImplementation.UpdateRoomList(roomList);
		}

		/// <summary>OnRoomListUpdate is a method in the PunCallbackImplementation class.</summary>
		/// <remarks>Called when a user joins the same room as the LocalUser.</remarks>
		/// <remarks>Create and register an new user in the UserManager.</remarks>
		public override void OnPlayerEnteredRoom(Player newPlayer)
		{
			base.OnPlayerEnteredRoom(newPlayer);
			NetworkManager.Instance.OnUserJoin(newPlayer.ActorNumber);
		}

		// fail & disconnect & leave
		public override void OnCreateRoomFailed(short returnCode, string message)
		{
			base.OnCreateRoomFailed(returnCode, message);
			Debug.LogError($"failed to create a room -> {message}");
		}

		public override void OnJoinRoomFailed(short returnCode, string message)
		{
			base.OnJoinRoomFailed(returnCode, message);
			NetworkManager.Instance.OnJoinRoomFailed($"{message} code -> {returnCode.ToString()}");
		}

		/// <summary>OnLeftRoom is a method in the PunCallbackImplementation class.</summary>
		/// <remarks>Join the main lobby if the LocalUser is still connected to the photon server.</remarks>
		public override void OnLeftRoom()
		{
			base.OnLeftRoom();
			NetworkManager.Instance?.OnLeftRoom();
		}

		public override void OnPlayerLeftRoom(Player otherPlayer)
		{
			base.OnPlayerLeftRoom(otherPlayer);
			NetworkManager.Instance.OnUserLeft(otherPlayer.ActorNumber);
		}

		public override void OnDisconnected(DisconnectCause cause)
		{
			base.OnDisconnected(cause);
			switch (cause)
			{
			case DisconnectCause.ExceptionOnConnect:
				NetworkManager.Instance?.OnDisconnect("An exception was occured onConnect.");
				break;
			case DisconnectCause.Exception:
				NetworkManager.Instance?.OnDisconnect("An exception was occured.");
				break;
			case DisconnectCause.ServerTimeout:
				NetworkManager.Instance?.OnDisconnect("Server timeout.");
				break;
			case DisconnectCause.ClientTimeout:
				NetworkManager.Instance?.OnDisconnect("Client timeout.");
				break;
			case DisconnectCause.DisconnectByServerLogic:
				NetworkManager.Instance?.OnDisconnect("Server logic.");
				break;
			case DisconnectCause.InvalidAuthentication:
				NetworkManager.Instance?.OnDisconnect("Invalid authentication.");
				break;
			case DisconnectCause.MaxCcuReached:
				NetworkManager.Instance?.OnDisconnect("Max CCU reached.");
				break;
			case DisconnectCause.DisconnectByClientLogic:
				NetworkManager.Instance?.OnDisconnect("Disconnect by client logic.");
				break;
			default:
				NetworkManager.Instance?.OnDisconnect("Reason unknown.");
				break;
			}
		}
		#endregion

	}
}
