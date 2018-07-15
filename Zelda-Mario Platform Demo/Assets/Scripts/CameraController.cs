using UnityEngine;
using System.Collections;

public enum Mode {
	FOLLOW,
	FIRST,
	SPLINE
}


public class CameraController : MonoBehaviour
{
	GameObject cameraTarget;
	public float rotateSpeed = 80f;
	float rotate;
	public float offsetDistance = 7f;
	public float offsetHeight = 4f;
	public float smoothing = 1f;
	public Vector3 offset;
	bool following = true;
	Vector3 lastPosition;
	public Mode mode = Mode.FOLLOW;

	void Start()
	{
		cameraTarget = GameObject.FindGameObjectWithTag("Player").transform.GetChild(5).gameObject;
		lastPosition = new Vector3(cameraTarget.transform.position.x, cameraTarget.transform.position.y + offsetHeight, cameraTarget.transform.position.z - offsetDistance);
		offset = new Vector3(cameraTarget.transform.position.x, cameraTarget.transform.position.y + offsetHeight, cameraTarget.transform.position.z - offsetDistance);
	}

	void Update()
	{
		switch (mode) {
		case Mode.FOLLOW: 
			if (Input.GetKey (KeyCode.Q)) {
				rotate = -1;
			} else if (Input.GetKey (KeyCode.E)) {
				rotate = 1;
			} else {
				rotate = 0;
			}
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
		case Mode.FIRST:
			break;
		case Mode.SPLINE:
			break;
		default:
			break;
		}
	}

	void LateUpdate()
	{
		lastPosition = transform.position;
	}
}