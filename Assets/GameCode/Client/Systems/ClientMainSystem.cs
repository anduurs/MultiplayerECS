using FNZ.Shared;
using Unity.Entities;
using UnityEngine;

namespace FNZ.Client.Systems
{
	[UpdateInGroup(typeof(SimulationSystemGroup))]
	public class ClientMainSystem : ComponentSystem
	{
		private NetworkClientSystem m_NetClientSystem;

		protected override void OnCreate()
		{
			Debug.LogWarning("ClientSystem Created");

			m_NetClientSystem = ClientApp.ECS_ClientWorld.GetOrCreateSystem<NetworkClientSystem>();
			m_NetClientSystem.InitializeClient(SharedConfigs.AppIdentifier, "127.0.0.1", 7676);
		}

		protected override void OnUpdate()
		{
			//Debug.LogWarning("ClientSystem Updating");
			m_NetClientSystem.Update();
		}

		protected override void OnDestroy()
		{
			Debug.LogWarning("ClientMainSystem Destroying");
		}
	}
}
