using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ExitScript : MonoBehaviour {

	public bool locked = true;
	public string targetScene = "Level1";
	ParticleSystem.MainModule main;
	public GameObject dataMan;

	// Use this for initialization
	void Start () {
		bool unlocked;
		dataMan = GameObject.Find ("GameData");
		if (dataMan.GetComponent<GameData> ().map.TryGetValue (SceneManager.GetActiveScene ().name, out unlocked)) {
			locked = !unlocked;
		}
		main = this.gameObject.transform.GetChild (0).GetComponentInChildren<ParticleSystem> ().main;
	}

	// Update is called once per frame
	void Update () {
		if (locked) {
			this.gameObject.transform.GetChild (0).GetComponent<MeshRenderer> ().material.SetColor ("_Color", new Color (0.9755f, 1.0f, 0.4481f, 1f));
			main.startColor = new Color (0.9755f, 1.0f, 0.4481f, 1f);
		} else {
			this.gameObject.transform.GetChild (0).GetComponent<MeshRenderer> ().material.SetColor ("_Color", new Color (0.6412f, 0.2133f, 0.9622f, 1.0f));	
			main.startColor = new Color (0.6412f, 0.2133f, 0.9622f, 1.0f);
		}
	}


}
