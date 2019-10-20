using FNZ.Shared.Model.Entity;
using FNZ.Shared.Model.Entity.Components;
using FNZ.Shared.Net;
using Lidgren.Network;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FNZ.Server.Net.API
{
	internal class EntityNetAPI
	{
		private readonly NetServer m_NetServer;

		public EntityNetAPI(NetServer netServer)
		{
            m_NetServer = netServer;
		}

		public NetOutgoingMessage UpdateComponentsMessage(FNEEntity parent, params FNEComponentData[] components)
		{
			int componentDataSizeInBytes = 0;

			foreach (var component in components)
				componentDataSizeInBytes += component.GetSizeInBytes();

			var sendBuffer = m_NetServer.CreateMessage(1 + 1 + 4 + componentDataSizeInBytes);

			sendBuffer.Write((byte)PacketType.UPDATE_COMPONENT);
			sendBuffer.Write((byte)components.Length);
			sendBuffer.Write(parent.entityNetId);

			foreach (var component in components)
			{
				// sendBuffer.Write(GetCompIdFromType(component.GetType()));
				component.Serialize(sendBuffer);
			}

            return sendBuffer;
		}
	}
}

