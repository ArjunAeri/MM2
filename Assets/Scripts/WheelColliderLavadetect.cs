using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WheelColliderLavadetect : MonoBehaviour {

	public GameObject mcamera;
	public GameObject lavaExplosion;

	private Rigidbody rrb;
	private RoverDamageManager rdm;
	// Use this for initialization
	void Start () 
	{
		rrb = GetComponent<Rigidbody> ();
		rdm = GetComponent<RoverDamageManager> ();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void deathbylava()
	{
		rrb.useGravity = false;
		rrb.constraints = RigidbodyConstraints.FreezeAll;
		print ("collider in lava");

		GetComponent<RoverDamageManager> ().killLava (this.gameObject);
		/*mcamera.transform.localPosition= new Vector3 (-3.79f, 1.38f, .95f);
		mcamera.transform.localEulerAngles = new Vector3 (30, 90, 8);
		Instantiate (lavaExplosion, transform).GetComponent<ParticleSystem> ().Play ();
		//StartCoroutine (spinToDoom());*/
	}

	private IEnumerator spinToDoom()
	{
		GetComponent<WheelCollider> ().motorTorque = -400;
		yield return new WaitForSeconds (2);
		GetComponent<WheelCollider> ().motorTorque = 0;
		yield return new WaitForSeconds (1.5f);
		GetComponent<WheelCollider> ().motorTorque = 400;

		Instantiate (lavaExplosion, transform).GetComponent<ParticleSystem> ().Play ();
	}
}
