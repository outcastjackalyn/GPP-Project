using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Boost {SPEED, JUMP, KEY, NONE}

public class CollectibleScript : MonoBehaviour {

	private GameObject Player;
	public float rotateSpeed = 20f;
	public float moveDist = 0.15f;
	public float moveSpeed = 0.3f;

	public GameObject Spawner;

	public Boost boost = Boost.NONE;

	private Transform BodyTrans;
	bool up = true;

	// Use this for initialization
	void Start () {
		BodyTrans = this.transform.GetChild (0);
		Player = GameObject.Find("Player");
	}

	// Update is called once per frame
	void Update () {
		this.transform.GetChild(0).Rotate (new Vector3 (0f, Time.deltaTime * rotateSpeed, 0f));
		if (BodyTrans.localPosition.y < 1 + moveDist && up)
		{
			BodyTrans.localPosition += new Vector3(0f, (Time.deltaTime * moveSpeed), 0f);
		} else { up = false;}
		if (BodyTrans.localPosition.y > 1 - moveDist && !up)
		{
			BodyTrans.localPosition -= new Vector3(0f, (Time.deltaTime * moveSpeed), 0f);
		} else { up = true; }
	}

	void OnTriggerEnter(Collider c)
	{
		if (c.gameObject == Player)
		{
			if (boost == Boost.KEY) {
				this.transform.SetParent (Player.transform);
				this.transform.position = Player.transform.position + new Vector3 (0.0f, 2.35f, 0.0f);

			} else {
				Player.GetComponent<CharacterController> ().boost = boost;
				Player.GetComponent<CharacterController> ().boostTimer = Player.GetComponent<CharacterController> ().boostMax;
				if (Spawner != null) {
					Spawner.GetComponent<CollectibleSpawnerScript> ().collectibleSpawned = false;
				}
				Destroy (this.gameObject);
			}
		}
	}
}
