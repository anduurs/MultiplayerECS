using Lidgren.Network;
using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

namespace FNZ.Server.Systems
{
	public class NetworkServerSystemGroup : ComponentSystemGroup { }

	[DisableAutoCreation]
	[UpdateInGroup(typeof(NetworkServerSystemGroup))]
	public class NetworkServerSystem : ComponentSystem
	{
		private NetServer m_Server;

		public void InitializeServer(string appIdentifier, int port, int maxConnections)
		{
			var config = new NetPeerConfiguration(appIdentifier)
			{
				Port = port,
				MaximumConnections = maxConnections
			};

			m_Server = new NetServer(config);
			m_Server.Start();

			Debug.Log("Lidgren NetServer initialized and started");
		}

		protected override void OnCreate()
		{
			Debug.Log("NetworkServerSystem Created");
		}

		protected override void OnUpdate()
		{
			NetIncomingMessage msg;
			//Debug.Log("NetworkServerSystem OnUpdate");

			while ((msg = m_Server.ReadMessage()) != null)
			{
				msg.m_readPosition = 0;

				switch (msg.MessageType)
				{
					case NetIncomingMessageType.Data:
						ParsePacket(msg);
						break;
					case NetIncomingMessageType.VerboseDebugMessage:
					case NetIncomingMessageType.DebugMessage:
					case NetIncomingMessageType.WarningMessage:
					case NetIncomingMessageType.ErrorMessage:
						break;
					case NetIncomingMessageType.StatusChanged:
						switch (msg.SenderConnection.Status)
						{
							case NetConnectionStatus.Connected:
								OnClientConnected(msg.SenderConnection);
								break;

							case NetConnectionStatus.Disconnected:
								Debug.Log("Client: " + msg.SenderConnection.ToString() + " disconnected from server!");

								OnClientDisconnected(msg.SenderConnection);
								break;
						}
						break;
					default:
						ParsePacket(msg);
						break;
				}
			}
		}

		private void ParsePacket(NetIncomingMessage incMsg)
		{

			m_Server.Recycle(incMsg);
		}

		private void OnClientConnected(NetConnection clientConnection)
		{
			Debug.Log("Client: " + clientConnection.ToString() + " connected to server!");

		}

		private void OnClientDisconnected(NetConnection clientConnection)
		{

		}

		protected override void OnDestroy()
		{
			Debug.Log("NetworkServerSystem Destroying");
		}
	}
}
