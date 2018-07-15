using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameData : MonoBehaviour {

	public Dictionary <string, bool> map = new Dictionary<string,bool> ();
	/*public bool a;
	public bool b;
	public bool c;*/


	// Use this for initialization
	void Start () {
		if (GameObject.FindGameObjectsWithTag ("GameController").Length > 1) {
			Destroy (this.gameObject);
		}
		map.Add ("Level1", false);
		map.Add ("Level2", false);
		map.Add ("Level3", false);
		DontDestroyOnLoad (gameObject);
	}

	// Update is called once per frame
	void Update () {
		//map.TryGetValue ("Level1", out a);
	}



	public void toggleMapValue(string key) {
		bool value; 
		map.TryGetValue (key, out value);
		map.Remove (key);
		map.Add (key, !value);
	}
}
