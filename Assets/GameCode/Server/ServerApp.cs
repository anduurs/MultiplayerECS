using FNZ.Server.Model.World;
using FNZ.Shared;
using System.Collections.Generic;
using System.Diagnostics;
using Unity.Entities;
using UnityEngine;
using UnityEngine.Experimental.LowLevel;

namespace FNZ.Server
{
	public class ServerApp : MonoBehaviour
	{
		public static World ECS_ServerWorld;
		public static ServerWorld World;

		public void Start()
		{
			ECS_ServerWorld = new World("ServerWorld");
			var systems = WorldCreator.GetSystemsFromAssemblies(ECS_ServerWorld, "FNZ.Server", "FNZ.Shared");

			var initializationSystemGroup = ECS_ServerWorld.GetOrCreateSystem<InitializationSystemGroup>();
			var simulationSystemGroup = ECS_ServerWorld.GetOrCreateSystem<SimulationSystemGroup>();
			var presentationSystemGroup = ECS_ServerWorld.GetOrCreateSystem<PresentationSystemGroup>();
			
			foreach (var type in systems)
			{
				var groups = type.GetCustomAttributes(typeof(UpdateInGroupAttribute), true);

				if (groups.Length == 0)
				{
					simulationSystemGroup.AddSystemToUpdateList(ECS_ServerWorld.GetOrCreateSystem(type) as ComponentSystemBase);
				}

				foreach (var g in groups)
				{
					var group = g as UpdateInGroupAttribute;

					if (group == null) continue;
					var groupMgr = ECS_ServerWorld.GetOrCreateSystem(group.GroupType);
					var groupSys = groupMgr as ComponentSystemGroup;

					if (groupSys != null)
					{
						groupSys.AddSystemToUpdateList(ECS_ServerWorld.GetOrCreateSystem(type) as ComponentSystemBase);
					}
				}
			}

			initializationSystemGroup.SortSystemUpdateList();
			simulationSystemGroup.SortSystemUpdateList();
			presentationSystemGroup.SortSystemUpdateList();
			WorldCreator.UpdatePlayerLoop(ECS_ServerWorld);
		}

		public void OnApplicationQuit()
		{
			if (ECS_ServerWorld != null)
			{
				ECS_ServerWorld.QuitUpdate = true;
				ScriptBehaviourUpdateOrder.UpdatePlayerLoop(null);
				ECS_ServerWorld.Dispose();
			}
			else
			{
				UnityEngine.Debug.LogError("World has already been destroyed");
			}
		}
	}
}
