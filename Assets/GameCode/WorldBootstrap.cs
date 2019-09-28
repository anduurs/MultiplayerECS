using UnityEngine;
using FNZ.Server;
using FNZ.Client;

// Edit -> Project Settings -> Player: Scripting Define Symbols, add: UNITY_DISABLE_AUTOMATIC_SYSTEM_BOOTSTRAP

public class WorldBootstrap : MonoBehaviour
{
	public void Start()
	{
		gameObject.AddComponent<ServerWorldBootstrap>();
		gameObject.AddComponent<ClientWorldBootstrap>();
	}
}
