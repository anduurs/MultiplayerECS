using FNZ.Server.Model;
using FNZ.Server.Model.World;
using FNZ.Server.Net;
using FNZ.Shared;
using FNZ.Shared.Model;
using FNZ.Shared.Utils;
using Unity.Entities;
using UnityEngine;

namespace FNZ.Server
{
	public class ServerApp : MonoBehaviour
	{
		public static World ECS_World;
		public static ServerWorld World;
		public static ServerNetworkAPI NetAPI;
		public static ServerNetworkConnector NetConnector;
		public static ServerEntityFactory EntityFactory;
		public static WorldChunkManager ChunkManager;
		public static WorldGenerator WorldGen;

		public void Start()
		{
			ECS_World = new World("ServerWorld");
			WorldGen  = new WorldGenerator(DataBank.Instance.GetData<WorldGenData>("default_world"));

			World = WorldGen.GenerateWorld(
				FNERandom.GetRandomIntInRange(0, 1600000),
				FNERandom.GetRandomIntInRange(0, 1600000)
			);

			ChunkManager  = new WorldChunkManager(World);
			EntityFactory = new ServerEntityFactory(World);

			var systems = ECSWorldCreator.GetSystemsFromAssemblies(ECS_World, "FNZ.Server", "FNZ.Shared");

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
			ECSWorldCreator.UpdatePlayerLoop(ECS_World);
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
