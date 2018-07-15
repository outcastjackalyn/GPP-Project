using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GearScript : MonoBehaviour {

	public float angleIncrement = 90f;
	public float turnSpeed = 15f;

	public bool isturning = false;
	public float waitTimer = 1;
	public float timer = 0f;
	public GameObject Player;
	public Vector3 angularVelocity;
	public Vector3 flatDisplacement;
	public Vector3 deltaVelocity;
	public Rigidbody rb;

	// Use this for initialization
	void Start () {
		rb = this.GetComponent<Rigidbody> ();
		isturning = false;
		Player = GameObject.Find("Player");
	}
	
	// Update is called once per frame
	void Update () {
		if (!isturning) {
			if (timer < waitTimer) {
				timer += Time.deltaTime;
			} else {
				timer = 0f;
				StartCoroutine (this.moveGear ());
			}
		}
	}


	void FixedUpdate () {
		
		if(Player.transform.IsChildOf(transform)) {
			angularVelocity = rb.angularVelocity;
			flatDisplacement = Player.transform.position - transform.position;
			flatDisplacement.y = 0f;
			deltaVelocity = Vector3.Cross (angularVelocity, flatDisplacement);
			Player.GetComponent<Rigidbody> ().velocity += deltaVelocity;
		}
	}

	public IEnumerator moveGear()
	{
		isturning = true;

		Quaternion nextAngle = transform.localRotation;
		nextAngle *= Quaternion.Euler(new Vector3(0f, angleIncrement, 0f));
			
		while (Quaternion.Angle(transform.localRotation, nextAngle) > 0)
		{
			rb.MoveRotation (rb.rotation * Quaternion.Euler (new Vector3 (0f, Time.deltaTime * turnSpeed, 0f)));
			if (Player.transform.IsChildOf (transform)) {
				Player.GetComponent<Rigidbody> ().MoveRotation (Player.GetComponent<Rigidbody> ().rotation * Quaternion.Euler (new Vector3 (0f, Time.deltaTime * turnSpeed, 0f)));
			}
			yield return null;
		}
		isturning = false;
		//Change door status
		yield return true;
	}
}
