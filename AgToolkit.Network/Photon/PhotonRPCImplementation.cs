using System;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

namespace AgToolkit.Network
{
	[RequireComponent(typeof(PhotonView))]
	public class PhotonRPCImplementation : MonoBehaviour, INetworkRemoteActionImplementation
	{
		private readonly Dictionary<string, Action<object[]>> _Actions = new Dictionary<string, Action<object[]>>();
		private PhotonView _PhotonView = null;
		private static int _ViewIds = 10; // 0 - 10 are allowed to Users GameObject

		private void Awake()
		{
			_PhotonView = PhotonView.Get(this);
			_PhotonView.ViewID = ++_ViewIds;
		}

		public void AddRemoteAction(Dictionary<string, Action<object[]>> actions)
		{
			foreach (string key in actions.Keys)
			{
				if (!_Actions.ContainsKey(key))
				{
					_Actions.Add(key, actions[key]); // Add action
				}
				else
				{
					_Actions[key] = actions[key]; // Update actions
				}
			}
		}

		public void InvokeActionLocal(string id, params object[] parameters)
		{
			_PhotonView.RPC(nameof(RpcMethod), RpcTarget.All, id, parameters);
		}

		public void InvokeActionViaServer(string id, params object[] parameters)
		{
			_PhotonView.RPC(nameof(RpcMethod), RpcTarget.AllViaServer, id, parameters);
		}

		[PunRPC]
		public void RpcMethod(string id, params object[] parameters)
		{
			Debug.Assert(_Actions.ContainsKey(id), $"There is no action with '{id}' identifier.");
			_Actions[id](parameters);
		}
	}
}
