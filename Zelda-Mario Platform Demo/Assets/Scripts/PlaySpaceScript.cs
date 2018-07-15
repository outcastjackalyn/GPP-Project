using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlaySpaceScript : MonoBehaviour {

	private GameObject Player;

	// Use this for initialization
	void Start () {
		Player = GameObject.Find("Player");
	}
	
	// Update is called once per frame
	void Update () {
		

	}

	void OnTriggerExit(Collider c){
		
		//if gameobject leaves play space then delete it, unless player then restart scene
		if (c.gameObject == Player) {
			string scene = SceneManager.GetActiveScene ().name;
			//SceneManager.UnloadSceneAsync (scene);
			SceneManager.LoadSceneAsync(scene);
		} else {
			Destroy (c.gameObject);
		}

	}
}
