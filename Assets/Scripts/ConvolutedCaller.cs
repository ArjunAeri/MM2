using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConvolutedCaller : MonoBehaviour {

	AnimationEventScheduler animationevent;
	// Use this for initialization
	void Start () 
	{
		animationevent = this.GetComponentInParent<AnimationEventScheduler> ();
	}

	void ay()
	{
		animationevent.ay ();
	}

	void MoveAnimation()
	{
		animationevent.MoveAnimation();
	}

	public void thisistheend()
	{
		animationevent.thisistheend();
	}

	void ExecuteSpoiler ()
	{
		animationevent.ExecuteSpoiler();
	}

	public void callthisistheend()
	{
		animationevent.thisistheend ();
	}
}
