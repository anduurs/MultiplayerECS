using FNZ.Shared.Model.Entity;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

namespace FNZ.Server.Model.World
{
	public class WorldChunk
	{
		public byte ChunkX;
		public byte ChunkY;
		public byte Size;

		public ushort[] TileIds;
		public byte[] TileCosts;
		public byte[] TileDangerLevels;
		public bool[] TileBlockingList;
		public bool[] TileSeeThroughList;
		public int[] TilePositionsX;
		public int[] TilePositionsY;
		public float[] TileTemperatures;
	}

	public class ServerWorld
	{
		// The size of a chunk in tiles (64 means the chunk is 64x64 tiles large)
		public readonly byte CHUNK_SIZE;
		// The number of chunks in a row of the terrain
		public readonly int HEIGHT_IN_CHUNKS;
		// The number of chunks in a column of the terrain
		public readonly int WIDTH_IN_CHUNKS;
		// The total width in tiles for the terrain
		public readonly int WIDTH;
		// The total height in tiles for the terrain
		public readonly int HEIGHT;

		public int SeedX;
		public int SeedY;

		public WorldChunk[,] chunks;

		public List<WorldChunk> currentlyLoadedChunks = new List<WorldChunk>();
		public List<WorldChunk> chunksToLoad = new List<WorldChunk>();
		public List<WorldChunk> chunksToUnload = new List<WorldChunk>();

		private List<FNEEntity> m_Players = new List<FNEEntity>();

		public ServerWorld(int widthInTiles, int heightInTiles, byte chunkSize)
		{
			WIDTH = widthInTiles;
			HEIGHT = heightInTiles;
			CHUNK_SIZE = chunkSize;

			WIDTH_IN_CHUNKS = WIDTH / CHUNK_SIZE;
			HEIGHT_IN_CHUNKS = HEIGHT / CHUNK_SIZE;

			chunks = new WorldChunk[WIDTH_IN_CHUNKS, HEIGHT_IN_CHUNKS];
		}

		public void Tick()
		{

		}

		public void AddPlayerEntity(FNEEntity playerEntity)
		{
			m_Players.Add(playerEntity);
		}

		public void RemovePlayerEntity(FNEEntity playerToRemove)
		{
			m_Players.Remove(playerToRemove);
		}

		public List<FNEEntity> GetAllPlayers()
		{
			return m_Players;
		}

		public WorldChunk GetWorldChunk(float2 position)
		{
			int worldX = (int)position.x;
			int worldY = (int)position.y;

			if (worldX < 0)
				return default;
			if (worldX >= WIDTH_IN_CHUNKS * CHUNK_SIZE)
				return default;
			if (worldY < 0)
				return default;
			if (worldY >= HEIGHT_IN_CHUNKS * CHUNK_SIZE)
				return default;

			int chunkYpos = worldY % CHUNK_SIZE;
			int chunkYnr = (worldY - chunkYpos) / CHUNK_SIZE;
			int chunkXpos = worldX % CHUNK_SIZE;
			int chunkXnr = (worldX - chunkXpos) / CHUNK_SIZE;

			return chunks[chunkXnr, chunkYnr];
		}

		public int2 GetChunkIndices(float2 position)
		{
			int worldX = (int)position.x;
			int worldY = (int)position.y;
			int chunkYpos = worldY % CHUNK_SIZE;
			int chunkYnr = (worldY - chunkYpos) / CHUNK_SIZE;
			int chunkXpos = worldX % CHUNK_SIZE;
			int chunkXnr = (worldX - chunkXpos) / CHUNK_SIZE;
			return new int2(chunkXnr, chunkYnr);
		}

        public int2 GetChunkTileIndices(WorldChunk chunk, float2 worldPosition)
        {
            int worldX = (int) worldPosition.x;
            int worldY = (int) worldPosition.y;

            int tileXnr = worldX - (chunk.ChunkX * CHUNK_SIZE);
            int tileYnr = worldY - (chunk.ChunkY * CHUNK_SIZE);

            return new int2(tileXnr, tileYnr);
        }

        public float GetTileTemperature(float2 position)
        {
            WorldChunk chunk = GetWorldChunk(position);
            int2 tileIndices = GetChunkTileIndices(chunk, position);

            return chunk.TileTemperatures[tileIndices.x + tileIndices.y * CHUNK_SIZE];
        }
    }
}

