using FNZ.Server.Model.World;
using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

namespace FNZ.Server.Systems
{
	[UpdateInGroup(typeof(SimulationSystemGroup))]
	public class WorldSimulationSystem : ComponentSystem
	{
		private ServerWorld m_World;

		protected override void OnCreate()
		{
			base.OnCreate();
		}

		protected override void OnUpdate()
		{
			//Debug.Log("WorldSimulationSystem OnUpdate");
		}

		protected override void OnDestroy()
		{
			base.OnDestroy();
		}
	}
}

