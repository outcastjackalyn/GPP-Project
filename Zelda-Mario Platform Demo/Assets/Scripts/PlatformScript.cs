using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformScript : MonoBehaviour {

	public Vector3[] location;
	public int currentDest = 0;
	public float speed = 3f;
	public GameObject Player;

	// Use this for initialization
	void Start () {
		Player = GameObject.Find("Player");
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		if (location.Length >= 0) {
			this.GetComponent<Rigidbody>().MovePosition (Vector3.MoveTowards (transform.position, location [currentDest], speed * Time.deltaTime));
			if(Player.transform.IsChildOf(transform)){
				Vector3 flatVelocity = this.GetComponent<Rigidbody>().velocity;
				flatVelocity.y = 0f;
				Player.GetComponent<Rigidbody> ().velocity += flatVelocity;
			}
		}
		if (transform.position == location [currentDest]) {
			if (currentDest + 1 == location.Length) {
				currentDest = 0;
			} else {
				currentDest++;
			}
		}
	}
}
