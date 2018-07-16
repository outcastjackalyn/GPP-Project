using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraSplineScript : SplineScript {

	public List<GameObject> sections;
	public List<GameObject> corners;
	public GameObject cornerObject;
	List<TerrainSplineScript> splines = new List<TerrainSplineScript>();
	// Use this for initialization
	void Start () {
		foreach (GameObject go in sections) {
			splines.Add (go.GetComponent<TerrainSplineScript> ());
		}
		StartCoroutine (_DrawOnLoad());
	}
	
	// Update is called once per frame
	void Update () {
		
	}



	public IEnumerator _DrawOnLoad() {
		foreach (TerrainSplineScript script in splines) {
			StartCoroutine (script._Path (this));
		}
		int i = 0;
		foreach (GameObject go in corners) {
			if (i == 0 || i == corners.Count - 1) {
				go.tag = "RailEnd";
			}
			i++;
		}
		yield return null;
	}


	public void addCorner (Vector3 corner) {
		corner += new Vector3 (0f, 5f, 0f); //offset above platforms
		GameObject newCorner = Instantiate (cornerObject);
		newCorner.transform.position = corner;
		newCorner.transform.parent = this.transform;
		corners.Add (newCorner);
		this.points.Add(corner);
		//instantiate the corner collider here
	}
}
