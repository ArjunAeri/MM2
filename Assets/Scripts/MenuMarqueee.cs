using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuMarqueee : MonoBehaviour 
{
	public Text title;
	public static Vector3 moveDirect;
	public static bool inUI = true;

	private Camera c;

	// Use this for initialization
	void Start () 
	{
		c = GetComponent<Camera> ();
		c.enabled = false;
		c.enabled = true;
		moveDirect = new Vector3 (-10, 0, 0) * Time.deltaTime;
	}
	
	// Update is called once per frame
	void Update () 
	{
		this.transform.Translate (moveDirect);

	}
}
