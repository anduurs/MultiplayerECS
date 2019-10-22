using FNZ.Server.Net;
using FNZ.Shared.Net;
using Lidgren.Network;
using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

namespace FNZ.Server.Systems
{
	[DisableAutoCreation]
	public class NetworkServerSystem : ComponentSystem
	{
		private NetServer m_Server;
		private ServerNetworkConnector m_NetConnector;

		public void InitializeServer(string appIdentifier, int port, int maxConnections)
		{
			var config = new NetPeerConfiguration(appIdentifier)
			{
				Port = port,
				MaximumConnections = maxConnections
			};

			m_Server = new NetServer(config);
			ServerApp.NetAPI = new ServerNetworkAPI(m_Server);
			ServerApp.NetConnector = new ServerNetworkConnector();
			m_NetConnector = ServerApp.NetConnector;
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
			m_NetConnector.Dispatch((NetMessageType)incMsg.ReadByte(), incMsg);
			m_Server.Recycle(incMsg);
		}

		private void OnClientConnected(NetConnection clientConnection)
		{
			Debug.Log("Client: " + clientConnection.ToString() + " connected to server!");

			ServerApp.NetAPI.STC_World_WorldSetup(
				ServerApp.World.WIDTH,
				ServerApp.World.HEIGHT,
				ServerApp.World.CHUNK_SIZE,
				clientConnection
			);

			Debug.Log("Sent world setup packet to client");
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
