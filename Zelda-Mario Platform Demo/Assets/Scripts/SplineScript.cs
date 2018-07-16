using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum SplineType{TERRAIN, PATROL, CAMERA, OTHER}; // was going to set up moving platforms to use this

public struct Spline {

	public List<Vector3> points;
	public SplineType type;

	public Spline(SplineType type, List<Vector3> points) {
		this.type = type;
		this.points = points;
	}
	public void AddPoint(Vector3 point) {
		this.points.Add(point);
	}
	public void SetType(SplineType type) {
		this.type = type;
	}
}


public class SplineScript : MonoBehaviour {

	public SplineType type;
	public List<Vector3> points = new List<Vector3>();
	public Spline spline;
	// Use this for initialization
	void Start () {
		spline = new Spline (type, points);
		//cameraRail = GameObject.Find ("Camera Rail");
	}
	
	// Update is called once per frame
	void Update () {
		
	}




}
