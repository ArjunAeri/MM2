using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Collections.ObjectModel;

public class terrainDesignator : MonoBehaviour {

	private Mesh mtm;
	private Vector3 deltaLs;

	public float difficulty;

	// Use this for initialization
	void Start () {
		mtm = GetComponent<MeshFilter>().mesh;
		deltaLs = new Vector3 (-.04f, -.04f, -.04f);
	}
	
	// Update is called once per frame
	void Update () {
		transform.localScale += deltaLs;

		Vector3 [] verts = mtm.vertices;

		for (int i = 0; i < verts.Length; i++) {

		}
	}
}
