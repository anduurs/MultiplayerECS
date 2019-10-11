using FNZ.Shared.Model.Entity;
using FNZ.Shared.Net;
using Lidgren.Network;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace FNZ.Client.Net
{
	public class ClientNetworkConnector
	{
		private NetEntityList m_NetEntities;
		private Dictionary<PacketType, Action<ClientNetworkConnector, NetIncomingMessage>> m_PacketListenFuncTable;

		public ClientNetworkConnector()
		{
			m_NetEntities = new NetEntityList(1000);
			m_PacketListenFuncTable = new Dictionary<PacketType, Action<ClientNetworkConnector, NetIncomingMessage>>();
		}

		public void Register(PacketType packetType, Action<ClientNetworkConnector, NetIncomingMessage> listenerFunc)
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
