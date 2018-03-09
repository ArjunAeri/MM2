using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class AnimationEventScheduler : NetworkBehaviour {

	public Camera maincamera;

	public Camera animationcamera;
	public Light camerasource;
	public Camera blackout;
	public float seconds;
	private Animator intro;
	private int test;
	private GameObject claw;
	private WheelColliderController mywcc;
	private RigidbodyConstraints gameplay;
	private Rigidbody rrb;
	private AudioSource introMusic;
	// Use this for initializationd
	void Start ()
	{
		if (!isLocalPlayer)
			return;

		camerasource.enabled = false;
		
		rrb = GetComponent<Rigidbody> ();
		intro = GetComponentInChildren<Animator> ();

		gameplay = rrb.constraints;
		rrb.constraints = RigidbodyConstraints.FreezeAll;

		print ("Freezing Rover " + rrb.constraints.ToString ());

		introMusic = animationcamera.GetComponent<AudioSource> ();
		introMusic.Play ();
		intro.speed = 1.2f;
	}

	public void ay()
	{
		if (!isLocalPlayer)
			return;
		
		print ("ay");
		CameraMovement.moveback = false;
		animationcamera.transform.localPosition = new Vector3 (.5f, 1.8f, -.5f);
		animationcamera.transform.localRotation = Quaternion.Euler (90, 270, 0);
		StartCoroutine (blackoutpause (seconds));
		CameraMovement.movement = new Vector3 (0, .45f, 0);
		CameraMovement.moveback = true;
	}

	public void MoveAnimation()
	{
		if (!isLocalPlayer)
			return;
		
		print ("ay2"); //Debug log

		blackout.enabled = true;
		animationcamera.enabled = false;

		animationcamera.transform.localPosition = new Vector3(5, 1.8f, 0);
		animationcamera.transform.rotation = Quaternion.Euler(new Vector3 (30, 270, 60));

		StartCoroutine (blackoutpause(seconds));
	}

	IEnumerator blackoutpause (float seconds)
	{
		if (!isLocalPlayer)
			yield break;
		
		yield return new WaitForSeconds(1.2f);

		print ("trying to turn camera back on");

		animationcamera.enabled = true;
		blackout.enabled = false;
	}

	public void thisistheend()
	{
		if (!isLocalPlayer)
			return;

		rrb.constraints = gameplay;
		
		print ("end");
		CameraMovement.end = true;
		GetComponentInChildren<Camera> ().enabled = true;
		blackout.enabled = false;
		enabled = false;
		introMusic.Stop();
		introMusic.enabled = false;
		print ("Trying to stop intro music m8");
		GetComponentInChildren<Animator> ().enabled = false;
		WheelColliderController.inputstarted = true;

		Cmdspawnclaw (GetComponent<NetworkIdentity>());

		camerasource.enabled = false;

	
	}

	public void ExecuteSpoiler ()
	{
		if (!isLocalPlayer)
			return;
		
		print ("Tryna Execute Spoiler");
		CameraMovement.moveback = false;

		StartCoroutine(continueSpoiler());

		animationcamera.transform.localPosition = new Vector3 (0, -.75f, -3);
		animationcamera.transform.localRotation = Quaternion.Euler (-30, 0, 0);
	}

	IEnumerator continueSpoiler()
	{
		if (!isLocalPlayer)
			yield break;
		
		blackout.enabled = true;
		animationcamera.enabled = false;

		yield return new WaitForSecondsRealtime (.7f);


		animationcamera.enabled = true;
		blackout.enabled = false;

		camerasource.enabled = true;
	}

	[Command]
	void Cmdspawnclaw(NetworkIdentity p)
	{
		GameObject claw= (GameObject) Resources.Load ("Prefabs/Roverclaw");
		claw.transform.position = transform.Find ("ClawSpawn").position;
		claw.transform.localScale = new Vector3(.5f, .5f, .5f);
		claw.transform.localEulerAngles = new Vector3 (0, 180, 0);

		claw = (GameObject)Instantiate (claw);
		NetworkServer.SpawnWithClientAuthority(claw, p.connectionToClient);

		Rpcassignrover (p, claw.GetComponent<NetworkIdentity> ());
	}		

	[ClientRpc]
	void Rpcassignrover(NetworkIdentity p, NetworkIdentity c)
	{
		print ("Executing assign rover now");
		c.gameObject.GetComponent<ClawControl> ().roverid = p;
		p.gameObject.GetComponent<RoverDamageManager> ().clawId = c;
	}
}
