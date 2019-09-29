using UnityEngine;
using Unity.Entities;
using System.Collections.Generic;
using FNZ.Shared;
using UnityEngine.Experimental.LowLevel;

namespace FNZ.Client
{
	public class ClientWorldBootstrap : MonoBehaviour
	{
		public static World ClientWorld;

		public void Start()
		{
			ClientWorld = new World("ClientWorld");
			var systems = WorldCreator.GetSystemsFromAssemblies(ClientWorld, "FNZ.Client", "FNZ.Shared");

			var initializationSystemGroup = ClientWorld.GetOrCreateSystem<InitializationSystemGroup>();
			var simulationSystemGroup = ClientWorld.GetOrCreateSystem<SimulationSystemGroup>();
			var presentationSystemGroup = ClientWorld.GetOrCreateSystem<PresentationSystemGroup>();

			foreach (var type in systems)
			{
				var groups = type.GetCustomAttributes(typeof(UpdateInGroupAttribute), true);

				if (groups.Length == 0)
				{
					simulationSystemGroup.AddSystemToUpdateList(WorldCreator.GetBehaviourManagerAndLogException(ClientWorld, type) as ComponentSystemBase);
				}

				foreach (var g in groups)
				{
					var group = g as UpdateInGroupAttribute;

					if (group == null) continue;

					if (!(typeof(ComponentSystemGroup)).IsAssignableFrom(group.GroupType))
					{
						UnityEngine.Debug.LogError($"Invalid [UpdateInGroup] attribute for {type}: {group.GroupType} must be derived from ComponentSystemGroup.");
						continue;
					}

					var groupMgr = WorldCreator.GetBehaviourManagerAndLogException(ClientWorld, group.GroupType);

					if (groupMgr == null)
					{
						UnityEngine.Debug.LogWarning(
							$"Skipping creation of {type} due to errors creating the group {group.GroupType}. Fix these errors before continuing.");
						continue;
					}

					var groupSys = groupMgr as ComponentSystemGroup;

					if (groupSys != null)
					{
						groupSys.AddSystemToUpdateList(WorldCreator.GetBehaviourManagerAndLogException(ClientWorld, type) as ComponentSystemBase);
					}
				}
			}

			initializationSystemGroup.SortSystemUpdateList();
			simulationSystemGroup.SortSystemUpdateList();
			presentationSystemGroup.SortSystemUpdateList();

			WorldCreator.UpdatePlayerLoop(ClientWorld);
		}

		public void OnApplicationQuit()
		{
			if (ClientWorld != null)
			{
				ClientWorld.QuitUpdate = true;
				ScriptBehaviourUpdateOrder.UpdatePlayerLoop(null);
				ClientWorld.Dispose();
			}
			else
			{
				Debug.LogError("World has already been destroyed");
			}
		}
	}
}

