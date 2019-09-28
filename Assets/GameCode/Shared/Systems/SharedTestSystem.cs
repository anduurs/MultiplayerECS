using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

namespace FNZ.Shared.Systems
{
	[DisableAutoCreation]
	public class SharedTestSystem : ComponentSystem
	{
		protected override void OnCreate()
		{
			base.OnCreate();
			Debug.Log("SharedTestSystem Created!");
		}

		protected override void OnUpdate()
		{

		}
	}
}

