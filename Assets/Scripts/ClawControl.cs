using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using UnityEngine.UI;

public class ClawControl : NetworkBehaviour
{
	public static bool shot = false;
	public static bool hit = false;
	public static bool shooting = false;

	public bool called = false;
	private bool grapple = false;
	public NetworkIdentity roverid;

	private Rigidbody clawphysics;
	private GameObject claw;
	private Rigidbody clawbody;
	ConfigurableJoint cj;
	public GameObject myrover;
	private Quaternion offset;
	private bool clawHitSomething = false;
	public GameObject claweffect;

	public bool stilllocking = false;

	GameObject clawspawn;

	private RoverDamageManager rdm;
	// Use this for initialization
	void Start ()
	{
		print("Clawcontrol started, " + roverid.gameObject);

		if (!roverid.gameObject.GetComponent<NetworkIdentity>().isLocalPlayer) 
		{
			print ("Destroying because not local player");
			Destroy (GetComponent<ConfigurableJoint> ());
			Destroy (this) ;
			return;
		}

		myrover = roverid.gameObject;

		print (roverid + " " + roverid.gameObject);

		myrover = roverid.gameObject;
			
		offset = Quaternion.Euler(0, 90, 0);
		clawphysics = GetComponent<Rigidbody> ();

		cj = GetComponent<ConfigurableJoint> ();
		cj.connectedBody = myrover.GetComponent<Rigidbody> ();

		claw = this.gameObject;

		print (claw + " " + claw==null);

		claweffect = myrover.GetComponent<WheelColliderController> ().clawspark;

		myrover.GetComponent<CTRSys> ().c = this;

		rdm = myrover.GetComponent<RoverDamageManager> ();

		clawspawn = myrover.GetComponent<WheelColliderController> ().clawspawn;

		return;
	}

	void Update()
	{
		if(!shooting && !shot)
			transform.rotation = myrover.transform.rotation * offset;
	}

	// Update is called once per frame
	void LateUpdate ()
	{
		if (WheelColliderController.inputstarted)
		{
			if (Input.GetKeyDown (KeyCode.Space) && !shot && !stilllocking) 
			{
				Destroy (GetComponent<ConfigurableJoint>());
				shot = true;
				shooting = true;
				hit = false;
				GetComponent<Collider>().enabled = true;

				if (CTRSys.target == null)
				{
					print("NO TARGET");
					clawphysics.AddForce (myrover.transform.forward * 300);

					StartCoroutine (BringBackClaw (true));
				}

				else 
				{
					print("TARGET ISGRAPPLE: " +  CTRSys.grapple);
					GetComponent<Collider> ().enabled = false;
					StartCoroutine(shootTarget(CTRSys.target, CTRSys.grapple, transform.position, Time.time));
				}
			}
		}
	}

	IEnumerator BringBackClaw(bool wait)
	{
		print ("Beginning Pause wait is " + wait);

		if(wait)
			yield return new WaitForSecondsRealtime (4);

		if (grapple)
			yield break;

		shooting = false;

		print("Claw done being shot");

		while (Mathf.Abs (clawphysics.position.z - myrover.transform.position.z) > 3) 
		{
			print ("In loop tryna get claw back g");
			transform.position = Vector3.Lerp (transform.position, new Vector3(myrover.transform.position.x, 0, myrover.transform.position.z), .01f);
			yield return null;
		}

		shooting = false;

		clawphysics.useGravity = false;

		print ("Calling reconnect claw " + myrover.GetComponent<NetworkIdentity> ());
		reconnectClaw ();
	}

	void reconnectClaw()
	{
		GetComponent<Collider> ().enabled = false;

		print ("Reconnect claw called, position is: ");
	
		transform.position = clawspawn.transform.position;

		clawspawn.transform.GetChild (0).GetComponent<ParticleSystem> ().Play ();

		this.gameObject.AddComponent<ConfigurableJoint>();
		ConfigurableJoint joint = GetComponent<ConfigurableJoint> ();
		GetComponent<ConfigurableJoint> ().xMotion = ConfigurableJointMotion.Locked;
		GetComponent<ConfigurableJoint> ().yMotion = ConfigurableJointMotion.Locked;
		GetComponent<ConfigurableJoint> ().zMotion = ConfigurableJointMotion.Locked;
		GetComponent<ConfigurableJoint> ().projectionMode = JointProjectionMode.PositionAndRotation;
		GetComponent<ConfigurableJoint> ().connectedBody = myrover.GetComponent<Rigidbody> ();

		shot = false;
		shooting = false;
	}

	void KillRover(NetworkIdentity roverhit, Rigidbody c)
	{
		StartCoroutine (BringBackClaw (false));
		reconnectClaw ();

		print ("Hit rover");
		//this.gameObject.AddComponent<FixedJoint> ();

		print (roverhit);

		print ("Calling CMD");
		print ("RoverID IS: " + roverid);
		rdm.takedamage (roverhit, roverid);

		if (myrover.GetComponent<RoverDamageManager> ().getHealth () <= 0) 
		{
			Destroy (GetComponent<FixedJoint> ());	
		}

		GetComponent<Rigidbody> ().useGravity = false;
	} 

	IEnumerator Rotate()
	{
		while (shooting) 
		{
			print ("In Loop");
			transform.Rotate (new Vector3 (0, 0, 1) * Time.deltaTime * 21);
			yield return null;
			//Rotate Clockwise
		}

		while (shot) 
		{
			print ("In Loop2");
			transform.Rotate (new Vector3 (0, 0, 1) * Time.deltaTime * 21);
			yield return null;
			//Rotate CounterClockwise.
		}
	}
		
	IEnumerator pullup(Vector3 postpos, Vector3 startpos, float starttime)
	{
		while (postpos.x - myrover.transform.position.x > 4 || postpos.y - myrover.transform.position.y > 4 || Mathf.Abs(postpos.z - myrover.transform.position.z) > 7) 
		{
			print ("In pullup loop");
			myrover.transform.position = Vector3.Lerp (startpos, postpos, Time.time - starttime);
			yield return null; 
		}

		Vector3 telportloc = new Vector3(postpos.x + 6, postpos.y, postpos.z);

		myrover.transform.position = telportloc;

		grapple = false;

		reconnectClaw ();
	}

	IEnumerator shootTarget(Transform target, bool grapple, Vector3 initshot, float start)
	{
		print ("SHOOT TARGET");

		while (Mathf.Abs (transform.position.z - target.position.z) > .3f) 
		{
			print ("Actually Lerpint to target: " + target);
			transform.position = Vector3.Lerp (initshot, target.position, Time.time - start);
			yield return null;
		}

		if (grapple)
			StartCoroutine (pullup (target.position, myrover.transform.position, Time.time));
		
		else 
			KillRover (target.gameObject.GetComponent<NetworkIdentity> (), transform.gameObject.GetComponent<Rigidbody> ());
	}	
}
