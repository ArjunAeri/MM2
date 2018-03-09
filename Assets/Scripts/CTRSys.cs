using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using UnityEngine.PostProcessing;

public class CTRSys : NetworkBehaviour//Claw Targeting Raycast System
{
	public int x, y ,z, offsetx, offsetY, offsetZ;
	public GameObject reticleGO;
	public Sprite Stage1, Stage2, Stage3;
	public GameObject camera;
	private int Ooffset = 8;
	private int stage = 1;
	private NetworkIdentity myId;
	public Vector3 direct;
	public GameObject aimreticle;
	public float distance = 0;
	Vector3 BoxDimensions;
	Vector3 spos;
  	public Quaternion forward;
	private const float toRad = (float) 3.14159265/180;
	public static Transform target = null;
	public static bool grapple = true;
private GameObject hud;
	private Text message, LavaDist;

	public PostProcessingProfile ppp;

	public ClawControl c;

	public GameObject lava;

	void Start () 
	{

		hud = GameObject.Find("HUDCONT").gameObject.transform.GetChild (0).gameObject;

		ppp.vignette.enabled = false;

		if (!isLocalPlayer)
			return;
		
		BoxDimensions = new Vector3 (x, y, z);
		forward = Quaternion.Euler(0,0,0);
		direct = Vector3.forward;
		spos = new Vector3 (transform.position.x, transform.position.y, transform.position.z); 

		InvokeRepeating("FindClosestTarget", 15, .5f); 

		myId = GetComponent<NetworkIdentity> ();

		lava = GameObject.FindGameObjectsWithTag ("Lava")[0];

		message  = hud.transform.GetChild (0).gameObject.GetComponent<Text>();
		LavaDist = hud.transform.GetChild (1).gameObject.GetComponent<Text>();
	}

	// Update is called once per frame
	void Update () 
	{
		
	}


	private void FindClosestTarget()
	{

		//Change lockon timing, adjust Oosize to make hard to get lock

		BoxDimensions = new Vector3 (x, y, z);

		spos = new Vector3 (transform.position.x + Ooffset * Mathf.Sin(transform.rotation.eulerAngles.y * toRad), transform.position.y + offsetY + 4, transform.position.z + Ooffset * Mathf.Cos(transform.rotation.eulerAngles.y * toRad)); 

		float xD = transform.position.x;

		RaycastHit[] hits = Physics.BoxCastAll (spos, BoxDimensions, direct, transform.rotation, distance);

		ArrayList objectsinscope = new ArrayList ();
		ArrayList hitIndex = new ArrayList ();

		for (int i = 0; i < hits.Length; i++) 
		{	
			GameObject cur = hits [i].transform.gameObject;

			if (cur.CompareTag ("Rover") || cur.CompareTag ("grapplehook"))
			{
				objectsinscope.Add (cur);
				hitIndex.Add (i);
			}
		}

		if (objectsinscope.Count < 1)
		{
			if(target !=null)
				if(target.CompareTag("Rover"))
					Cmdincstage (target.GetComponent<NetworkIdentity>(), 1);

			target = null;
			aimreticle.SetActive (false);
			stage =  1;
			return;
		}

		aimreticle.SetActive (true);

		GameObject closest = (GameObject) objectsinscope [0];

		float closestX = Mathf.Abs(xD - closest.transform.position.x);

		int closestIndex = 0;

		for(int i=0; i<objectsinscope.Count; i++)
		{
			GameObject a = (GameObject) objectsinscope [i];

			if ( Mathf.Abs (a.transform.position.x - xD) < closestX) 
			{
				closest = a;
				closestX = a.transform.position.x;
				closestIndex = i;
			}
		}

		target = closest.transform;

		grapple = (closest.CompareTag ("grapplehook")) ? true : false; 
	
		if (!grapple)
		{
			if (stage < 4)
			{
				c.stilllocking = true;
				increaseStage (target.gameObject.GetComponent<NetworkIdentity> ());
			}
		}

		else 
			c.stilllocking = false;
			
		moveAimReticle (target.position);

		float lavaDist = transform.position.y - lava.transform.position.y;

		LavaDist.text = string.Format ("{0:N2}",lavaDist);

		print ("Lava dist is: " + string.Format ("{0:N2}", lavaDist));

		if (lavaDist <= 2) 
		{
			VignetteModel.Settings m = ppp.vignette.settings;
			message.text = "Lava Close to Rover!";
			m.intensity = .685f;
			m.smoothness = .133f;
			m.roundness = .754f;
			ppp.vignette.enabled = true;
		}
	}

	private void moveAimReticle (Vector3 target)
	{
		aimreticle.transform.position = target;
	}

	private IEnumerator AutoAim(Vector3 moveTo)
	{
		print ("In coroutine");
		Vector3 direction = moveTo - aimreticle.transform.position;

		while (Mathf.Abs(direction.magnitude) > .3f) 
		{
			aimreticle.transform.position = Vector3.Lerp (aimreticle.transform.position, moveTo, Time.deltaTime * 20);
			yield return null;
		}
	}

	[Command]
	void Cmdincstage(NetworkIdentity targeted, int stage)
	{
		if (!targeted.isLocalPlayer)
		{
			print ("Rpc returned");
			Rpcincstage (targeted, stage);
			return;
		}

		if (stage == 2) 
		{
			hud.GetComponentInChildren<Text> ().text = "An enemy rover is locking on to you";
			VignetteModel.Settings m = ppp.vignette.settings;
			m.intensity = .685f;
			m.smoothness = .133f;
			m.roundness = .754f;
			ppp.vignette.enabled = true;
		} 

		else 
		{
			if (stage == 1) 
			{
				message.text = "";
				ppp.vignette.enabled = false;
				return;
			}

			if (stage == 3) 
			{
				VignetteModel.Settings m = ppp.vignette.settings;
				m.intensity = .776f;
				m.smoothness = .253f;
				m.roundness = .837f;
			}

			if (stage == 4) 
			{
				VignetteModel.Settings m = ppp.vignette.settings;
				m.intensity = .947f;
				m.smoothness = .741f;
				m.roundness = .89f;
			}
		}

		print ("Stage is" + stage);
	}

	[ClientRpc]
	void Rpcincstage(NetworkIdentity targeted, int stage)
	{
		if (!targeted.isLocalPlayer)
		{
			return;
		}

		if (stage == 2) 
		{
			hud.GetComponentInChildren<Text> ().text = "An enemy rover is locking on to you";
			VignetteModel.Settings m = ppp.vignette.settings;
			m.intensity = .685f;
			m.smoothness = .133f;
			m.roundness = .754f;
			ppp.vignette.enabled = true;
		} 

		else 
		{
			if (stage == 1) 
			{
				ppp.vignette.enabled = false;
				return;
			}

			if (stage == 3) 
			{
				VignetteModel.Settings m = ppp.vignette.settings;
				m.intensity = .776f;
				m.smoothness = .253f;
				m.roundness = .837f;
			}

			if (stage == 4) 
			{
				VignetteModel.Settings m = ppp.vignette.settings;
				m.intensity = .947f;
				m.smoothness = .741f;
				m.roundness = .89f;
			}
		}
	}

	private void increaseStage(NetworkIdentity target)
	{
		stage++;

		switch (stage) 
		{
			case 2:
				Cmdincstage (target, stage);
				reticleGO.GetComponent<Image> ().sprite = Stage2;
			break;

			case 3:
				Cmdincstage (target, stage);
				reticleGO.GetComponent<Image> ().sprite = Stage3;
			break;

		    case 4:
				Cmdincstage (target, stage);
				reticleGO.GetComponent<Image> ().sprite = Stage1;
				c.stilllocking = false;
			break;
		}
	}
}
