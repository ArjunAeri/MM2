using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using UnityEngine.UI;

public class MyNetManager : NetworkManager
{
	public Canvas c;
	public bool created;
	private bool RCOvveride;

	public Button create, join, multiplayer, rallycross;


	public Text title;
	public Text gamecreated;
	public Text subtitle;

	public NetworkDiscovery discovery;

	public NetworkStartPosition [] LavaSP;

	private NetworkConnection cacheHost;

	void Start()
	{
		print ("Start Called");
		create.onClick.AddListener (() => makeGame ());

		join.onClick.AddListener (() => search ());

		rallycross.onClick.AddListener (() => startRallyCross ());
		multiplayer.onClick.AddListener (() => showMultUI ());
	}

	public override void OnServerConnect(NetworkConnection conn)
	{
		print ("Custom ServerConnectCalled");
			
		base.OnClientConnect (cacheHost);
		base.OnServerConnect (conn);

	}

	public override void OnClientConnect(NetworkConnection conn)
	{
		print (NetworkManager.singleton.networkAddress + " " + conn.address);

		if (!conn.address.Equals ("localServer") || RCOvveride) {
			print ("Custom Client Connect");
			base.OnClientConnect (conn);
			c.enabled = false;
		} 

		else
			cacheHost = conn;
	}

	public void makeGame()
	{
		print ("Called, should fix");
		create.gameObject.SetActive (false);
		join.gameObject.SetActive (false);
		title.enabled = false;

		gamecreated.gameObject.SetActive (true);
		subtitle.gameObject.SetActive (true);

		discovery.Initialize ();
		discovery.StartAsServer();
		base.StartHost();
	}

	public void search()
	{
		discovery.Initialize ();
		discovery.StartAsClient();
	}

	private void showMultUI()
	{
		multiplayer.gameObject.SetActive (false);
		rallycross.gameObject.SetActive (false);
		create.gameObject.SetActive (true);
		join.gameObject.SetActive(true);

		c.worldCamera.enabled = false;
		c.worldCamera.enabled = true;
	}

	private void startRallyCross()
	{
		Destroy (LavaSP[0]);
		Destroy (LavaSP[1]);


		RCOvveride = true;

		makeGame ();
	}
}
