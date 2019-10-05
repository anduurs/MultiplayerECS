using FNZ.Shared.Model;
using Lidgren.Network;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FNZ.Server.Net
{

	public partial class ServerNetworkAPI
	{
		private readonly NetPeer m_NetPeer;

		public ServerNetworkAPI(NetPeer netPeer)
		{
			m_NetPeer = netPeer;
		}

		public void SendWorldSetupDataToClient(int widthInTiles, int heightInTiles, byte chunkSize, NetConnection clientConnection)
		{
			var sendBuffer = m_NetPeer.CreateMessage();
			sendBuffer.Write((byte)1);

			IdTranslator.Instance.Serialize(sendBuffer);

			sendBuffer.Write(widthInTiles);
			sendBuffer.Write(heightInTiles);
			sendBuffer.Write(chunkSize);

			clientConnection.SendMessage(sendBuffer, NetDeliveryMethod.ReliableOrdered, 0);
		}
		


	}
}

