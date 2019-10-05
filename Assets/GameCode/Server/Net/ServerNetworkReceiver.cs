using FNZ.Shared.Net;
using Lidgren.Network;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FNZ.Server.Net
{
	public interface INetworkListener
	{
		void OnPacketReceived(ServerNetworkReceiver receiver, NetIncomingMessage incMsg);
	}

	public class ServerNetworkReceiver
	{
		private Dictionary<PacketType, Action<ServerNetworkReceiver, NetIncomingMessage>> m_PacketReceiverMap;

		public ServerNetworkReceiver()
		{
			m_PacketReceiverMap = new Dictionary<PacketType, Action<ServerNetworkReceiver, NetIncomingMessage>>();
		}

		public void Register(PacketType packetType, Action<ServerNetworkReceiver, NetIncomingMessage> func)
		{
			m_PacketReceiverMap.Add(packetType, func);
		}

		public void Execute(PacketType packetType, NetIncomingMessage incMsg)
		{
			if (m_PacketReceiverMap.ContainsKey(packetType))
			{
				m_PacketReceiverMap[packetType].Invoke(this, incMsg);
			}
		}
	}
}
