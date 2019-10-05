using FNZ.Server.Model.World;
using FNZ.Server.Net;
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
		public static World ECS_World;
		public static ServerWorld World;
		public static ServerNetworkAPI NetAPI;
		public static ServerNetworkReceiver NetChannel;

		public void Start()
		{
			ECS_World = new World("ServerWorld");
			var systems = WorldCreator.GetSystemsFromAssemblies(ECS_World, "FNZ.Server", "FNZ.Shared");

			var initializationSystemGroup = ECS_World.GetOrCreateSystem<InitializationSystemGroup>();
			var simulationSystemGroup = ECS_World.GetOrCreateSystem<SimulationSystemGroup>();
			var presentationSystemGroup = ECS_World.GetOrCreateSystem<PresentationSystemGroup>();
			
			foreach (var type in systems)
			{
				var groups = type.GetCustomAttributes(typeof(UpdateInGroupAttribute), true);

				if (groups.Length == 0)
				{
					simulationSystemGroup.AddSystemToUpdateList(ECS_World.GetOrCreateSystem(type) as ComponentSystemBase);
				}

				foreach (var g in groups)
				{
					var group = g as UpdateInGroupAttribute;

					if (group == null) continue;
					var groupMgr = ECS_World.GetOrCreateSystem(group.GroupType);
					var groupSys = groupMgr as ComponentSystemGroup;

					if (groupSys != null)
					{
						groupSys.AddSystemToUpdateList(ECS_World.GetOrCreateSystem(type) as ComponentSystemBase);
					}
				}
			}

			initializationSystemGroup.SortSystemUpdateList();
			simulationSystemGroup.SortSystemUpdateList();
			presentationSystemGroup.SortSystemUpdateList();
			WorldCreator.UpdatePlayerLoop(ECS_World);
		}

		public void OnApplicationQuit()
		{
			if (ECS_World != null)
			{
				ECS_World.QuitUpdate = true;
				ScriptBehaviourUpdateOrder.UpdatePlayerLoop(null);
				ECS_World.Dispose();
			}
			else
			{
				UnityEngine.Debug.LogError("World has already been destroyed");
			}
		}
	}
}
