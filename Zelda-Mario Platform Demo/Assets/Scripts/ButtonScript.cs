using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonScript : MonoBehaviour {

    public float defaultXPosition = -0.75f;
    public float pressedXPosition = -0.99f;
    public float buttonAnimSpeed = 0.4f;
    public float clickPauseTime = 1.5f;
    public float punchPauseTime = 3f;
    
	public Transform BodyTrans;
    public GameObject Player;
	public Camera MainCamera;
	public Camera SceneCamera;
	public float CameraRotationSpeed = 0.1f;

    public bool cameraSide = false; //false is left, true is right
    public Vector3 cameraPosition;
    public Vector3 cameraRotation;

    public GameObject DoorObject;

    public bool buttonStatus = false; //false is default, true is pressed
    public bool playerInRange = false;

    // Use this for initialization
    void Start () {
        buttonStatus = false;
        playerInRange = false;
        BodyTrans = this.gameObject.transform.GetChild(0);
        SceneCamera = this.gameObject.transform.GetChild(1).gameObject.GetComponent<Camera>();


		if(MainCamera == null){
			MainCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
			if(MainCamera == null){
				Debug.LogError("ERROR: There is no camera in scene.");
				Destroy(this);
			}
		}

       // MainCamera = GameObject.Find("Main Camera").GetComponent<Camera>();

        if (DoorObject == null) { DoorObject = GameObject.Find("Door"); }

        Player = GameObject.Find("Player");


        if (cameraSide)
        {
            cameraPosition = new Vector3(0f, 1f, 1.5f);
            //cameraRotation = new Vector3(0f, -155f, 0f);
        }
        else
        {
            cameraPosition = new Vector3(0f, 1f, -1.5f);
            //cameraRotation = new Vector3(0f, -25f, 0f);
        }

        SceneCamera.gameObject.GetComponent<Transform>().localPosition = cameraPosition;
        //SceneCamera.gameObject.GetComponent<Transform>().localRotation = Quaternion.Euler(cameraRotation);
		SceneCamera.gameObject.GetComponent<Transform>().LookAt(BodyTrans);
    }
	
	// Update is called once per frame
	void Update ()
    {
        
        if (Player.GetComponent<CharacterController>().isInteracting)
        {
            if (!buttonStatus && playerInRange)
            {
                StartCoroutine(this._Cutscene());

            }
        }
        
    }

    public IEnumerator _PressButton()
    {
        buttonStatus = true;

        while (BodyTrans.localPosition.x > pressedXPosition)
        {
            BodyTrans.localPosition -= new Vector3((Time.deltaTime * buttonAnimSpeed), 0f, 0f);
            //UPDATE 1: add yield
            yield return null;
        }


        float timer = 0f;
        while (timer < clickPauseTime)
        {
            timer += Time.deltaTime;
            yield return null;
        }

        while (BodyTrans.localPosition.x < defaultXPosition)
        {
            BodyTrans.localPosition += new Vector3((Time.deltaTime * buttonAnimSpeed / 20), 0f, 0f);
            //UPDATE 1: add yield
            yield return null;
        }

        //Change door status
        //UPDATE 1: add yield
        yield return null;
    }

    void OnTriggerEnter(Collider c)
	{
        if (c.gameObject == Player)
        {
            playerInRange = true;
        }
    }
    void OnTriggerExit(Collider c)
    {
        if (c.gameObject == Player)
        {
            playerInRange = false;
        }
    }

    public IEnumerator _Cutscene()
	{
		Player.GetComponent<Transform>().rotation = this.transform.rotation;
		Player.GetComponent<Transform>().Rotate(0, -90, 0);
		Player.GetComponent<Transform>().position = this.transform.position + new Vector3(0f,-1.5f,0f) + this.transform.right * 0.4f;
		StartCoroutine (Player.GetComponent<CharacterController> ()._LockMovement (0f));
        MainCamera.enabled = false; SceneCamera.enabled = true;

        float timer = 0f;
        while (timer < punchPauseTime)
        {
            timer += Time.deltaTime;
            yield return null;
        }
        StartCoroutine(this._PressButton());

        timer = 0f;
        while (timer < clickPauseTime / 2f)
        {
            timer += Time.deltaTime;
            yield return null;
        }

        //SceneCamera.GetComponent<Transform>().localRotation = Quaternion.Slerp(transform.localRotation, dest, Time.deltaTime * doorAnimSpeed);

        timer = 0f;
		bool begin = false;
        while (timer < 2f)
        {
            Vector3 targetDir = DoorObject.transform.GetChild(0).position - SceneCamera.GetComponent<Transform>().position;
            // The step size is equal to speed times frame time.
            Vector3 newDir = Vector3.RotateTowards(SceneCamera.GetComponent<Transform>().forward, targetDir, CameraRotationSpeed * Time.deltaTime, 0.0f);
            Debug.DrawRay(transform.position, newDir, Color.red);
            // Move our position a step closer to the target.
            SceneCamera.GetComponent<Transform>().rotation = Quaternion.LookRotation(newDir);
            timer += Time.deltaTime;
			if (!begin && timer > 0.5f) {
				DoorObject.GetComponent<DoorScript>().buttonGo = true;
				begin = true;
			}
            yield return null;
		}

        timer = 0f;
        while (timer < 1f)
        {
            timer += Time.deltaTime;
            yield return null;
        }
		MainCamera.enabled = true; SceneCamera.enabled = false;
		StartCoroutine (Player.GetComponent<CharacterController> ()._UnlockMovement ());
		SceneCamera.gameObject.GetComponent<Transform>().LookAt(BodyTrans);

        buttonStatus = false;
        yield return null;
    }
}