using FNZ.Shared.Model;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FNZ.Server.Model.World
{
	public class WorldGenerator
	{
		public void GenerateWorld(WorldGenData data, int seedX, int seedY)
		{
			IdTranslator.Instance.GenerateMissingIds();

			var world = new ServerWorld(255 * data.chunkSize, 255 * data.chunkSize, data.chunkSize)
			{
				SeedX = seedX,
				SeedY = seedY
			};

			ServerApp.World = world;
		}

		public void GenerateChunk(WorldChunk chunkToGenerate, WorldGenData data)
		{
			byte chunkX = chunkToGenerate.chunkX;
			byte chunkY = chunkToGenerate.chunkY;

			float[] heightMap = GenerateHeightMap(data, chunkX, chunkY, ServerApp.World.SeedX, ServerApp.World.SeedY);

			for (int y = 0; y < data.chunkSize; y++)
			{
				for (int x = 0; x < data.chunkSize; x++)
				{
					float height = Mathf.Clamp(heightMap[x + y * data.chunkSize], -1.0f, 1.0f);

					foreach (var biomeTileData in data.tilesInBiome)
					{
						
					}
				}
			}
		}

		private float[] GenerateHeightMap(WorldGenData data, int chunkX, int chunkY, int seedX, int seedY)
		{
			float[] heightMap = new float[data.chunkSize * data.chunkSize];

			float layerFrequency = data.layerFrequency;
			float layerWeight = data.layerWeight;

			for (int octave = 0; octave < data.octaves; octave++)
			{
				for (int y = 0; y < data.chunkSize; y++)
				{
					for (int x = 0; x < data.chunkSize; x++)
					{
						float inputX = chunkX * data.chunkSize + x + seedX;
						float inputY = chunkY * data.chunkSize + y + seedY;
						float noise = data.layerWeight * SimplexNoise.Noise(inputX * layerFrequency, inputY * layerFrequency);
						heightMap[x + y * data.chunkSize] += noise;
					}
				}

				layerFrequency *= 2.0f;
				layerWeight *= data.roughness;
			}

			return heightMap;
		}

		private float Distance(float x1, float y1, float x2, float y2)
		{
			return (float)(Mathf.Sqrt(Mathf.Pow(Mathf.Abs(x1 - x2), 2) + Mathf.Pow(Mathf.Abs(y1 - y2), 2)));
		}
	}
}

