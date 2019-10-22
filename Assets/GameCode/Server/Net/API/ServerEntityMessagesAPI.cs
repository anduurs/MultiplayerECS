using FNZ.Shared.Model.Entity;
using FNZ.Shared.Model.Entity.Components;
using FNZ.Shared.Net;
using Lidgren.Network;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FNZ.Server.Net.API
{
	internal class ServerEntityMessagesAPI
	{
		private readonly NetServer m_NetServer;

		public ServerEntityMessagesAPI(NetServer netServer)
		{
            m_NetServer = netServer;
		}

		public NetMessage CreateUpdateComponentMessage(FNEEntity parent, params FNEComponentData[] components)
		{
			int componentDataSizeInBytes = 0;

			foreach (var component in components)
				componentDataSizeInBytes += component.GetSizeInBytes();

			var sendBuffer = m_NetServer.CreateMessage(1 + 1 + 4 + componentDataSizeInBytes);

			sendBuffer.Write((byte)NetMessageType.UPDATE_COMPONENT);
			sendBuffer.Write((byte)components.Length);
			sendBuffer.Write(parent.NetId);

			foreach (var component in components)
			{
				// sendBuffer.Write(GetCompIdFromType(component.GetType()));
				component.Serialize(sendBuffer);
			}

            return new NetMessage 
			{ 
				Buffer = sendBuffer,
				Type = NetMessageType.UPDATE_COMPONENT,
				DeliveryMethod = NetDeliveryMethod.ReliableOrdered,
				Channel = SequenceChannel.ENTITY_STATE,
			};
		}
	}
}

