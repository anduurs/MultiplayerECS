using FNZ.Server.Net;
using FNZ.Shared.Model.Entity;
using FNZ.Shared.Net;
using FNZ.Shared.Utils;
using Lidgren.Network;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

namespace FNZ.Server.Model.World
{
	public class WorldChunkManager
	{
		private int m_ChunkRadius = 1;

		private ServerWorld m_World;
		private Dictionary<NetConnection, PlayerChunkState> m_PlayerChunkStates;

		public WorldChunkManager(ServerWorld world)
		{
			m_World = world;
			m_PlayerChunkStates = new Dictionary<NetConnection, PlayerChunkState>();
			ServerApp.NetConnector.Register(NetMessageType.REQUEST_WORLD_SPAWN, OnRequestWorldSpawnPacketRecieved);
		}

		public void OnRequestWorldSpawnPacketRecieved(ServerNetworkConnector net, NetIncomingMessage incMsg)
		{
			string playerName = incMsg.ReadString();
			float2 playerPosition = new float2(ServerApp.World.WIDTH / 2, ServerApp.World.HEIGHT / 2);

			var newPlayer = ServerApp.EntityFactory.CreatePlayer(playerPosition, playerName);
			var clientConnection = incMsg.SenderConnection;

			net.SyncEntity(newPlayer);
			net.AddConnectedClient(newPlayer, clientConnection);

			OnPlayerEnteringNewChunk(newPlayer);
		}

		public void AddClientToChunkStreamingSystem(NetConnection clientConnection, PlayerChunkState chunkState)
		{
			m_PlayerChunkStates.Add(clientConnection, chunkState);
		}

		public void OnPlayerEnteringNewChunk(FNEEntity player)
		{
			var newChunk = m_World.GetWorldChunk(player.Position);

			int2 chunkPos = ServerApp.World.GetChunkIndices(player.Position);

			if (newChunk == null)
			{
				newChunk = LoadWorldChunk((byte)chunkPos.x, (byte)chunkPos.y);
			}

			NetConnection clientConnection = ServerApp.NetConnector.GetConnectionFromPlayer(player);
			PlayerChunkState state = m_PlayerChunkStates[clientConnection];

			byte newChunkX = newChunk.ChunkX;
			byte newChunkY = newChunk.ChunkY;

			List<WorldChunk> newLoadedChunks = new List<WorldChunk>();

			for (int y = -m_ChunkRadius; y <= m_ChunkRadius; y++)
			{
				for (int x = -m_ChunkRadius; x <= m_ChunkRadius; x++)
				{
					byte cx = (byte)(newChunkX + x);
					byte cy = (byte)(newChunkY + y);

					if (cx < 0 || cy < 0 || cx >= m_World.WIDTH_IN_CHUNKS || cy >= m_World.HEIGHT_IN_CHUNKS) continue;

					WorldChunk nChunk = m_World.chunks[cx, cy];

					if (nChunk == null)
					{
						nChunk = LoadWorldChunk(cx, cy);
					}

					if (!state.CurrentlyLoadedChunks.Contains(nChunk) &&
						!state.ChunksSentForLoadAwaitingConfirm.Contains(nChunk) &&
						!state.ChunksAwaitingLoad.Contains(nChunk))
					{
						state.ChunksAwaitingLoad.Add(nChunk);
					}

					state.ChunksAwaitingUnload.RemoveAll(t => t.Item1 == nChunk);

					newLoadedChunks.Add(nChunk);
				}
			}

			state.CurrentlyActiveChunks.Clear();
			int innerRadius = m_ChunkRadius - 1;

			for (int y = -innerRadius; y <= innerRadius; y++)
			{
				for (int x = -innerRadius; x <= innerRadius; x++)
				{
					byte cx = (byte)(newChunkX + x);
					byte cy = (byte)(newChunkY + y);

					if (cx < 0 || cy < 0 || cx >= m_World.WIDTH_IN_CHUNKS || cy >= m_World.HEIGHT_IN_CHUNKS) continue;

					WorldChunk nChunk = m_World.chunks[cx, cy];

					state.CurrentlyActiveChunks.Add(nChunk);
				}
			}

			int i = 0;
			while (i < state.CurrentlyLoadedChunks.Count)
			{
				var chunk = state.CurrentlyLoadedChunks[i];

				if (!newLoadedChunks.Contains(chunk) &&
					state.ChunksAwaitingUnload.FindAll(c => c.Item1 == chunk).Count == 0 &&
					!state.ChunksSentForUnloadAwaitingConfirm.Contains(chunk))
				{
					state.ChunksAwaitingUnload.Add(new Tuple<WorldChunk, long>(chunk, FNETime.NanoTime()));
				}

				i++;
			}

			i = 0;
			while (i < state.ChunksAwaitingLoad.Count)
			{
				var chunk = state.ChunksAwaitingLoad[i];

				if (!newLoadedChunks.Contains(chunk))
				{
					state.ChunksAwaitingLoad.RemoveAt(i);
				}
				else
				{
					i++;
				}
			}

			foreach (var chunk in state.ChunksSentForLoadAwaitingConfirm)
			{
				if (!newLoadedChunks.Contains(chunk))
				{
					state.ChunksAwaitingUnload.Add(new Tuple<WorldChunk, long>(chunk, FNETime.NanoTime()));
				}
			}
		}

		private WorldChunk LoadWorldChunk(byte chunkX, byte chunkY)
		{
			var chunk = new WorldChunk
			{
				ChunkX = chunkX,
				ChunkY = chunkY,
				Size = m_World.CHUNK_SIZE
			};

			int size = chunk.Size * chunk.Size;

			chunk.TileIds = new ushort[size];

			chunk.TileCosts = new byte[size];
			chunk.TileDangerLevels = new byte[size];

			chunk.TileBlockingList = new bool[size];
			chunk.TileSeeThroughList = new bool[size];
			
			chunk.TilePositionsX = new int[size];
			chunk.TilePositionsY = new int[size];

			chunk.TileTemperatures = new float[size];

			m_World.chunksToLoad.Add(chunk);

			ServerApp.WorldGen.GenerateChunk(chunk);

			return chunk;
		}

		public void GetConnectionsWithChunkLoaded(WorldChunk chunk, ref List<NetConnection> conns)
		{
			foreach (var conn in m_PlayerChunkStates.Keys)
			{
				if (m_PlayerChunkStates[conn].CurrentlyLoadedChunks.Contains(chunk))
				{
					if (!conns.Contains(conn)) conns.Add(conn);
				}
			}
		}
	}
}

