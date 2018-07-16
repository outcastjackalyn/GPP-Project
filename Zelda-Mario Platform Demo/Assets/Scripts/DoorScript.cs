using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorScript : MonoBehaviour {

    public float openAngle = -160.0f;
    public float closedAngle = 0.0f;
    public float doorAnimSpeed = 2.0f;

    //private Transform playerTrans = null;
    public bool doorStatus = false; //false is close, true is open
    private bool doorMoving = false; //for Coroutine, when start only one
    public bool buttonGo = false; 

	void Start () {
        doorStatus = false;
        buttonGo = false;
    }
	
	// Update is called once per frame
	void Update () {
        if (buttonGo && !doorMoving)
        {
            if (doorStatus)
            { //close door
                StartCoroutine(this._MoveDoor(Quaternion.Euler(0, closedAngle, 0)));
            }
            else { //open door
                StartCoroutine(this._MoveDoor(Quaternion.Euler(0, openAngle, 0)));
            }
        }
    }

    public IEnumerator _MoveDoor(Quaternion dest)
    {
        doorMoving = true;
        //Check if close/open, if angle less 4 degree, or use another value more 0
        while (Quaternion.Angle(transform.localRotation, dest) > 0)
        {
            transform.localRotation = Quaternion.Slerp(transform.localRotation, dest, Time.deltaTime * doorAnimSpeed);
            //UPDATE 1: add yield
            yield return null;
        }
        //Change door status
        doorStatus = !doorStatus;
        doorMoving = false;
        buttonGo = false;
        //UPDATE 1: add yield
        yield return null;
    }
}
