using FNZ.Server.Model.World;
using FNZ.Server.Net.API;
using FNZ.Shared.Model.Entity;
using FNZ.Shared.Model.Entity.Components;
using FNZ.Shared.Net;
using Lidgren.Network;
using UnityEngine;

namespace FNZ.Server.Net
{
	public class ServerNetworkAPI
	{
        private readonly NetServer m_NetServer;

        private readonly WorldNetAPI m_WorldNetAPI;
        private readonly EntityNetAPI m_EntityNetAPI;

        public ServerNetworkAPI(NetServer netPeer)
		{
            m_NetServer = netPeer;

            m_WorldNetAPI = new WorldNetAPI(netPeer);
            m_EntityNetAPI = new EntityNetAPI(netPeer);
        }

		public void SendToClient_WorldSetup(int widthInTiles, int heightInTiles, byte chunkSize, NetConnection clientConnection)
		{
			ServerApp.ChunkManager.AddClientToChunkStreamingSystem(clientConnection, new PlayerChunkState());
			var result = m_WorldNetAPI.SendWorldSetupDataToClient(widthInTiles, heightInTiles, chunkSize, clientConnection);
			HandleSendResult(result, "SendToClient_WorldSetup");
		}

        public void Broadcast_All(NetOutgoingMessage messageBuffer, NetDeliveryMethod deliveryMethod, SequenceChannel channel)
        {
            m_NetServer.SendMessage(
                messageBuffer, 
                m_NetServer.Connections,
                deliveryMethod,
                (int) channel
            );
        }

        public void BR_UpdateComponents(FNEEntity parent, params FNEComponentData[] components)
        {
            var message = m_EntityNetAPI.UpdateComponentsMessage(parent, components);
            Broadcast_All(
                message,
                NetDeliveryMethod.ReliableOrdered,
                SequenceChannel.ENTITY_STATE
            );
        }

        private void HandleSendResult(NetSendResult result, string nameOfCommand)
		{
			switch (result)
			{
				case NetSendResult.FailedNotConnected:
					Debug.LogError("Message failed to enqueue because there is no connection, Command: " + nameOfCommand);
					break;
				case NetSendResult.Sent:
					Debug.Log("Message was immediately sent, Command: " + nameOfCommand);
					break;
				case NetSendResult.Queued:
					Debug.Log("Message was queued for delivery, Command: " + nameOfCommand);
					break;
				case NetSendResult.Dropped:
					Debug.LogWarning("Message was dropped immediately since too many message were queued, Command: " + nameOfCommand);
					break;
			}
		}
	}
}

