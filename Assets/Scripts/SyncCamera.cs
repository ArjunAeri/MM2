using UnityEngine;
using System.Collections;

public class SyncCamera : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () 
	{
		if (WheelColliderController.skip)
			Destroy (this);
	}
}
