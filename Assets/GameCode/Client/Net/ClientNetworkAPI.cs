using FNZ.Client.Net.API;
using FNZ.Shared.Model.Entity.Components;
using Lidgren.Network;
using UnityEngine;

namespace FNZ.Client.Net
{
	public class ClientNetworkAPI
	{
		// All network modules 
		private readonly WorldNetAPI m_WorldNetAPI;
		private readonly EntityNetAPI m_EntityNetAPI;

		public ClientNetworkAPI(NetClient netClient)
		{
			m_WorldNetAPI  = new WorldNetAPI(netClient);
			m_EntityNetAPI = new EntityNetAPI(netClient); 
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="playerName"></param>
		public void Cmd_RequestWorldSpawn(string playerName)
		{
			var result = m_WorldNetAPI.RequestWorldSpawn(playerName);
			HandleSendResult(result, "Cmd_RequestWorldSpawn");
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

