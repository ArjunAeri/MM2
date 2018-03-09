using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.Networking;

public class WheelColliderController : NetworkBehaviour {

	public int roverTorque;
    public GameObject     [] visualwheels;
	public WheelCollider  [] wheelcolliders;

	private WheelCollider [] left;
	private WheelCollider [] right;

	public GameObject clawspark;

	bool mobile=false;
	float vertical;
	float horizontal;
	public ParticleSystem [] movingdebris;
	Vector3 wheelrotinterval;
	private Rigidbody rover;
	public GameObject rovercam;
	public static bool skip = false;
	private Vector3 rovercaminterval;
	float rotcameraz;
	public static bool inputstarted = false;
	public static Rigidbody rova;
	public GameObject clawspawn;
	private float rotspeed = 1;
	private float health = 50f;


 	 void Start()
	 {
		GetComponent<RoverDamageManager> ().nonplayervisualwheels = new GameObject[] { visualwheels [0], visualwheels [2], visualwheels [3], visualwheels [5] };

		if (!GetComponent<NetworkIdentity>().isLocalPlayer)
		{
			print ("NOT LOCAL DESTROYING");
			DestroyUnnecessary ();
		}

		rovercaminterval = new Vector3 (0, 0, 2);
		rover = GetComponent<Rigidbody> ();
		wheelrotinterval = new Vector3 (30, 0, 0);

		if (Application.platform == RuntimePlatform.Android||Application.platform == RuntimePlatform.IPhonePlayer) 
			mobile = true;

		left = new WheelCollider [3];
		right = new WheelCollider[3];

		for (int i = 0; i <= 2; i++) //This loop goes through and adds the first 3 wheels to the left array and the last three to the right array.
		{
			left  [i] = wheelcolliders [i];
			right [i] = wheelcolliders [i + 3];
		}

		if (!isLocalPlayer) 
		{
			Destroy (this);
			return;
		}


		if (isLocalPlayer) {
			Camera child = GetComponentInChildren<Camera> ();
			child.enabled = true;
		} 

		else {
			Camera child = GetComponentInChildren<Camera> ();
			Destroy (child);
			Destroy (this);
			return;
		}
			
		rova = GetComponent<Rigidbody> ();
	}

	void Update () 
	{
		if (!inputstarted)
			return;

		if (!movingdebris [0].isPlaying) {
			movingdebris [0].Play ();
			movingdebris [1].Play ();
		}
		
		if (vertical > 0) 
		{
			movingdebris [0].transform.localPosition =  new Vector3 (-1.2f, -1.3f, 3.06f);
			movingdebris [1].transform.localPosition =  new Vector3 ( 1.2f,  -1.3f, 3.06f);
			movingdebris [0].transform.localRotation = Quaternion.Euler(new Vector3 (0, 180, 0));
			movingdebris [1].transform.localRotation = Quaternion.Euler (new Vector3 (0, 180, 0));
		}

		else 
		{
			if (vertical < 0) 
			{
				movingdebris [0].transform.localPosition = new Vector3(-1.2f, -1.3f, -1.28f);
				movingdebris [1].transform.localPosition = new Vector3(1.2f, -1.3f, -1.28f);
				movingdebris [0].transform.localRotation = Quaternion.Euler(new Vector3 (0, 0, 0));
				movingdebris [1].transform.localRotation = Quaternion.Euler (new Vector3 (0, 0, 0));
			}
		}

		movingdebris [0].playbackSpeed= Mathf.Abs(wheelcolliders [0].rpm / 180);
		movingdebris [1].playbackSpeed= Mathf.Abs(wheelcolliders [1].rpm / 180);
	}

	// Update is called once per frame
	void FixedUpdate ()
	{
		if(inputstarted)
			processInput ();
		
		rovercam.transform.localRotation = Quaternion.Slerp (rovercam.transform.localRotation, Quaternion.Euler (0, 0, rotcameraz), Time.deltaTime * rotspeed);
			
		updateAllVisual ();
	}

	private void addTorque(WheelCollider [] wheelset, float directionSpeed)
	{ 
		for (int i = 0; i < wheelset.Length; i++) 
			wheelset [i].motorTorque = directionSpeed;
	}

	private void addBrakeTorque(WheelCollider [] wheelset, float directionSpeed)
	{
		for (int i = 0; i < wheelset.Length; i++) 
			wheelset [i].brakeTorque = directionSpeed;
	}

	private void updateVisualRotation(int colliderpos) //Ignore this for now.
	{
		Vector3 vect;
		Quaternion rot;
		wheelcolliders[colliderpos].GetWorldPose(out vect, out rot);
		visualwheels[colliderpos].transform.position = vect;
		visualwheels[colliderpos].transform.rotation = rot;
	}
		

	private void updateAllVisual() 
	{
		Vector3 position;
		Quaternion rotation;

		for(int i=0;i<wheelcolliders.Length;i++)
		{
			wheelcolliders[i].GetWorldPose(out position, out rotation);
			position.z -= .05F;
			visualwheels  [i].transform.position = position;
			visualwheels  [i].transform.rotation = rotation;
		}
	}

	private void stabilizeRover() //Kill any stray wheelrotation if user doesn't move for a while
	{
		foreach (WheelCollider current in wheelcolliders) 
		{
			current.brakeTorque = 100;
		}
	}

	private void ResetBeforeApply(WheelCollider [] wheels)
	{
		foreach (WheelCollider cur in wheels) 
		{
			cur.motorTorque = 0.0f;
			cur.brakeTorque = 0.0f;
		}
	}

	[ClientRpc]
	public void Rpctakedamage(NetworkIdentity rover)
	{
		print ("RPC damage called");
		GameObject roverGO = rover.gameObject;

		WheelColliderController rwcc = roverGO.GetComponent<WheelColliderController>();


		print ("Setting false");

		roverGO.SetActive (false);
	}

	[Command]
	public void Cmdtakedamage(NetworkIdentity rover)
	{
		print ("CmdDamageCalled");
		Rpctakedamage (rover);
	}

	private void processInput()
	{
		if (mobile) 
		{
			vertical = Input.acceleration.y;
			horizontal = Input.acceleration.x;
			
			vertical = (Mathf.Abs(Input.acceleration.y) > .2f) ? vertical + (1 - vertical) * 75 : 0;
			horizontal = (Mathf.Abs(Input.acceleration.x) > .2f) ? horizontal + (1 - horizontal) * 75 : 0;
		} 

		else 
		{
			vertical = Input.GetAxis ("Vertical");
			horizontal = Input.GetAxis("Horizontal");
		}

		//If user is only moving forward, and not rotating at all;
		if (Mathf.Abs (vertical) > 0 && Mathf.Abs(horizontal) == 0) 
		{
			addTorque (wheelcolliders, roverTorque * vertical);
			if (vertical > 0)
				rotcameraz = 0;
			else
				rotcameraz = 180;
		} 

		else 
		{
			if (rotspeed != 1)
				rotspeed = 1;

			if (Mathf.Abs (vertical) == 0 && Mathf.Abs (horizontal) > 0) 
			{ 
				//If rover is rotating in a certain direction, no forward or torque
				addTorque (right, 2 * -roverTorque * horizontal);
				addTorque (left,  2 * roverTorque * horizontal);
				if (horizontal > 0)
					rotcameraz = 90;
				else
					rotcameraz =-90;
			}

			else 
			{
				if (Mathf.Abs (vertical) > 0 && Mathf.Abs (horizontal) > 0)
				{
					if (horizontal > 0) 
					{
						addTorque (left, 3 * roverTorque * horizontal * vertical);
						addTorque (right, roverTorque * horizontal / 3);

						rotcameraz = (vertical > 0) ?  45 : 135;
					} 

					else 
					{
						addTorque (left, roverTorque * -horizontal * vertical / 3);
						addTorque (right, 3 * roverTorque * -horizontal * vertical);

						rotcameraz = (vertical > 0) ? -45 : 225;
					}

				}

				//No user input
				else
				{
					rotspeed = .2f;
					rotcameraz = 0;
					addTorque (wheelcolliders, roverTorque * vertical);
				}
			}
		}
	}

	private void DestroyUnnecessary()
	{
		print ("Called, destroy");
		AnimationEventScheduler eAnimS = GetComponent<AnimationEventScheduler> ();
		Destroy (eAnimS.animationcamera);
		Destroy (eAnimS.blackout);
		Destroy (GetComponent<WheelColliderController> ());
		Destroy (transform.GetChild(transform.childCount-1).gameObject);
		Destroy (this);
		return;
	}

}
