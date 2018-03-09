using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;


	public class RoverDamageManager : NetworkBehaviour
	{

		public GameObject[] nonplayervisualwheels;
		public GameObject hudcont;
		private GameObject hud;
		public GameObject mlw;
		public GameObject mrw;
		public GameObject deathexplosion;
		public NetworkIdentity clawId;
		private float health = 50;
		public NetworkIdentity killer;
		public int mRamspeed; 

		private bool dead = false; 

	void Start()
	{
		hud = GameObject.Find("HUDCONT").gameObject.transform.GetChild (0).gameObject;
		hud.SetActive (true);
	}

	void Update()
	{
		UpdateNonPlayerWheels ();
	}

	void UpdateNonPlayerWheels()
	{
		for (int i = 0; i < 2; i++) 
		{
			nonplayervisualwheels [i].transform.localRotation = mlw.transform.localRotation;
			nonplayervisualwheels [i + 2].transform.localRotation = mrw.transform.localRotation;
		}
	}

	public float getHealth()
	{
		return health;
	}

	public void takedamage(NetworkIdentity hitrover, NetworkIdentity attackrover)
	{
		print ("KILLER IS " + attackrover);
		killer = attackrover;

		Cmdtakedamage (hitrover, attackrover);
	}


	public void killLava(GameObject rover)
	{
		if(dead)
			return;
		
		GameObject dpe = (GameObject)Instantiate (deathexplosion);
		dpe.transform.position = rover.transform.position;

		StartCoroutine(waitforexplosionanddestroy(rover, dpe));

		dead = true;
	}

	[Command]
	public void CmdKillLava(NetworkIdentity rover)
	{
		RpcKillLava (rover);
	}

	[ClientRpc]
	public void RpcKillLava(NetworkIdentity rover)
	{
		Destroy (rover);
	}

	[Command]
	void Cmdtakedamage(NetworkIdentity rover, NetworkIdentity attackrover)
	{
		print ("CmdDamageCalled on " + isLocalPlayer);

		if (isServer)
		{
			GameObject roverGO = rover.gameObject;

			WheelColliderController rwcc = roverGO.GetComponent<WheelColliderController>();

			print ("Setting false");

			GameObject dpe = (GameObject)Instantiate (deathexplosion);
			dpe.transform.position = roverGO.transform.position;

			StartCoroutine(waitforexplosionanddestroy(roverGO, dpe));
		}

		Rpctakedamage (rover, attackrover);
	}

	[ClientRpc]
	void Rpctakedamage(NetworkIdentity hitrover, NetworkIdentity attackrover)
	{
		print ("RPC damage called");
		GameObject roverGO = hitrover.gameObject;

		WheelColliderController rwcc = roverGO.GetComponent<WheelColliderController>();

		print (health);

		roverGO.GetComponent<RoverDamageManager> ().killer = attackrover;
			
		print ("Setting false");

		GameObject dpe = (GameObject)Instantiate (deathexplosion);
		dpe.transform.position = roverGO.transform.position;
		dpe.GetComponent<ParticleSystem>().Play ();

		StartCoroutine(waitforexplosionanddestroy(roverGO, dpe));
	}

	public void OnCollisionEnter(Collider other)
	{
		if(other.CompareTag("rover"))
		{
			print ("rover collission");
			float myV = this.gameObject.GetComponent<Rigidbody>().velocity.magnitude;

			if (other.gameObject.GetComponent<Rigidbody> ().velocity.magnitude > myV || myV < mRamspeed)
				return;

			Cmdtakedamage (other.gameObject.GetComponent<NetworkIdentity> (), GetComponent<NetworkIdentity> ());
		}
	}

	public void OnDestroy()
	{
		if (!isLocalPlayer)
			return;

		if (killer == null)
			return;
		
		print ("This Rover was kill ;(");

		print (killer);
		Camera c = (Camera) killer.gameObject.GetComponentsInChildren<Camera> ().GetValue (1);
		c.enabled = true;

		hud.SetActive (true);
		hud.GetComponent<Canvas>().enabled = true;
		hud.GetComponentInChildren<Text> ().text = "Rover is kill ;(";
	}

	IEnumerator waitforexplosionanddestroy(GameObject roverGO, GameObject dpe)
	{
		LavaTriggerDetect.finishRound ();

		yield return new WaitForSeconds (2);
		Destroy (clawId.gameObject);
		Destroy (roverGO);

		Destroy (dpe);
	}
}


