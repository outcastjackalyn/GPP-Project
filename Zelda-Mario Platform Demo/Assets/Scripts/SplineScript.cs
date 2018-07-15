using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum SplineType{TERRAIN, FOLLOW, OTHER};

public struct Spline {

	public Vector3[] points;
	public SplineType type;

	public Spline(SplineType type, Vector3[] points) {
		this.type = type;
		this.points = points;
	}
	public void AddPoint(Vector3 point) {
		points.SetValue (point, points.Length);
	}
	public void SetType(SplineType type) {
		this.type = type;
	}
}


public class SplineScript : MonoBehaviour {


	public Material m1;
	public Material m2;
	public bool color;

	public Vector3[] points;
	public Vector3 lastPosition;
	public Quaternion lastRotation;
	public Vector3 currentPosition;
	public Quaternion currentRotation;
	public GameObject rock;
	GameObject tracer;
	public float interval = 0.5f;
	// Use this for initialization
	void Start () {
		tracer = GameObject.Find ("Tracer");
		if (points [0].Equals (null)) {
			points [0] = Vector3.zero;
		}
		lastPosition = points [0];
		StartCoroutine (path ());
	}
	
	// Update is called once per frame
	void Update () {
		
	}


	public IEnumerator path() {
		tracer.transform.position = points [0];
		lastPosition = tracer.transform.position;
		lastRotation = tracer.transform.rotation;
		for (int i = 1; i < points.Length; i++) {
			int count = 0;
			Vector3 dir = points [i] - tracer.transform.position;
			while ((points [i] - tracer.transform.position).magnitude > 1) {
				tracer.transform.position += dir.normalized * Time.deltaTime;
				currentPosition = tracer.transform.position;
				if ((points [i] - tracer.transform.position).magnitude < 4 && count > 0) {
					tracer.transform.rotation = Quaternion.Slerp(tracer.transform.rotation, Quaternion.FromToRotation (Vector3.forward, points [i + 1] - tracer.transform.position), .6f);
				} else {
					tracer.transform.rotation = Quaternion.Slerp(tracer.transform.rotation, Quaternion.FromToRotation (Vector3.forward, dir), .6f);
				}
				//tracer.transform.rotation.;
				currentRotation = tracer.transform.rotation;
				if((lastPosition - currentPosition).magnitude > 3) {
					count++;
					GameObject go = Instantiate (rock);
					go.transform.rotation = tracer.transform.rotation;
					go.transform.position = tracer.transform.position;
					if (color) {
						go.GetComponent<MeshRenderer> ().material = m1;
						go.transform.GetChild (0).GetComponent<MeshRenderer> ().material = m1;
						//go.transform.GetChild (1).GetComponent<MeshRenderer> ().material = m1;
					} else {
						go.transform.position += new Vector3(0f,0.001f,0f);
						go.GetComponent<MeshRenderer> ().material = m2;
						go.transform.GetChild (0).GetComponent<MeshRenderer> ().material = m2;
						//go.transform.GetChild (1).GetComponent<MeshRenderer> ().material = m2;
					}
					color = !color;
					lastPosition = currentPosition;
					lastRotation = currentRotation;
				}
				dir = points [i] - tracer.transform.position;
			}

		}




		yield return null;
	}


}
