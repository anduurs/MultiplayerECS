using FNZ.Client.Net.Api;
using FNZ.Shared.Model.Components;
using Lidgren.Network;
using UnityEngine;

namespace FNZ.Client.Net
{
	public class ClientNetworkAPI
	{
		private readonly NetClient m_NetClient;

		// Api Controllers
		private readonly WorldController m_WorldController;
		private readonly EntityController m_EntityController;

		public ClientNetworkAPI(NetClient netClient)
		{
			m_NetClient = netClient;

			m_WorldController  = new WorldController(netClient);
			m_EntityController = new EntityController(netClient); 
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="playerName"></param>
		public void Cmd_RequestWorldSpawn(string playerName)
		{
			var result = m_WorldController.RequestWorldSpawn(playerName);
			HandleSendResult(result, "Cmd_RequestWorldSpawn");
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="components"></param>
		public void Cmd_UpdateComponents(params FNEComponent[] components)
		{
			var result = m_EntityController.UpdateComponents(components);
			HandleSendResult(result, "Cmd_UpdateComponents");
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

