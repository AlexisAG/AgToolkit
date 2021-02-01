using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;

namespace AgToolkit.Network
{
	public class NetworkDiscovery
	{
		private int _BroadcastPort = 5056;
		private int _UDPTimeout = 1000;
		private string _UDPDataWanted = "IAmPhotonServer";

		private UdpClient _Client = null;
		private Thread _DataReceiverThread = null;

		private ConnectionConfig _Config;

		~NetworkDiscovery() => StopThread();

		private void ReceiveUdpData()
		{
			IPEndPoint remoteEp = new IPEndPoint(IPAddress.Any, 0);
			string[] serverResponseString = new string[2];
			byte[] broadcastData = Encoding.ASCII.GetBytes("LookForPhotonServer");

			_Client.Send(broadcastData, broadcastData.Length, new IPEndPoint(IPAddress.Broadcast, _BroadcastPort));

			while (serverResponseString[0] != _UDPDataWanted)
			{
				try
				{
					byte[] serverResponseData = _Client.Receive(ref remoteEp);
					serverResponseString = Encoding.ASCII.GetString(serverResponseData).Split('|');
					Debug.Log($"Received PORT: {serverResponseString[1]} from {remoteEp.Address.ToString()}");
				}
				catch (Exception e)
				{
					Debug.LogError($"ReceiveUDPData error -> {e}");
					_Client?.Send(broadcastData, broadcastData.Length, new IPEndPoint(IPAddress.Parse("192.168.255.255")/*Broadcast*/, _BroadcastPort));
				}
			}

			_Config = new ConnectionConfig(remoteEp.Address.ToString(), int.Parse(serverResponseString[1]));
			StopThread(true);
		}

		public void StartNetworkDiscovery()
		{
			_Client = new UdpClient { EnableBroadcast = true, Client = { ReceiveTimeout = _UDPTimeout } };

			// Start thread
			_DataReceiverThread = new Thread(ReceiveUdpData)
			{
				IsBackground = true
			};
			_DataReceiverThread.Start();
		}

		public void StopThread(bool serverFound = false)
		{
			if (serverFound)
			{
				NetworkManager.Instance.OnServerFound(_Config);
			}

			_Client?.Close();
			_DataReceiverThread?.Abort();
			_DataReceiverThread = null;
			_Client = null;
		}
	}
}
