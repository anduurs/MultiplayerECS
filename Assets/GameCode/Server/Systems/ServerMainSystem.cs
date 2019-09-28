using FNZ.Shared;
using Unity.Entities;
using UnityEngine;

namespace FNZ.Server.Systems
{
	[DisableAutoCreation]
	public class ServerMainSystem : ComponentSystem
	{
		private NetworkServerSystem m_NetworkServerSystem;

		protected override void OnCreate()
		{
			Debug.Log("ServerSystem Created");

			m_NetworkServerSystem = ServerWorldBootstrap.ServerWorld.GetOrCreateSystem<NetworkServerSystem>();
			m_NetworkServerSystem.InitializeServer(SharedConfigs.AppIdentifier, 7676, 5);
		}

		protected override void OnUpdate()
		{
			//Debug.Log("ServerSystem Updating");

			m_NetworkServerSystem.Update();
		}

		protected override void OnDestroy()
		{
			Debug.Log("ServerMainSystem Destroying");
		}
	}
}

