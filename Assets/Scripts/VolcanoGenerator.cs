using UnityEngine;
using System.Collections;

public class VolcanoGenerator : MonoBehaviour {

	public int numvolcanoes;
    public ParticleSystem volcano;
	public GameObject volcanobase;
	public GameObject weirdrock;
	public GameObject rectrock;
	public ParticleSystem sandstorm;
    private int terrainlength=-2560;
    private int terrainwidth=-1280;
    private Vector2 terrainpos= new Vector2(5120,2560);
	void Start () 
	{
		for (int i = 1; i <= numvolcanoes; i++) {
			Vector3 curpos = new Vector3 (Random.Range (-2560, 2560), 0, Random.Range (-1280, 1280));
			Instantiate (volcanobase, curpos,Quaternion.identity);
			curpos.y += 10;
			Instantiate (volcano,curpos,Quaternion.Euler(new Vector3(270,90,270)));	
		}
		for (int i = 1; i <=Random.Range(numvolcanoes*3,numvolcanoes*9); i++) {
			Vector3 curpos = new Vector3 (Random.Range (-2560, 2560), 0, Random.Range (-1280, 1280));
			Instantiate (weirdrock, curpos,Quaternion.identity);
		}
		for (int i = 1; i <= Random.Range(numvolcanoes*3,numvolcanoes*9); i++) {
			Vector3 curpos = new Vector3 (Random.Range (-2560, 2560), 0, Random.Range (-1280, 1280));
			Instantiate (rectrock, curpos,Quaternion.identity);
		}
		for (int i = 1; i <= (int)(Random.Range(numvolcanoes/4,numvolcanoes/2)); i++) {
			Vector3 curpos = new Vector3 (Random.Range (-2560, 2560), 200, Random.Range (-1280,1280));
			Instantiate (sandstorm, curpos, Quaternion.Euler(new Vector3(160,90,270)));
			sandstorm.Play ();
		}
	}

}
