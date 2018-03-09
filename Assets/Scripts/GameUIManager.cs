using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameUIManager : MonoBehaviour 
{
	public Canvas c;
	public bool created = false;

	public Button create;
	public Button join;
	public Button mutiplayer;

	public GameObject[] HCSP, LavaSP;

	public GameObject broadcastObject;

	private NetworkBroadcaster mbroadcaster;
	// Use this for initialization
	void Start () 
	{
		mbroadcaster = broadcastObject.GetComponent<NetworkBroadcaster>();

		print ("Start Called");
		create.onClick.AddListener (() => makeGame ());

		join.onClick.AddListener (() => search());
	}

	public void makeGame()
	{
		if (!created) 
		{
			print ("Create game clicked");

			mbroadcaster.Init ();

			print ("Le brodacster init");

			mbroadcaster.StartAsServ();
		}
	}

	public void search()
	{
		mbroadcaster.Init ();
		mbroadcaster.startAsClient ();
	}


}
