using FNZ.Shared;
using System.Collections.Generic;
using System.Diagnostics;
using Unity.Entities;
using UnityEngine;

namespace FNZ.Server
{
	public class ServerWorldBootstrap : MonoBehaviour
	{
		public static World ServerWorld;
		private List<ComponentSystemBase> m_Systems;

		public static double TICKS_PER_SECOND = 5;

		public static float DeltaTime;
		public static int tickCounter = 0;

		private double frameCounter = 0;
		private float passedTime = 0;

		private double seconds_per_tick = 1.0 / TICKS_PER_SECOND;

		public static long NanoTime()
		{
			long nano = 10000L * Stopwatch.GetTimestamp();
			nano /= System.TimeSpan.TicksPerMillisecond;
			nano *= 100L;
			return nano;
		}

		public void Start()
		{
			ServerWorld = new World("ServerWorld");
			m_Systems = WorldCreator.CreateWorldAndSystemsFromAssemblies(ServerWorld, "FNZ.Server", "FNZ.Shared");
			World.Active = null;
			ScriptBehaviourUpdateOrder.UpdatePlayerLoop(ServerWorld);
		}

		public void Update()
		{
			passedTime += Time.deltaTime;
			frameCounter += Time.deltaTime;

			if (passedTime >= 0.2f)
			{
				foreach (var system in m_Systems)
				{
					system.Update();
				}

				tickCounter++;
				passedTime = 0;
			}

			if (frameCounter >= 1.0f)
			{
				UnityEngine.Debug.Log("Ticks per second: " + tickCounter);
				frameCounter = 0;
				tickCounter = 0;
			}
		}

		public void OnApplicationQuit()
		{
			if (ServerWorld != null)
			{
				ServerWorld.QuitUpdate = true;
				ScriptBehaviourUpdateOrder.UpdatePlayerLoop(null);
				ServerWorld.Dispose();
			}
			else
			{
				UnityEngine.Debug.LogError("World has already been destroyed");
			}
		}
	}
}
