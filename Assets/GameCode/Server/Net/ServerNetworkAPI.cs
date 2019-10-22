using FNZ.Server.Model.World;
using FNZ.Server.Net.API;
using FNZ.Shared.Model.Entity;
using FNZ.Shared.Model.Entity.Components;
using FNZ.Shared.Net;
using Lidgren.Network;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

namespace FNZ.Server.Net
{
	public class ServerNetworkAPI
	{
        private readonly NetServer m_NetServer;

        private readonly ServerWorldMessagesAPI m_WorldAPI;
        private readonly ServerEntityMessagesAPI m_EntityAPI;

        public ServerNetworkAPI(NetServer netServer)
		{
            m_NetServer = netServer;

            m_WorldAPI  = new ServerWorldMessagesAPI(netServer);
            m_EntityAPI = new ServerEntityMessagesAPI(netServer);
        }

		/// <summary>
		/// Send method: STC (Send To Client)
		/// MessagesAPI: World
		/// MessageType: WorldSetup
		/// 
		/// Sends the total world width and height in tiles and the chunksize in tiles to the given client connection.
		/// </summary>
		/// <param name="widthInTiles"> Total world width in tiles</param>
		/// <param name="heightInTiles">Total world height in tiles</param>
		/// <param name="chunkSize">Chunk size in tiles. E.g: chunkSize = 32 means one chunk is 32x32 tiles</param>
		/// <param name="clientConnection">The connection to the client</param>
		public void STC_World_WorldSetup(int widthInTiles, int heightInTiles, byte chunkSize, NetConnection clientConnection)
		{
			ServerApp.ChunkManager.AddClientToChunkStreamingSystem(clientConnection, new PlayerChunkState());
			var message = m_WorldAPI.CreateWorldSetupMessage(widthInTiles, heightInTiles, chunkSize);
			SendToClient(message, clientConnection);
		}

		/// <summary>
		/// Send method: BAR (Broadcast To All Relevant Clients)
		/// MessagesAPI: Entity
		/// MessageType: UpdateComponents
		/// 
		/// Broadcasts an update of the given Components to all relevant players. A relevant player is
		/// a player that is on the same or neighbouring chunks as the Entity on which the components were attached
		/// </summary>
		/// <param name="parent">Parent Entity for the given component(s)</param>
		/// <param name="components">The component(s) that has been updated and needs to be replicated</param>
		public void BAR_Entity_UpdateComponents(FNEEntity parent, params FNEComponentData[] components)
		{
			var message = m_EntityAPI.CreateUpdateComponentMessage(parent, components);
			Broadcast_All_Relevant(message, parent.Position);
		}

		/// <summary>
		/// Send method Abbreviation: STC
		/// </summary>
		private void SendToClient(NetMessage message, NetConnection clientConnection)
		{
			var result = clientConnection.SendMessage(
				message.Buffer, 
				message.DeliveryMethod, 
				(int) message.Channel
			);

			HandleSendResult(result, message.Type);
		}

		/// <summary>
		/// Send method Abbreviation: BTC
		/// </summary>
		private void Broadcast_To_Clients(NetMessage message, List<NetConnection> clientConnections)
		{
			m_NetServer.SendMessage(
				message.Buffer,
				clientConnections,
				message.DeliveryMethod,
				(int) message.Channel
			);
		}

		/// <summary>
		/// Send method Abbreviation: BA
		/// </summary>
		private void Broadcast_All(NetMessage message)
        {
            m_NetServer.SendMessage(
				message.Buffer, 
                m_NetServer.Connections,
                message.DeliveryMethod,
                (int) message.Channel
            );
        }

		/// <summary>
		/// Send method Abbreviation: BO
		/// </summary>
		private void Broadcast_Other(NetMessage message, NetConnection connToExclude)
		{
			var connectionsToBroadcast = new List<NetConnection>();

			foreach (var nc in m_NetServer.Connections)
			{
				if (connToExclude == nc) continue;
				connectionsToBroadcast.Add(nc);
			}

			if (connectionsToBroadcast.Count > 0)
			{
				m_NetServer.SendMessage(
				   message.Buffer,
				   connectionsToBroadcast,
				   message.DeliveryMethod,
				   (int) message.Channel
				);
			}
		}

		/// <summary>
		/// Send method Abbreviation: BAR
		/// </summary>
		private void Broadcast_All_Relevant(NetMessage message, float2 impactPosition)
		{
			var world = ServerApp.World;
			var chunkManager = ServerApp.ChunkManager;
			var chunk = world.GetWorldChunk(impactPosition);

			if (chunk != default)
			{
				var chunksToCheck = world.GetNeighbouringChunks(chunk);
				var connections = new List<NetConnection>();

				foreach (var c in chunksToCheck)
				{
					chunkManager.GetConnectionsWithChunkLoaded(c, ref connections);
				}

				m_NetServer.SendMessage(
				   message.Buffer,
				   connections,
				   message.DeliveryMethod,
				   (int) message.Channel
				);
			}
		}

		/// <summary>
		/// Send method Abbreviation: BOR
		/// </summary>
		private void Broadcast_Other_Relevant(NetMessage message, float2 impactPosition, NetConnection connToExclude)
		{
			var world = ServerApp.World;
			var chunkManager = ServerApp.ChunkManager;
			var chunk = world.GetWorldChunk(impactPosition);

			if (chunk != default)
			{
				var chunksToCheck = world.GetNeighbouringChunks(chunk);
				var connections = new List<NetConnection>();

				foreach (var chunkToCheck in chunksToCheck)
				{
					chunkManager.GetConnectionsWithChunkLoaded(chunkToCheck, ref connections);
				}

				connections.Remove(connToExclude);

				if (connections.Count > 0)
				{
					m_NetServer.SendMessage(
					   message.Buffer,
					   connections,
					   message.DeliveryMethod,
					   (int) message.Channel
					);
				}
			}
		}

        private void HandleSendResult(NetSendResult result, NetMessageType messageType)
		{
			string messageTypeName = messageType.ToString();

			switch (result)
			{
				case NetSendResult.FailedNotConnected:
					Debug.LogError("[Server] Message failed to enqueue because there is no connection. MessageType: " + messageTypeName);
					break;
				case NetSendResult.Sent:
					Debug.Log("[Server] Message was immediately sent. MessageType: " + messageTypeName);
					break;
				case NetSendResult.Queued:
					Debug.Log("[Server] Message was queued for delivery. MessageType: " + messageTypeName);
					break;
				case NetSendResult.Dropped:
					Debug.LogWarning("[Server] Message was dropped immediately since too many message were queued. MessageType: " + messageTypeName);
					break;
			}
		}
	}
}

