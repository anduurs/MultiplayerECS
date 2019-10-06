using FNZ.Shared.Net;
using Lidgren.Network;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FNZ.Client.Net
{
	public partial class ClientNetworkAPI
	{
		private readonly NetClient m_NetClient;

		public ClientNetworkAPI(NetClient netClient)
		{
			m_NetClient = netClient;
		}

		public void Cmd_RequestWorldSpawn(string playerName)
		{
			var sendBuffer = m_NetClient.CreateMessage();
			sendBuffer.Write((byte)PacketType.REQUEST_WORLD_SPAWN);
			sendBuffer.Write(playerName);
			m_NetClient.SendMessage(sendBuffer, NetDeliveryMethod.ReliableOrdered, (int)SequenceChannel.WORLD_SETUP);
		}
	}
}

