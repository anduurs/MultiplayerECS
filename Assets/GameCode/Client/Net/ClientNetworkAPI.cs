using FNZ.Client.Net.API;
using FNZ.Shared.Net;
using Lidgren.Network;
using UnityEngine;

namespace FNZ.Client.Net
{
	public class ClientNetworkAPI
	{
		private readonly NetClient m_NetClient;

		// All network modules 
		private readonly ClientWorldMessagesAPI m_WorldAPI;
		private readonly ClientEntityMessagesAPI m_EntityAPI;

		public ClientNetworkAPI(NetClient netClient)
		{
			m_NetClient = netClient;

			m_WorldAPI  = new ClientWorldMessagesAPI(netClient);
			m_EntityAPI = new ClientEntityMessagesAPI(netClient); 
		}

		public void Cmd_RequestWorldSpawn(string playerName)
		{
			var message = m_WorldAPI.CreateRequestWorldSpawnMessage(playerName);
			Command(message);
		}

		private void Command(NetMessage message)
		{
			var result = m_NetClient.SendMessage(
				message.Buffer, 
				message.DeliveryMethod, 
				(int)message.Channel
			);

			HandleSendResult(result, message.Type);
		}

		private void HandleSendResult(NetSendResult result, NetMessageType messageType)
		{
			string messageTypeName = messageType.ToString();

			switch (result)
			{
				case NetSendResult.FailedNotConnected:
					Debug.LogError("[Client] Message failed to enqueue because there is no connection, CommandType: " + messageTypeName);
					break;
				case NetSendResult.Sent:
					Debug.Log("[Client] Message was immediately sent, CommandType: " + messageTypeName);
					break;
				case NetSendResult.Queued:
					Debug.Log("[Client] Message was queued for delivery, CommandType: " + messageTypeName);
					break;
				case NetSendResult.Dropped:
					Debug.LogWarning("[Client] Message was dropped immediately since too many message were queued, CommandType: " + messageTypeName);
					break;
			}
		}
	}
}

