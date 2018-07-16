using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainSplineScript : SplineScript {
	
	public Material m1;
	public Material m2;
	public bool color;
	public GameObject rock;
	GameObject tracer;
	public float interval = 3f;

	Vector3 lastPosition;
	Quaternion lastRotation;
	Vector3 currentPosition;
	Quaternion currentRotation;


	// Use this for initialization
	void Start () {
		tracer = GameObject.Find ("Tracer");
		if (points [0].Equals (null)) {
			points [0] = Vector3.zero;
		}
		lastPosition = points [0];
		//StartCoroutine (_Path ()); // call this from camera rail i think
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}


	public IEnumerator _Path(CameraSplineScript cameraRail) {
		tracer.transform.position = points [0];
		cameraRail.addCorner (points [0]);
		lastPosition = tracer.transform.position;
		lastRotation = tracer.transform.rotation;
		for (int i = 1; i < points.Count; i++) {
			cameraRail.addCorner (points [i]);
			tracer.transform.position = points [i - 1];
			lastPosition = tracer.transform.position;
			//lastRotation = tracer.transform.rotation;
			int count = 0;
			Vector3 dir = points [i] - tracer.transform.position;
			while ((points [i] - tracer.transform.position).magnitude > 2f) {
				tracer.transform.position += dir.normalized * Time.deltaTime;
				currentPosition = tracer.transform.position;
				if ((points [i] - tracer.transform.position).magnitude < interval && count > 0) {
					tracer.transform.rotation = Quaternion.Slerp (tracer.transform.rotation, Quaternion.FromToRotation (Vector3.forward, points [i + 1] - tracer.transform.position), .6f);
				} else if (count == 0) {
					tracer.transform.rotation = Quaternion.FromToRotation (Vector3.forward, dir);
				} else {
					tracer.transform.rotation = Quaternion.Slerp(tracer.transform.rotation, Quaternion.FromToRotation (Vector3.forward, dir), .6f);
				}
				//tracer.transform.rotation.;
				currentRotation = tracer.transform.rotation;
				if((lastPosition - currentPosition).magnitude > interval || count == 0) {
					count++;
					if (type == SplineType.TERRAIN) {
						//place path into world
						GameObject go = Instantiate (rock);
						go.transform.rotation = tracer.transform.rotation;
						go.transform.position = tracer.transform.position;
						go.transform.parent = this.transform;
						if (color) {
							go.GetComponent<MeshRenderer> ().material = m2;
							go.transform.GetChild (0).GetComponent<MeshRenderer> ().material = m2;
							//go.transform.GetChild (1).GetComponent<MeshRenderer> ().material = m1;
						} else {
							Vector3 displayOffset = new Vector3 (0f, 0.001f, 0f);
							go.transform.position += displayOffset;
							go.GetComponent<BoxCollider> ().center -= displayOffset;
							go.GetComponent<MeshRenderer> ().material = m1;
							go.transform.GetChild (0).GetComponent<MeshRenderer> ().material = m1;
							//go.transform.GetChild (1).GetComponent<MeshRenderer> ().material = m2;
						}
						color = !color;
					} else {
						// for sections of a 2.5d sequence not using the generated path
					}
					lastPosition = currentPosition;
					lastRotation = currentRotation;
				}
				dir = points [i] - tracer.transform.position;
			}
		}
		yield return null;
	}

}
