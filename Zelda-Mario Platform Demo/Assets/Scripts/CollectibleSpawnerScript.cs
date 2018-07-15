using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//public enum CollectibleType {SPEED, JUMP};

public class CollectibleSpawnerScript : MonoBehaviour {

	//public CollectibleType type;
	public bool collectibleSpawned;
	public GameObject collectible;

	// Use this for initialization
	void Start () {
		StartCoroutine(this.spawn(0f));
		collectibleSpawned = true;
	}

	// Update is called once per frame
	void Update () {
		if(!collectibleSpawned){
			StartCoroutine(this.spawn(6f));
		}
	}

	public IEnumerator spawn(float spawnTimer)
	{
		collectibleSpawned = true;
		while (spawnTimer > 0f) {
			spawnTimer -= Time.deltaTime;
			yield return null;
		}
		GameObject newCollectible;
		newCollectible = Instantiate (collectible);
		newCollectible.GetComponent<Transform>().position = transform.position;
		newCollectible.SetActive (true);
		newCollectible.GetComponent<CollectibleScript> ().Spawner = this.gameObject;
		yield return null;
	}
}
