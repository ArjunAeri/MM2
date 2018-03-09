using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class NetworkBroadcasterer : NetworkDiscovery
{
	public override void OnReceivedBroadcast(string fromAddress, string information)
	{
		print ("OnReceiveBroadCast Called");
		NetworkManager.singleton.networkAddress = fromAddress;
		NetworkManager.singleton.StartClient();
		base.StopBroadcast ();
	}
}
