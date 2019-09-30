using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

namespace FNZ.Server.Model.World
{
	public struct WorldChunk
	{
		public byte chunkX;
		public byte chunkY;
		public byte size;

		public byte[] tileCosts;
		public byte[] tileDangerLevels;
		public bool[] tileBlockingList;
		public bool[] tileSeeThroughList;
		public int[] tilePositionsX;
		public int[] tilePositionsY;
		public float[] tileTemperatures;
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

		public ServerWorld(int widthInTiles, int heightInTiles, byte chunkSize)
		{
			WIDTH = widthInTiles;
			HEIGHT = heightInTiles;
			CHUNK_SIZE = chunkSize;

			WIDTH_IN_CHUNKS = WIDTH / CHUNK_SIZE;
			HEIGHT_IN_CHUNKS = HEIGHT / CHUNK_SIZE;

			chunks = new WorldChunk[WIDTH_IN_CHUNKS, HEIGHT_IN_CHUNKS];
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
	}
}

