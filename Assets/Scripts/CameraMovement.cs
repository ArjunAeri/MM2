 using UnityEngine;
using System.Collections;

public class CameraMovement : MonoBehaviour
{
	public Camera gamecamera;
	public static bool skip = false;
	public Animator animationsequence;
	public GameObject rover;
	public Camera blackout;
	float timepassed; // Seconds used for timepassed as counter
	public static bool moveback = true;
	public static bool end = false;
	public static Vector3 movement;
	AudioSource animationmusic;
	// Use this for initialization
	void Start () 
	{
		movement = new Vector3 (0, -.5f, 0);
		blackout.enabled = false;
		gamecamera.enabled = false;
		enabled = true;
		animationmusic = GetComponent<AudioSource> (); //Gets AudioComponent
		animationmusic.Play (); // Plays background music
	}
	
	// Update is called once per frame
	void Update ()
	{
		if (moveback && !end)
		{
				transform.Translate (movement * Time.deltaTime / 2); //Moves .25 units backward every second
				timepassed += Time.deltaTime / 2;  //every half second
				
				if (Input.GetKeyDown(KeyCode.Space))
				{
					AnimationEventScheduler instance = new AnimationEventScheduler();
					instance.thisistheend ();
					WheelColliderController.skip = true;
					Destroy (this);
				}
		} 
	}


	IEnumerator RotateAndMoveIntoGame(bool done)
	{
		transform.rotation = Quaternion.Euler(new Vector3 (transform.rotation.x, 0, 0));

		while(!done)
		{
			transform.RotateAround (rover.transform.position, Vector3.up, Time.deltaTime/5);
			yield return null;

			if (Mathf.Abs (transform.position.x) > .1f)
				done = true;
		}

		gamecamera.enabled = true; // switches view from animation to main camera.
		blackout.enabled = false;
		enabled = false;
		rover.GetComponent<Animator> ().enabled = false;
		WheelColliderController.inputstarted = true;
		animationmusic.Stop ();
	}

	void Destroy()
	{
		Destroy (this);
	}
}
