﻿using FNZ.Client.Net;
using FNZ.Shared.Model;
using FNZ.Shared.Net;
using Lidgren.Network;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FNZ.Client.Model.World
{
	public class ClientWorld
	{
		// The size of a chunk in tiles (64 means the chunk is 64x64 tiles large)
		public byte CHUNK_SIZE;
		// The number of chunks in a row of the terrain
		public int HEIGHT_IN_CHUNKS;
		// The number of chunks in a column of the terrain
		public int WIDTH_IN_CHUNKS;
		// The total width in tiles for the terrain
		public int WIDTH;
		// The total height in tiles for the terrain
		public int HEIGHT;

		public ClientWorld()
		{
			ClientApp.NetConnector.Register(NetMessageType.WORLD_SETUP, OnWorldSetupDataReceived);
		}

		public void OnWorldSetupDataReceived(ClientNetworkConnector net, NetIncomingMessage incMsg)
		{
			Debug.LogWarning("WORLD_SETUP packet received in client world!");
			IdTranslator.Instance.Deserialize(incMsg);

			WIDTH = incMsg.ReadInt32();
			HEIGHT = incMsg.ReadInt32();
			CHUNK_SIZE = incMsg.ReadByte();

			ClientApp.NetAPI.Cmd_RequestWorldSpawn("player");
		}
	}
}

