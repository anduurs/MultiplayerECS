using FNZ.Shared.Model;
using FNZ.Shared.Model.World.Tile;
using FNZ.Shared.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FNZ.Server.Model.World
{
	public class WorldGenerator
	{
		private readonly WorldGenData m_WorldGenData;

		public WorldGenerator(WorldGenData data)
		{
			m_WorldGenData = data;
		}

		public ServerWorld GenerateWorld(int seedX, int seedY)
		{
			IdTranslator.Instance.GenerateMissingIds();

			var world = new ServerWorld(255 * m_WorldGenData.chunkSize, 255 * m_WorldGenData.chunkSize, m_WorldGenData.chunkSize)
			{
				SeedX = seedX,
				SeedY = seedY
			};

			return world;
		}

		public void GenerateChunk(WorldChunk chunk)
		{
			byte chunkX = chunk.ChunkX;
			byte chunkY = chunk.ChunkY;
			byte chunkSize = chunk.Size;

			float[] heightMap = GenerateHeightMap(chunkX, chunkY, chunkSize, ServerApp.World.SeedX, ServerApp.World.SeedY);

			for (int y = 0; y < chunkSize; y++)
			{
				for (int x = 0; x < chunkSize; x++)
				{
					float height = Mathf.Clamp(heightMap[x + y * chunkSize], -1.0f, 1.0f);

					foreach (var biomeTileData in m_WorldGenData.tilesInBiome)
					{
						TileData tileData = DataBank.Instance.GetData<TileData>(biomeTileData.tileRef);
						byte tileId = tileData.textureSheetIndex;

						if (biomeTileData.height >= height)
						{
							int index = x + y * chunkSize;

							chunk.TileIds[index] = IdTranslator.Instance.GetId<TileData>(tileData.nameDef);

							chunk.TilePositionsX[index] = x + chunkX * chunkSize;
							chunk.TilePositionsY[index] = y + chunkY * chunkSize;

							if (FNERandom.GetRandomIntInRange(0, 30) == 0)
								chunk.TileDangerLevels[index] = (byte)FNERandom.GetRandomIntInRange(0, 2);

							break;
						}
					}
				}
			}
		}

		private float[] GenerateHeightMap(byte chunkX, byte chunkY, byte chunkSize, int seedX, int seedY)
		{
			float[] heightMap = new float[chunkSize * chunkSize];

			float layerFrequency = m_WorldGenData.layerFrequency;
			float layerWeight = m_WorldGenData.layerWeight;

			for (int octave = 0; octave < m_WorldGenData.octaves; octave++)
			{
				for (int y = 0; y < chunkSize; y++)
				{
					for (int x = 0; x < chunkSize; x++)
					{
						float inputX = chunkX * chunkSize + x + seedX;
						float inputY = chunkY * chunkSize + y + seedY;
						float noise = m_WorldGenData.layerWeight * SimplexNoise.Noise(inputX * layerFrequency, inputY * layerFrequency);
						heightMap[x + y * chunkSize] += noise;
					}
				}

				layerFrequency *= 2.0f;
				layerWeight *= m_WorldGenData.roughness;
			}

			return heightMap;
		}
	}
}

