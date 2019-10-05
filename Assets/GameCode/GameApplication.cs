using UnityEngine;
using FNZ.Server;
using FNZ.Client;

public class GameApplication : MonoBehaviour
{
	public void Start()
	{
		gameObject.AddComponent<ServerApp>();
		gameObject.AddComponent<ClientApp>();
	}
}
