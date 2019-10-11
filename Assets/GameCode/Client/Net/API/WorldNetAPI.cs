using FNZ.Shared.Net;
using Lidgren.Network;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FNZ.Client.Net.API
{
	internal class WorldNetAPI
	{
		private readonly NetClient m_NetClient;

		public WorldNetAPI(NetClient netClient)
		{
			m_NetClient = netClient;
		}

		public NetSendResult RequestWorldSpawn(string playerName)
		{
			var sendBuffer = m_NetClient.CreateMessage();

			sendBuffer.Write((byte)PacketType.REQUEST_WORLD_SPAWN);
			sendBuffer.Write(playerName);

			return m_NetClient.SendMessage(sendBuffer, NetDeliveryMethod.ReliableOrdered, (int)SequenceChannel.WORLD_SETUP);
		}
	}
}

