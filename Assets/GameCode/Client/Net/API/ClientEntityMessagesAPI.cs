using FNZ.Shared.Model.Entity.Components;
using FNZ.Shared.Net;
using Lidgren.Network;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FNZ.Client.Net.API
{
	internal class ClientEntityMessagesAPI
	{
		private readonly NetClient m_NetClient;

		public ClientEntityMessagesAPI(NetClient netClient)
		{
			m_NetClient = netClient;
		}
	}
}

