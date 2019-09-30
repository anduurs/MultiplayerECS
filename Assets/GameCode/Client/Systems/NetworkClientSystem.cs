using Lidgren.Network;
using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

namespace FNZ.Client.Systems
{
	public class NetworkClientSystemGroup : ComponentSystemGroup { }

	[DisableAutoCreation]
	[UpdateInGroup(typeof(NetworkClientSystemGroup))]
	public class NetworkClientSystem : ComponentSystem
	{
		private NetClient m_Client;

		public void InitializeClient(string appIdentifier, string ip, int port)
		{
			NetPeerConfiguration config = new NetPeerConfiguration(appIdentifier);
			m_Client = new NetClient(config);
			m_Client.Start();

			NetOutgoingMessage approval = m_Client.CreateMessage();
			approval.Write("secret");

			Debug.LogWarning("C: IP is " + ip + ":" + port);

			m_Client.Connect(ip, port, approval);

			Debug.LogWarning("Connecting... allegedly...");
		}

		protected override void OnCreate()
		{
			base.OnCreate();
			Debug.LogWarning("NetworkClientSystem created!");
		}

		protected override void OnDestroy()
		{
			Debug.LogWarning("NetworkClientSystem destroyed!");
			base.OnDestroy();
		}

		protected override void OnUpdate()
		{
			//Debug.LogWarning("NetworkClientSystem OnUpdate");
			NetIncomingMessage incMsg;

			while ((incMsg = m_Client.ReadMessage()) != null)
			{
				switch (incMsg.MessageType)
				{
					case NetIncomingMessageType.Error:
						break;
					case NetIncomingMessageType.ConnectionApproval:
						break;
					case NetIncomingMessageType.Data:
						ParsePacket(incMsg);
						break;
					case NetIncomingMessageType.VerboseDebugMessage:
					case NetIncomingMessageType.DebugMessage:
					case NetIncomingMessageType.WarningMessage:
					case NetIncomingMessageType.ErrorMessage:
						break;
					case NetIncomingMessageType.StatusChanged:
						switch (incMsg.SenderConnection.Status)
						{
							case NetConnectionStatus.Connected:
								OnConnected();
								break;
							case NetConnectionStatus.Disconnected:

								break;
						}
						break;
					default:

						break;
				}
			}
		}

		private void ParsePacket(NetIncomingMessage incMsg)
		{

			m_Client.Recycle(incMsg);
		}

		private void OnConnected()
		{
			Debug.Log("Client Connected To Server!");
		}
	}
}


