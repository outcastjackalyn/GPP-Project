using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PatrolState{ PATROLLING, AGGRO, POINT, NONE }

public class EnemyPatrolScript : MonoBehaviour {

	public PatrolState state = PatrolState.PATROLLING;
	//public Transform next;

	public Vector3 end1 = new Vector3 (-10f, 2f, 0f);
	public Vector3 end2 = new Vector3 (-20f, 2f, 0f);

	public GameObject tracking;
	public GameObject next;

	public Spline spline;


	// Use this for initialization
	void Start () {
		next = Instantiate (tracking);
		spline = new Spline(SplineType.PATROL, new List<Vector3>());
		spline.points.Add (end1);
		spline.points.Add (end2);
		next.transform.position = end1;
	}
	
	// Update is called once per frame
	void Update () {
		if ((Vector3.Cross((this.gameObject.transform.position - next.transform.position), new Vector3(1f, 0f, 1f))).magnitude < 0.6f) {
			foreach (Vector3 v in spline.points) {
				if(next.transform.position.Equals(v)) {
					foreach (Vector3 w in spline.points) {
						if(!next.transform.position.Equals(w)) {
							next.transform.position = w;
						}
					}
				}
			}
		}
	}
}
