using FNZ.Server.Model.World;
using FNZ.Shared.Model;
using FNZ.Shared.Net;
using Lidgren.Network;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FNZ.Server.Net.API
{
	internal class WorldNetAPI
	{
		private readonly NetPeer m_NetPeer;

		public WorldNetAPI(NetPeer netPeer)
		{
			m_NetPeer = netPeer;
		}

		public NetSendResult SendWorldSetupDataToClient(int widthInTiles, int heightInTiles, byte chunkSize, NetConnection clientConnection)
		{
			var sendBuffer = m_NetPeer.CreateMessage();
			sendBuffer.Write((byte)PacketType.WORLD_SETUP);

			IdTranslator.Instance.Serialize(sendBuffer);

			sendBuffer.Write(widthInTiles);
			sendBuffer.Write(heightInTiles);
			sendBuffer.Write(chunkSize);

			return clientConnection.SendMessage(sendBuffer, NetDeliveryMethod.ReliableOrdered, (int)SequenceChannel.WORLD_SETUP);
		}
	}
}

