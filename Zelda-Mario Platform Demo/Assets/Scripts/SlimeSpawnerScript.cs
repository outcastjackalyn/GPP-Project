using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum SlimeType {BLUE, RED};
public enum SpawnType {REPEAT, SINGLE, INFINITE};

public struct SlimeData {
	public SlimeType type;
	public float moveSpeed;
	public int size;
	public int split;
	public float scale;

	public SlimeData(SlimeType type, int size){
		this.type = type;
		this.size = size;
		switch (type) {
		case SlimeType.BLUE: // is okay for up to maybe size 5?
			this.scale = 1.0f;
			this.moveSpeed = size > 1 ? 2f : 1.5f;
			this.split = size > 1 ? 2 : 0;
			break;
		case SlimeType.RED://best to have max size 2
			this.scale = size > 1 ? 2.0f : 0.75f;
			this.split = size > 1 ? 8 : 0;
			this.moveSpeed = size > 1 ? 1.4f : 2.3f;
			break;
		default:
			this.scale = 1.0f;
			this.moveSpeed = 1.0f;
			this.split = size > 1 ? 2 : 0;
			break;
		}
	}
};

public class SlimeSpawnerScript : MonoBehaviour {


	//public SlimeData data;
	public SlimeType type = SlimeType.BLUE;
	public PatrolState state = PatrolState.AGGRO; 
	public int size = 3;
	public GameObject slime;
	public GameObject spawnBody;
	public SpawnType spawnType = SpawnType.SINGLE;
	public float timer = 0;
	public float repeatTimer = 10f;
	public int repeat = -1;
	// Use this for initialization
	void Start () {
		
		//slime = GameObject.Find ("Slime");
		if (spawnType != SpawnType.SINGLE) {
			GameObject newBody;
			newBody = Instantiate (spawnBody);
			newBody.GetComponent<Transform> ().SetParent (this.transform);
			newBody.GetComponent<Transform> ().position = transform.position - new Vector3 (0f, 2.8f, 0f);
			this.gameObject.transform.GetChild (0).GetComponent<ParticleSystem> ().collision.SetPlane (0, GameObject.Find ("BouncePlane").transform);
		} else {
			repeat = 0;
			StartCoroutine(this.spawn(new SlimeData (type, size)));			
		}
	}
	
	// Update is called once per frame
	void Update () {
		timer -= Time.deltaTime;
		if (timer <= 0f) {
			StartCoroutine(this.spawn(new SlimeData (type, size)));		
			timer = repeatTimer;
			repeat--;
		}
	}



	public IEnumerator spawn(SlimeData data)
	{
		GameObject newSlime;
		newSlime = Instantiate (slime);
		newSlime.GetComponent<Transform>().position = transform.position;
		newSlime.GetComponent<EnemySlimeScript>().data = data;
		newSlime.GetComponent<Transform>().localScale = new Vector3 (data.size * data.scale, data.size * data.scale, data.size * data.scale);
		if (repeat == 0 && transform.root == transform) {
			Destroy (this.gameObject); // removes gameobject when it's finished spawning things
		}
		yield return null;
	}
}
