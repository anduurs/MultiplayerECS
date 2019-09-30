using UnityEngine;
using FNZ.Server;
using FNZ.Client;

public class WorldBootstrap : MonoBehaviour
{
	public void Start()
	{
		gameObject.AddComponent<ServerApp>();
		gameObject.AddComponent<ClientApp>();
	}
}
