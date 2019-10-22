using FNZ.Server.Model.World;
using FNZ.Shared.Model;
using FNZ.Shared.Net;
using Lidgren.Network;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FNZ.Server.Net.API
{
	internal class ServerWorldMessagesAPI
	{
		private readonly NetServer m_NetServer;

		public ServerWorldMessagesAPI(NetServer netServer)
		{
			m_NetServer = netServer;
		}

		public NetMessage CreateWorldSetupMessage(int widthInTiles, int heightInTiles, byte chunkSize)
		{
			var sendBuffer = m_NetServer.CreateMessage();
			sendBuffer.Write((byte)NetMessageType.WORLD_SETUP);

			IdTranslator.Instance.Serialize(sendBuffer);

			sendBuffer.Write(widthInTiles);
			sendBuffer.Write(heightInTiles);
			sendBuffer.Write(chunkSize);

			return new NetMessage
			{
				Buffer = sendBuffer,
				Type = NetMessageType.WORLD_SETUP,
				DeliveryMethod = NetDeliveryMethod.ReliableOrdered,
				Channel = SequenceChannel.WORLD_SETUP,
			};
		}
	}
}

