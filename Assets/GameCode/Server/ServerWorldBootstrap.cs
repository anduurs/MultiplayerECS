using FNZ.Shared;
using System.Collections.Generic;
using System.Diagnostics;
using Unity.Entities;
using UnityEngine;
using UnityEngine.Experimental.LowLevel;

namespace FNZ.Server
{
	public class ServerWorldBootstrap : MonoBehaviour
	{
		public static World ServerWorld;

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
			var systems = WorldCreator.GetSystemsFromAssemblies(ServerWorld, "FNZ.Server", "FNZ.Shared");

			var initializationSystemGroup = ServerWorld.GetOrCreateSystem<InitializationSystemGroup>();
			var simulationSystemGroup = ServerWorld.GetOrCreateSystem<SimulationSystemGroup>();
			var presentationSystemGroup = ServerWorld.GetOrCreateSystem<PresentationSystemGroup>();
			
			foreach (var type in systems)
			{
				var groups = type.GetCustomAttributes(typeof(UpdateInGroupAttribute), true);

				if (groups.Length == 0)
				{
					simulationSystemGroup.AddSystemToUpdateList(ServerWorld.GetOrCreateSystem(type) as ComponentSystemBase);
				}

				foreach (var g in groups)
				{
					var group = g as UpdateInGroupAttribute;

					if (group == null) continue;
					var groupMgr = ServerWorld.GetOrCreateSystem(group.GroupType);
					var groupSys = groupMgr as ComponentSystemGroup;

					if (groupSys != null)
					{
						groupSys.AddSystemToUpdateList(ServerWorld.GetOrCreateSystem(type) as ComponentSystemBase);
					}
				}
			}

			initializationSystemGroup.SortSystemUpdateList();
			simulationSystemGroup.SortSystemUpdateList();
			presentationSystemGroup.SortSystemUpdateList();
			WorldCreator.UpdatePlayerLoop(ServerWorld);
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
