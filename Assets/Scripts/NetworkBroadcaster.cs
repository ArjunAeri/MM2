using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class NetworkBroadcaster : NetworkDiscovery
{
	private bool client = false;
	private bool server = false;

	public override void OnReceivedBroadcast(string fromAddress, string information)
	{
		print ("OnReceiveBroadCast Called");
		NetworkManager.singleton.networkAddress = fromAddress;
		NetworkManager.singleton.StartClient();
		base.StopBroadcast ();
	}

	public void Init()
	{
		base.Initialize ();
		print (base.hostId);
	}

	public void StartAsServ()
	{
		if (server)
			return;
		
		if (!client)
			base.StartAsServer ();
		
		else 
		{
			base.StopBroadcast ();
			base.Initialize ();
			base.StartAsServer ();
			client = false;
		}

		server = true;
	}

	public void startAsClient()
	{
		if (client)
			return;

		if (!server)
			base.StartAsClient();

		else 
		{
			base.StopBroadcast ();
			base.Initialize ();
			base.StartAsClient ();
			server = false;
		}

		client = true;
	}
}
