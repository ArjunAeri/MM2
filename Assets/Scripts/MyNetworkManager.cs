using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class MyNetworkManager : NetworkManager
{
	public NetworkDiscovery broadcast;

	public void StartGameHost()
	{
		broadcast.Initialize ();
		broadcast.StartAsClient ();
	}

	public void StartGameClient()
	{
		broadcast.showGUI = false;
	}

	public override void OnStartHost()
	{
		broadcast.StopBroadcast ();
		print ("Start Host Broadcast");
		broadcast.showGUI = true;
	}
}
