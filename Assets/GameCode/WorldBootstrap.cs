using UnityEngine;
using FNZ.Server;
using FNZ.Client;
using Unity.Entities;
using UnityEngine.Experimental.LowLevel;
using UnityEngine.Experimental.PlayerLoop;
using FNZ.Shared;

// Edit -> Project Settings -> Player: Scripting Define Symbols, add: UNITY_DISABLE_AUTOMATIC_SYSTEM_BOOTSTRAP

public class WorldBootstrap : MonoBehaviour
{
	public void Start()
	{
		gameObject.AddComponent<ServerWorldBootstrap>();
		gameObject.AddComponent<ClientWorldBootstrap>();

		//WorldCreator.UpdatePlayerLoop(ServerWorldBootstrap.ServerWorld, ClientWorldBootstrap.ClientWorld);
		//WorldCreator.UpdatePlayerLoop(ClientWorldBootstrap.ClientWorld);
	}

	
}
