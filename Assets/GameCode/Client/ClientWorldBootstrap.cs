using UnityEngine;
using Unity.Entities;
using System.Collections.Generic;
using FNZ.Shared;

namespace FNZ.Client
{
	public class ClientWorldBootstrap : MonoBehaviour
	{
		public static World ClientWorld;
		private List<ComponentSystemBase> m_Systems;

		public void Start()
		{
			ClientWorld = new World("ClientWorld");
			m_Systems = WorldCreator.CreateWorldAndSystemsFromAssemblies(ClientWorld, "FNZ.Client", "FNZ.Shared");
			World.Active = null;
			ScriptBehaviourUpdateOrder.UpdatePlayerLoop(ClientWorld);
		}

		public void Update()
		{
			foreach (var system in m_Systems)
			{
				system.Update();
			}
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

