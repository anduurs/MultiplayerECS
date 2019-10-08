using FNZ.Shared.Model;
using FNZ.Shared.Net;
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
			sendBuffer.Write((byte)PacketType.WORLD_SETUP);

			IdTranslator.Instance.Serialize(sendBuffer);

			sendBuffer.Write(widthInTiles);
			sendBuffer.Write(heightInTiles);
			sendBuffer.Write(chunkSize);

			var result = clientConnection.SendMessage(sendBuffer, NetDeliveryMethod.ReliableOrdered, (int)SequenceChannel.WORLD_SETUP);
			HandleSendResult(result);
		}

		private void HandleSendResult(NetSendResult result)
		{
			switch (result)
			{
				case NetSendResult.FailedNotConnected:
					break;
				case NetSendResult.Sent:
					break;
				case NetSendResult.Queued:
					break;
				case NetSendResult.Dropped:
					break;
			}
		}
	}
}

