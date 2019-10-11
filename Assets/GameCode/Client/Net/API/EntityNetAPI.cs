using FNZ.Shared.Model.Components;
using FNZ.Shared.Net;
using Lidgren.Network;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FNZ.Client.Net.API
{
	internal class EntityNetAPI
	{
		private readonly NetClient m_NetClient;

		public EntityNetAPI(NetClient netClient)
		{
			m_NetClient = netClient;
		}

		public NetSendResult UpdateComponents(params FNEComponent[] components)
		{
			int componentDataSizeInBytes = 0;

			foreach (var component in components)
				componentDataSizeInBytes += component.GetSizeInBytes();

			var sendBuffer = m_NetClient.CreateMessage(1 + 1 + 4 + componentDataSizeInBytes);

			sendBuffer.Write((byte)PacketType.UPDATE_COMPONENT);
			sendBuffer.Write((byte)components.Length);
			sendBuffer.Write(components[0].parent.entityNetId);

			foreach (var component in components)
			{
				// sendBuffer.Write(GetCompIdFromType(component.GetType()));
				component.Serialize(sendBuffer);
			}

			return m_NetClient.SendMessage(sendBuffer, NetDeliveryMethod.ReliableOrdered, (int)SequenceChannel.ENTITY_STATE);
		}
	}
}

