using System.Collections.Generic;
using UnityEngine;

namespace AgToolkit.AgToolkit.Network.Photon
{
	[RequireComponent(typeof(PhotonView), typeof(PhotonTransformView))]
	public class PhotonSyncPosition : MonoBehaviour
	{
		private PhotonView _PhotonView = null;
		private PhotonTransformView _PhotonTransformView = null;

		private void Awake()
		{
			_PhotonView = GetComponent<PhotonView>();
			_PhotonTransformView = GetComponent<PhotonTransformView>();

			// TransformView Sync settings
			_PhotonTransformView.m_SynchronizePosition = true;
			_PhotonTransformView.m_SynchronizeRotation = true;
			_PhotonTransformView.m_SynchronizeScale = false;

			// PhotonView Observable & settings
			_PhotonView.ObservedComponents = new List<Component> // Init the observed list
			{
				GetComponent(typeof(PhotonTransformView))
			};
			_PhotonView.Synchronization = ViewSynchronization.UnreliableOnChange;
		}
		public void SetPhotonViewInfo(int id, bool isOwner)
		{
			_PhotonView.ViewID = id;

			if (isOwner)
			{
				_PhotonView.TransferOwnership(id);
			}
		}

		public void SetActive(bool b)
		{
			_PhotonView.enabled = b;
			_PhotonTransformView.enabled = b;
		}
	}
}
