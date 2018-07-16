using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum Mode {
	FOLLOW,
	FIRST,
	MENU,
	SPLINE,
	OTHER
}


public class CameraController : MonoBehaviour
{
	GameObject cameraTarget;
	public float rotateSpeed = 80f;
	float rotate;
	float zoom;
	public float offsetDistance = 7f;
	public float offsetHeight = 7f;
	public float offsetSplineDistance = 7f;
	public float offsetSplineHeight = 3f;
	public float smoothing = 1f;
	public float smoothingSpline = 4f;
	public Vector3 offset;
	bool following = true;
	Vector3 lastPosition;
	public Mode mode = Mode.FOLLOW;
	Quaternion lastRotation;
	public Vector3 lastCorner;
	public Vector3 newCorner;
	public Vector3 nextCorner;
	public GameObject cameraRail;
	public List<GameObject> rails;
	public List<Vector3> railPoints;
	public Quaternion perpendicular;

	void Start()
	{
		cameraRail = GameObject.Find ("CameraRail");
		rails = cameraRail.GetComponent<CameraSplineScript> ().corners;
		railPoints = cameraRail.GetComponent<CameraSplineScript> ().points;
		cameraTarget = GameObject.FindGameObjectWithTag("Player").transform.GetChild(5).gameObject;
		lastPosition = new Vector3(cameraTarget.transform.position.x, cameraTarget.transform.position.y + offsetHeight, cameraTarget.transform.position.z - offsetDistance);
		offset = new Vector3(cameraTarget.transform.position.x, cameraTarget.transform.position.y + offsetHeight, cameraTarget.transform.position.z - offsetDistance);
	}

	void Update()
	{
		zoom = Input.GetAxisRaw ("ZoomCamera");
		switch (mode) {
		case Mode.FOLLOW: 
			rotate = Input.GetAxisRaw ("RotateCamera");

			offset = transform.position - cameraTarget.transform.position;
			offset = Quaternion.AngleAxis (rotate * rotateSpeed, Vector3.up) * offset;
			offset.y = 0f;
			transform.position = cameraTarget.transform.position + offset; 
			offset.Normalize ();


			transform.position = new Vector3 (Mathf.Lerp (lastPosition.x, cameraTarget.transform.position.x + (offset.x * offsetDistance), smoothing * Time.deltaTime), 
				Mathf.Lerp (lastPosition.y, cameraTarget.transform.position.y + offsetHeight, smoothing * Time.deltaTime * 3), 
				Mathf.Lerp (lastPosition.z, cameraTarget.transform.position.z + (offset.z * offsetDistance), smoothing * Time.deltaTime));
			transform.LookAt (cameraTarget.transform.position);
			break;
		case Mode.SPLINE:
			if (newCorner != null)
			{
				if (newCorner != lastCorner)
				{
					lastCorner = newCorner;
					nextCorner = findNext();
					//cameraTarget.transform.position = newCorner;
					//new Vector3 (lastRotation.x, , 0f);
					//transform.RotateAround(cameraTarget.transform.position, Vector3.up, 4f);
				}
			}
			double xdiff = (nextCorner.x - newCorner.x);
			double zdiff = (nextCorner.z - newCorner.z);
			double pxdiff = (cameraTarget.transform.position.x - newCorner.x);
			double pzdiff = (cameraTarget.transform.position.z - newCorner.z);
			double angle = System.Math.Atan((nextCorner.x - newCorner.x)/(nextCorner.z - newCorner.z));
			double prop = System.Math.Sqrt(System.Math.Pow(pxdiff,2) + System.Math.Pow(pzdiff, 2))/ System.Math.Sqrt(System.Math.Pow(xdiff, 2) + System.Math.Pow(zdiff, 2));
			cameraTarget.transform.LookAt(nextCorner);
			transform.LookAt(new Vector3(newCorner.x + (float)(prop * xdiff), newCorner.y, newCorner.z + (float)(prop * zdiff)));
			//transform.position = new Vector3(Mathf.Lerp(lastPosition.x, newCorner.x + offsetHeight * (float)System.Math.Cos(angle) + (float)pxdiff, smoothing * Time.deltaTime),
			//Mathf.Lerp(lastPosition.y, newCorner.y + offsetHeight, smoothing * Time.deltaTime * 3),
			//Mathf.Lerp(lastPosition.z, newCorner.z + offsetHeight * (float)System.Math.Sin(angle) + (float)pzdiff, smoothing * Time.deltaTime));
			transform.position = new Vector3(Mathf.Lerp(lastPosition.x, cameraTarget.transform.position.x + offsetHeight * (float)System.Math.Cos(angle), smoothing * Time.deltaTime),
				Mathf.Lerp(lastPosition.y, cameraTarget.transform.position.y + offsetHeight, smoothing * Time.deltaTime * 3),
				Mathf.Lerp(lastPosition.z, cameraTarget.transform.position.z + offsetHeight * (float)System.Math.Sin(angle), smoothing * Time.deltaTime));
			Debug.Log(prop + " " + xdiff + " " + zdiff + " " + cameraTarget.transform.position.x + " " + cameraTarget.transform.position.z + " " + newCorner.x + " " + newCorner.z + " " + nextCorner.x + " " + nextCorner.z + " " + lastPosition.x + " " + lastPosition.z);




			/* attempt 2

			if (newCorner != null) {
				if (newCorner != lastCorner) {
					lastCorner = newCorner;
					nextCorner = findNext ();
					//new Vector3 (lastRotation.x, , 0f);
					//transform.RotateAround(cameraTarget.transform.position, Vector3.up, 4f);
				}
			}
			Debug.DrawRay (transform.position, (nextCorner - lastCorner), Color.red);

			if((transform.rotation.eulerAngles - (nextCorner - lastCorner)).magnitude > 3f) {
							transform.RotateAround(cameraTarget.transform.position, Vector3.up, 4f);
						}
							
			//transform.LookAt (cameraTarget.transform.position);


			//offset = transform.position - cameraTarget.transform.position;

			//Quaternion q = Quaternion.RotateTowards (transform.rotation, Quaternion.AngleAxis (Vector3.Angle (nextCorner, lastCorner), Vector3.up), 1f);
			//offset = q * offset;
			//offset.Normalize ();
		//	transform.position = new Vector3 (Mathf.Lerp (lastPosition.x, cameraTarget.transform.position.x + offsetDistance, smoothing * Time.deltaTime), 
		//		Mathf.Lerp (lastPosition.y, cameraTarget.transform.position.y + offsetHeight, smoothing * Time.deltaTime * 3), 
		//		Mathf.Lerp (lastPosition.z, cameraTarget.transform.position.z + offsetDistance, smoothing * Time.deltaTime));
			
						//transform.position.y = Mathf.Lerp (lastPosition.y, cameraTarget.transform.position.y + offsetHeight, smoothing * Time.deltaTime * 3);

						if(transform.position.x - cameraTarget.transform.position.x > 0f && transform.position.z - cameraTarget.transform.position.z > 0f)  {
							
						}







			/* attempt 1


			Debug.DrawRay (transform.position, (lastCorner - nextCorner), Color.red);
			//perpendicular = Quaternion.Euler (0f, Vector3.Angle (nextCorner, lastCorner), 0f);

			//perpendicular = Quaternion.AngleAxis (Vector3.Angle (nextCorner, lastCorner) - 180f, Vector3.up);



			transform.position = new Vector3 (Mathf.Lerp (lastPosition.x, cameraTarget.transform.position.x + (offset.x * offsetSplineDistance), smoothingSpline * Time.deltaTime), 
				Mathf.Lerp (lastPosition.y, cameraTarget.transform.position.y + offsetSplineHeight, smoothingSpline * Time.deltaTime * 3), 
				Mathf.Lerp (lastPosition.z, cameraTarget.transform.position.z + (offset.z * offsetSplineDistance), smoothingSpline * Time.deltaTime));

			//transform.rotation = Quaternion.Slerp (lastRotation, perpendicular, Time.deltaTime * rotateSpeed);
			lastRotation = transform.rotation;
			//transform.LookAt (cameraTarget.transform.position); */
			break;
		case Mode.FIRST: // for intro cutscene to display full level.
			break;
		default:
			break;
		}
	}


	


	void LateUpdate()
	{
		lastPosition = transform.position;

	}

	private Vector3 findNext () 
	{
		Vector3 back;	
		Vector3 forward;
		int i = railPoints.IndexOf (newCorner);
		if (i > 0) {
			back = (rails.ToArray () [i - 1].transform.position - lastCorner).normalized;
		} else {
			return rails.ToArray () [i + 1].transform.position;
		}
		if (i < railPoints.Count - 1) {
			forward = (rails.ToArray () [i + 1].transform.position - lastCorner).normalized;
		} else {
			return rails.ToArray () [i - 1].transform.position;
		}
		if ((cameraTarget.transform.position - (lastCorner + forward)).magnitude < (cameraTarget.transform.position - (lastCorner + back)).magnitude) {
			return rails.ToArray () [i - 1].transform.position;
		} else {
			return rails.ToArray () [i + 1].transform.position;
		}
	}
}