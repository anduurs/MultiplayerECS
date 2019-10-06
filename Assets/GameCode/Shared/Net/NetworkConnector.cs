using FNZ.Shared.Model.Entity;
using Lidgren.Network;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FNZ.Shared.Net
{
	public interface INetworkListener
	{
		void OnPacketReceived(NetworkConnector net, NetIncomingMessage incMsg);
	}

	public class NetworkConnector
	{
		private NetEntityList m_NetEntities;
		private Dictionary<PacketType, INetworkListener> m_PacketReceivedMap;
	
		public NetworkConnector()
		{
			m_NetEntities = new NetEntityList(1000);
			m_PacketReceivedMap = new Dictionary<PacketType, INetworkListener>();
		}

		public void Register(PacketType packetType, INetworkListener listener)
		{
			if (m_PacketReceivedMap.ContainsKey(packetType))
			{
				Debug.LogError("Listener for packet type: " + packetType.ToString() + " has already been added");
				return;
			}

			m_PacketReceivedMap.Add(packetType, listener);
		}

		public void Dispatch(PacketType packetType, NetIncomingMessage incMsg)
		{
			if (m_PacketReceivedMap.ContainsKey(packetType))
			{
				m_PacketReceivedMap[packetType].OnPacketReceived(this, incMsg);
			}
		}

		public FNEEntity GetEntity(int netId)
		{
			return m_NetEntities.GetEntity(netId);
		}

		public void SyncEntity(FNEEntity entity, int netID)
		{
			m_NetEntities.Add(entity, netID);
		}

		public void UnsyncEntity(FNEEntity entity)
		{
			m_NetEntities.Remove(entity.entityNetId);
		}

		public int GetNumberOfSyncedEntities()
		{
			return m_NetEntities.GetNumOfSyncedEntities();
		}
	}
}

