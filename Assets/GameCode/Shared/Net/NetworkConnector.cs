using FNZ.Shared.Model.Entity;
using Lidgren.Network;
using System;
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

		private Dictionary<PacketType, INetworkListener> m_PacketListenerTable;
		private Dictionary<PacketType, Action<NetworkConnector, NetIncomingMessage>> m_PacketListenFuncTable;

		public NetworkConnector()
		{
			m_NetEntities		    = new NetEntityList(1000);
			m_PacketListenerTable   = new Dictionary<PacketType, INetworkListener>();
			m_PacketListenFuncTable = new Dictionary<PacketType, Action<NetworkConnector, NetIncomingMessage>>();
		}

		public void Register(PacketType packetType, INetworkListener listener)
		{
			if (m_PacketListenerTable.ContainsKey(packetType))
			{
				Debug.LogError("Listener for packet type: " + packetType.ToString() + " has already been added");
				return;
			}

			m_PacketListenerTable.Add(packetType, listener);
		}

		public void Register(PacketType packetType, Action<NetworkConnector, NetIncomingMessage> listenerFunc)
		{
			if (m_PacketListenFuncTable.ContainsKey(packetType))
			{
				Debug.LogError("Listener function for packet type: " + packetType.ToString() + " has already been added");
				return;
			}

			m_PacketListenFuncTable.Add(packetType, listenerFunc);
		}

		public void Dispatch(PacketType packetType, NetIncomingMessage incMsg)
		{
			if (m_PacketListenerTable.ContainsKey(packetType))
			{
				m_PacketListenerTable[packetType].OnPacketReceived(this, incMsg);
			}

			if (m_PacketListenFuncTable.ContainsKey(packetType))
			{
				m_PacketListenFuncTable[packetType].Invoke(this, incMsg);
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

