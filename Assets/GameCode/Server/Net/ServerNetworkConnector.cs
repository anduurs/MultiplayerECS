using FNZ.Shared.Model.Entity;
using FNZ.Shared.Net;
using Lidgren.Network;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace FNZ.Server.Net
{
	public class ServerNetworkConnector 
	{
		private NetEntityList m_NetEntities;
		private Dictionary<NetMessageType, Action<ServerNetworkConnector, NetIncomingMessage>> m_PacketListenFuncTable;
		private Dictionary<FNEEntity, NetConnection> m_ConnectedClients;

		public ServerNetworkConnector()
		{
			m_NetEntities = new NetEntityList(1000);
			m_PacketListenFuncTable = new Dictionary<NetMessageType, Action<ServerNetworkConnector, NetIncomingMessage>>();
			m_ConnectedClients = new Dictionary<FNEEntity, NetConnection>();
		}

		public void Register(NetMessageType packetType, Action<ServerNetworkConnector, NetIncomingMessage> listenerFunc)
		{
			if (m_PacketListenFuncTable.ContainsKey(packetType))
			{
				Debug.LogError("Listener function for packet type: " + packetType.ToString() + " has already been added");
				return;
			}

			m_PacketListenFuncTable.Add(packetType, listenerFunc);
		}

		public void Dispatch(NetMessageType packetType, NetIncomingMessage incMsg)
		{
			if (m_PacketListenFuncTable.ContainsKey(packetType))
			{
				m_PacketListenFuncTable[packetType].Invoke(this, incMsg);
			}
		}

		public void AddConnectedClient(FNEEntity clientEntity, NetConnection clientConnection)
		{
			m_ConnectedClients.Add(clientEntity, clientConnection);
		}

		public FNEEntity GetPlayerFromConnection(NetConnection conn)
		{
			foreach (var player in m_ConnectedClients.Keys)
			{
				if (m_ConnectedClients[player] == conn)
				{
					return player;
				}
			}

			return null;
		}

		public NetConnection GetConnectionFromPlayer(FNEEntity player)
		{
			if (m_ConnectedClients.ContainsKey(player))
				return m_ConnectedClients[player];

			return null;
		}

		public void SyncEntity(FNEEntity entity)
		{
			m_NetEntities.Add(entity);
		}

		public void UnsyncEntity(FNEEntity entity)
		{
			m_NetEntities.Remove(entity.NetId);
		}

		public FNEEntity GetEntity(int netId)
		{
			return m_NetEntities.GetEntity(netId);
		}

		public int GetNumberOfSyncedEntities()
		{
			return m_NetEntities.GetNumOfSyncedEntities();
		}
	}
}

