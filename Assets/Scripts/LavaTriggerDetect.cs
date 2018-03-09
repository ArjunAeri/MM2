using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LavaTriggerDetect : MonoBehaviour 
{
	public float roundTimeInSeconds;

	private float movementperFrame;
	private Vector3 lavamovement;

	private static bool rfinished = false;
	// Use this for initialization
	void Start () 
	{
		movementperFrame = 14 / roundTimeInSeconds * Time.deltaTime;
		lavamovement = new Vector3 (0, movementperFrame, 0);
	}
	
	// Update is called once per frame
	void Update () 
	{
		if (!WheelColliderController.inputstarted || rfinished)
			return;
		
		if (transform.position.y < 71)
			transform.position += lavamovement;
		else
			print ("Apocalypse");
			//end of game
	}

	void OnTriggerEnter(Collider c)
	{
		print ("Lava collde w/ rover");
		if(c.gameObject.CompareTag("Rover"))
			c.gameObject.GetComponent<WheelColliderLavadetect> ().deathbylava ();
	}
		
	public static void finishRound()
	{
		rfinished = true;
	}
}
