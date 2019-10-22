using FNZ.Shared.Net;
using Lidgren.Network;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FNZ.Client.Net.API
{
	internal class ClientWorldMessagesAPI
	{
		private readonly NetClient m_NetClient;

		public ClientWorldMessagesAPI(NetClient netClient)
		{
			m_NetClient = netClient;
		}

		public NetMessage CreateRequestWorldSpawnMessage(string playerName)
		{
			var sendBuffer = m_NetClient.CreateMessage();

			sendBuffer.Write((byte)NetMessageType.REQUEST_WORLD_SPAWN);
			sendBuffer.Write(playerName);

			return new NetMessage
			{
				Buffer = sendBuffer,
				Type = NetMessageType.REQUEST_WORLD_SPAWN,
				DeliveryMethod = NetDeliveryMethod.ReliableOrdered,
				Channel = SequenceChannel.WORLD_SETUP,
			};
		}
	}
}

