using FNZ.Server.Model.World;
using FNZ.Server.Net.API;
using Lidgren.Network;
using UnityEngine;

namespace FNZ.Server.Net
{
	public class ServerNetworkAPI
	{
		private readonly WorldNetAPI m_WorldNetAPI;

		public ServerNetworkAPI(NetPeer netPeer)
		{
			m_WorldNetAPI = new WorldNetAPI(netPeer);
		}

		public void SendToClient_WorldSetup(int widthInTiles, int heightInTiles, byte chunkSize, NetConnection clientConnection)
		{
			ServerApp.ChunkManager.AddClientToChunkStreamingSystem(clientConnection, new PlayerChunkState());
			var result = m_WorldNetAPI.SendWorldSetupDataToClient(widthInTiles, heightInTiles, chunkSize, clientConnection);
			HandleSendResult(result, "SendToClient_WorldSetup");
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

