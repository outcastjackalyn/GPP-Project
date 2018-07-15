using UnityEngine;
using System.Collections;using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ExitScript : MonoBehaviour {

	public bool locked = true;
	public string targetScene = "Level1";
	public GameObject dataMan;
	ParticleSystem.MainModule main;
	// Use this for initialization
	void Start () {
		bool unlocked;
		dataMan = GameObject.Find ("GameData");
		if (dataMan.GetComponent<GameData> ().map.TryGetValue (SceneManager.GetActiveScene ().name, out unlocked)) {
			locked = !unlocked;
		}
		main = this.gameObject.transform.GetChild (0).GetComponentInChildren<ParticleSystem> ().main;
	}

	// Update is called once per frame
	void Update () {
		if (locked) {
			this.gameObject.transform.GetChild (0).GetComponent<MeshRenderer> ().material.SetColor ("_Color", new Color (0.9755f, 1.0f, 0.4481f, 1f));
			main.startColor = new Color (0.9755f, 1.0f, 0.4481f, 1f);
		} else {
			this.gameObject.transform.GetChild (0).GetComponent<MeshRenderer> ().material.SetColor ("_Color", new Color (0.6412f, 0.2133f, 0.9622f, 1.0f));	
			main.startColor = new Color (0.6412f, 0.2133f, 0.9622f, 1.0f);
		}
	}


}


//Script requires Rigidbody and NavMeshAgent components.
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(UnityEngine.AI.NavMeshAgent))]
public class CharacterController : MonoBehaviour{
	
	#region Variables

	//Components.
	Rigidbody rb;
	protected Animator animator;
	public GameObject target;
	[HideInInspector]
	public Vector3 targetDashDirection;
	public Camera sceneCamera;
	private UnityEngine.AI.NavMeshAgent agent;
	public GameObject key;

	//Movement.
	[HideInInspector]
	public bool canMove = true;
	public float walkSpeed = 5f;
	float moveSpeed;
	public float runSpeed = 10f;
	public float fastRunSpeed = 18f;
	float rotationSpeed = 40f;
	Vector3 inputVec;
	Vector3 newVelocity;
	public float slopeAmount = 0.1f;
	public bool onAllowableSlope;

	//Navmesh.
	public bool useNavMesh = false;
	private float navMeshSpeed;
	public Transform goal;

	//Jumping.
	public float gravity = -9.8f;
	[HideInInspector]
	public bool canJump;
	bool isJumping = false;
	[HideInInspector]
	public bool isGrounded;
	public float jumpSpeed = 12;
	public float doublejumpSpeed = 16;
	bool doublejumping = false;
	//[HideInInspector]
	public bool canDoubleJump = false;
	[HideInInspector]
	public bool isDoubleJumping = false;
	bool doublejumped = false;
	bool isFalling;
	bool startFall;
	float fallingVelocity = -0.01f;
	float fallTimer = 0f;
	public float fallDelay = 0.2f;
	float distanceToGround;

	//Movement in the air.
	public float inAirSpeed = 8f;
	float maxVelocity = 2f;
	float minVelocity = -2f;

	//Strafing / Actions.
	[HideInInspector]
	public bool canAction = true;
	[HideInInspector]
	public bool isStrafing = false;
	[HideInInspector]
	public bool isDead = false;
	public float knockbackMultiplier = 1f;
	bool isKnockback;
	public bool isAttacking = false;
	public bool isInteracting = false;
	public int health = 0;

    //Input variables.
    float inputHorizontal = 0f;
	float inputVertical = 0f;
	//float inputDashVertical = 0f;
	//float inputDashHorizontal = 0f;
	bool inputBlock;
	//bool inputLightHit;
	//bool inputDeath;
	bool inputAttackR;
	bool inputAttackL;
	//bool inputCastL;
	//bool inputCastR;
	bool inputJump;

	//Boost
	public Boost boost = Boost.NONE;
	public float boostTimer = 0f;
	public float boostMax = 4f;

	#endregion

	#region Initialization and Inputs

	void Start(){
		//Find the Animator component.
		if(animator = GetComponentInChildren<Animator>()){
		}
		else{
			Debug.LogError("ERROR: There is no animator for character.");
			Destroy(this);
		}
		//Use MainCamera if no camera is selected.
		if(sceneCamera == null){
			sceneCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
			if(sceneCamera == null){
				Debug.LogError("ERROR: There is no camera in scene.");
				Destroy(this);
			}
		}
		rb = GetComponent<Rigidbody>();
		agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
		agent.enabled = false;

		key = GameObject.Find ("Key");
	}

	/// <summary>
	/// Input abstraction for easier asset updates using outside control schemes.
	/// </summary>
	void Inputs(){
        //inputDashHorizontal = Input.GetAxisRaw("DashHorizontal");
        //inputDashVertical = Input.GetAxisRaw("DashVertical");
        inputHorizontal = Input.GetAxisRaw("Horizontal");
        inputVertical = Input.GetAxisRaw("Vertical");
        //inputLightHit = Input.GetButtonDown("LightHit");
        //inputDeath = Input.GetButtonDown("Death");
        inputAttackL = Input.GetButtonDown("AttackL");
		inputAttackR = Input.GetButtonDown("AttackR");
		//inputCastL = Input.GetButtonDown("CastL");
		//inputCastR = Input.GetButtonDown("CastR");
		inputBlock = Input.GetButton("TargetBlock");
		inputJump = Input.GetButtonDown("Jump");
	}

	#endregion

	#region Updates

	void Update(){
		getTarget ();
		Inputs();
		if(canMove && !isDead && !useNavMesh){
			CameraRelativeMovement();
		} 
		Jumping();
		/*
		if(inputDeath && canAction && isGrounded){
			if(!isDead){
				Death();
			}
			else{
				Revive();
			}
		}*/
		if(inputAttackL && canAction && isGrounded) {
			Attack(1);
		}
		if(inputAttackR && canAction && isGrounded) {
			AttackKick(2);
		}
		//Strafing.
		if(inputBlock && canAction) {  
			isStrafing = true;
			animator.SetBool("Strafing", true);
		}
		else{
			isStrafing = false;
			animator.SetBool("Strafing", false);
		}
		//Navmesh.
		if(useNavMesh){
			agent.enabled = true;
			navMeshSpeed = agent.velocity.magnitude;
		}
		else{
			agent.enabled = false;
		}
		//Navigate to click.
		if(Input.GetMouseButtonDown(0))
		{
			if(useNavMesh)
			{
				RaycastHit hit;
				if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 100)) {
					agent.destination = hit.point;
				}
			}
		}
		//Slow time.
		if(Input.GetKeyDown(KeyCode.T)){
			if(Time.timeScale != 1){
				Time.timeScale = 1;
			}
			else{
				Time.timeScale = 0.15f;
			}
		}
		//Pause.
		if(Input.GetKeyDown(KeyCode.P)){
			if(Time.timeScale != 1){
				Time.timeScale = 1;
			}
			else{
				Time.timeScale = 0f;
			}
		}
		if (boostTimer >= boostMax) {
			switch (boost) {
			case Boost.SPEED:
				if (fastRunSpeed > runSpeed) {
					float f = runSpeed;
					runSpeed = fastRunSpeed;
					fastRunSpeed = f;
				}
				animator.speed = 1.5f;
				this.gameObject.transform.GetChild(2).GetComponent<ParticleSystem>().Play();
				break;
			case Boost.JUMP:
				doublejumping = true;
				this.gameObject.transform.GetChild(3).GetComponent<ParticleSystem>().Play();
				break;
			default:
				break;
			}
		}
		if (boostTimer > 0) {
			boostTimer -= Time.deltaTime;
		}
		if (boostTimer <= 0f) {
			if (fastRunSpeed < runSpeed) {
				float f = runSpeed;
				runSpeed = fastRunSpeed;
				fastRunSpeed = f;
			}
			animator.speed = 1f;
			this.gameObject.transform.GetChild(2).GetComponent<ParticleSystem>().Stop();
			this.gameObject.transform.GetChild(3).GetComponent<ParticleSystem>().Stop();
			doublejumping = false;
			boost = Boost.NONE;
		}
	}

	void FixedUpdate(){
		CheckForGrounded();
		//Apply gravity.
		rb.AddForce(0, gravity, 0, ForceMode.Acceleration);
		AirControl();
		//Check if character can move.
		if(canMove && !isDead){
			moveSpeed = UpdateMovement();  
		}
		//Check if falling.
		if(rb.velocity.y < fallingVelocity && useNavMesh != true){
			isFalling = true;
			animator.SetInteger("Jumping", 2);
			canJump = false;
		}
		else{
			isFalling = false;
		}
	}

	/// <summary>
	/// Get velocity of rigid body and pass the value to the animator to control the animations.
	/// </summary>
	void LateUpdate(){
		if(!useNavMesh){
			//Get local velocity of charcter
			float velocityXel = transform.InverseTransformDirection(rb.velocity).x;
			float velocityZel = transform.InverseTransformDirection(rb.velocity).z;
			//Update animator with movement values
			animator.SetFloat("Velocity X", velocityXel / runSpeed);
			animator.SetFloat("Velocity Z", velocityZel / runSpeed);
			//if character is alive and can move, set our animator
			if(!isDead && canMove){
				if(moveSpeed > 0){
					animator.SetBool("Moving", true);
				}
				else{
					animator.SetBool("Moving", false);
				}
			}
		}
		else{
			animator.SetFloat("Velocity X", agent.velocity.sqrMagnitude);
			animator.SetFloat("Velocity Z", agent.velocity.sqrMagnitude);
			if(navMeshSpeed > 0){
				animator.SetBool("Moving", true);
			}
			else{
				animator.SetBool("Moving", false);
			}
		}
	}

	#endregion

	#region UpdateMovement

	/// <summary>
	/// Movement based off camera facing.
	/// </summary>
	void CameraRelativeMovement(){
		//converts control input vectors into camera facing vectors.
		Transform cameraTransform = sceneCamera.transform;
		//Forward vector relative to the camera along the x-z plane.
		Vector3 forward = cameraTransform.TransformDirection(Vector3.forward);
		forward.y = 0;
		forward = forward.normalized;
		//Right vector relative to the camera always orthogonal to the forward vector.
		Vector3 right = new Vector3(forward.z, 0, -forward.x);
		inputVec = inputHorizontal * right + inputVertical * forward;
	}

	/// <summary>
	/// Rotate character towards movement direction.
	/// </summary>
	void RotateTowardsMovementDir(){
		if(inputVec != Vector3.zero && !isStrafing){
			transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(inputVec), Time.deltaTime * rotationSpeed);
		}
	}

	/// <summary>
	/// Applies velocity to rigidbody to move the character, and controls rotation if not targetting.
	/// </summary>
	/// <returns>The movement.</returns>
	float UpdateMovement(){
		if(!useNavMesh){
			CameraRelativeMovement();
		}
		Vector3 motion = inputVec;
		//Reduce input for diagonal movement.
		if(motion.magnitude > 1){
			motion.Normalize();
		}
		if(canMove){
			//Set speed by walking / running.
			if(isStrafing){
				newVelocity = motion * walkSpeed;
			}
			else{
				newVelocity = motion * runSpeed;
			}
			//Rolling uses rolling speed and direction.
		}
		if(!isStrafing || !canMove){
			RotateTowardsMovementDir();
		}
		if(isStrafing){
			//Make character face target.
			Quaternion targetRotation;
			Vector3 targetPos = target.transform.position;
			targetRotation = Quaternion.LookRotation(targetPos - new Vector3(transform.position.x, 0, transform.position.z));
			transform.eulerAngles = Vector3.up * Mathf.MoveTowardsAngle(transform.eulerAngles.y, targetRotation.eulerAngles.y, (rotationSpeed * Time.deltaTime) * rotationSpeed);
		}
		newVelocity.y = rb.velocity.y;
		rb.velocity = newVelocity;
		//Return movement value for Animator component.
		return inputVec.magnitude;
	}

	#endregion

	#region Jumping

	/// <summary>
	/// Checks if character is within a certain distance from the ground, and markes it IsGrounded.
	/// </summary>
	void CheckForGrounded(){
		RaycastHit hit;
		Vector3 offset = new Vector3(0, 0.1f, 0);
		if(Physics.Raycast((transform.position + offset), -Vector3.up, out hit, 100f)){
			distanceToGround = hit.distance;
			if(distanceToGround < slopeAmount){
				isGrounded = true;
				if(!isJumping){
					canJump = true;
				}
				startFall = false;
				doublejumped = false;
				canDoubleJump = false;
				isFalling = false;
				fallTimer = 0;
				if(!isJumping){
					animator.SetInteger("Jumping", 0);
				}
			}
			else{
				fallTimer += 0.009f;
				if(fallTimer >= fallDelay){
					isGrounded = false;
				}
			}
		}
	}

	void Jumping(){
		if(isGrounded){
			if(canJump && inputJump){
				StartCoroutine(_Jump());
			}
		}
		else{    
			canDoubleJump = true;
			canJump = false;
			if(isFalling){
				//Set the animation back to falling.
				animator.SetInteger("Jumping", 2);
				//Prevent from going into land animation while in air.
				if(!startFall){
					animator.SetTrigger("JumpTrigger");
					startFall = true;
				}
			}
			if(canDoubleJump && doublejumping && inputJump && !doublejumped){
				//Apply the current movement to launch velocity.
				rb.velocity += doublejumpSpeed * Vector3.up;
				animator.SetInteger("Jumping", 3);
				doublejumped = true;
				this.gameObject.transform.GetChild(4).GetComponent<ParticleSystem>().Emit(50);
			}
		}
	}

	public IEnumerator _Jump(){
		isJumping = true;
		animator.SetInteger("Jumping", 1);
		animator.SetTrigger("JumpTrigger");
		//Apply the current movement to launch velocity.
		rb.velocity += jumpSpeed * Vector3.up;
		canJump = false;
		yield return new WaitForSeconds(.5f);
		isJumping = false;
	}

	/// <summary>
	/// Controls movement of character while in the air.
	/// </summary>
	void AirControl(){
		if(!isGrounded){
			CameraRelativeMovement();
			Vector3 motion = inputVec;
			motion *= (Mathf.Abs(inputVec.x) == 1 && Mathf.Abs(inputVec.z) == 1) ? 0.7f : 1;
			rb.AddForce(motion * inAirSpeed, ForceMode.Acceleration);
			//Limit the amount of velocity character can achieve.
			float velocityX = 0;
			float velocityZ = 0;
			if(rb.velocity.x > maxVelocity){
				velocityX = GetComponent<Rigidbody>().velocity.x - maxVelocity;
				if(velocityX < 0){
					velocityX = 0;
				}
				rb.AddForce(new Vector3(-velocityX, 0, 0), ForceMode.Acceleration);
			}
			if(rb.velocity.x < minVelocity){
				velocityX = rb.velocity.x - minVelocity;
				if(velocityX > 0){
					velocityX = 0;
				}
				rb.AddForce(new Vector3(-velocityX, 0, 0), ForceMode.Acceleration);
			}
			if(rb.velocity.z > maxVelocity){
				velocityZ = rb.velocity.z - maxVelocity;
				if(velocityZ < 0){
					velocityZ = 0;
				}
				rb.AddForce(new Vector3(0, 0, -velocityZ), ForceMode.Acceleration);
			}
			if(rb.velocity.z < minVelocity){
				velocityZ = rb.velocity.z - minVelocity;
				if(velocityZ > 0){
					velocityZ = 0;
				}
				rb.AddForce(new Vector3(0, 0, -velocityZ), ForceMode.Acceleration);
			}
		}
	}

	#endregion

	#region Actions

	//0 = No side.
	//1 = Left.
	//2 = Right.
	public void Attack(int attackSide){
		if (canAction && isGrounded) {
			isInteracting = true;
			animator.SetInteger("Action", 3);
			StartCoroutine (_LockMovementAndAct (0, .75f));
			animator.SetTrigger("AttackTrigger");
        }
	}

	public void AttackKick(int kickSide){
		if(isGrounded){
			isAttacking = true;
			animator.SetInteger("Action", kickSide);
			animator.SetTrigger("AttackKickTrigger");
			StartCoroutine(_LockMovementAndAct(0, .8f));
		}
	}

	public void GetHit(){
		int hits = 5;
		int hitNumber = Random.Range(1, hits + 1);
		animator.SetInteger("Action", hitNumber);
		animator.SetTrigger("GetHitTrigger");
		StartCoroutine(_LockMovementAndAct(.1f, .4f));
		//Apply directional knockback force.
		if(hitNumber <= 2){
			StartCoroutine(_Knockback(-transform.forward, 8, 4));
		}
		else if(hitNumber == 3){
			StartCoroutine(_Knockback(-transform.right, 8, 4));
		}
		else if(hitNumber == 4){
			StartCoroutine(_Knockback(transform.forward, 8, 4));
		}
		else if(hitNumber == 5){
			StartCoroutine(_Knockback(transform.right, 8, 4));
		}
	}

	IEnumerator _Knockback(Vector3 knockDirection, int knockBackAmount, int variableAmount){
		isKnockback = true;
		StartCoroutine(_KnockbackForce(knockDirection, knockBackAmount, variableAmount));
		yield return new WaitForSeconds(.1f);
		isKnockback = false;
	}

	IEnumerator _KnockbackForce(Vector3 knockDirection, int knockBackAmount, int variableAmount){
		while(isKnockback){
			rb.AddForce(knockDirection * ((knockBackAmount + Random.Range(-variableAmount, variableAmount)) * (knockbackMultiplier * 10)), ForceMode.Impulse);
			yield return null;
		}
	}

	#endregion

	#region Misc

	public void Death(){
		animator.SetInteger("Action", 1);
		animator.SetTrigger("DeathTrigger");
		StartCoroutine(_LockMovementAndAct(.1f, 1.5f));
		isDead = true;
		animator.SetBool("Moving", false);
		inputVec = new Vector3(0, 0, 0);
	}

	public void Revive(){
		animator.SetInteger("Action", 1);
		animator.SetTrigger("ReviveTrigger");
		isDead = false;
	}

	//Kkeep character from moveing while attacking, etc.
	public IEnumerator _LockMovementAndAct(float delayTime, float lockTime){
		yield return new WaitForSeconds(delayTime);
		canAction = false;
		canMove = false;
		animator.SetBool("Moving", false);
		rb.velocity = Vector3.zero;
		rb.angularVelocity = Vector3.zero;
		inputVec = new Vector3(0, 0, 0);
		animator.applyRootMotion = true;
		yield return new WaitForSeconds(lockTime);
		canAction = true;
		canMove = true;
		isAttacking = false;
		isInteracting = false;
        animator.applyRootMotion = false;
	}

	//Placeholder functions for Animation events.
	public void Hit(){
	}

	public void Shoot(){
	}

	public void FootR(){
	}

	public void FootL(){
	}

	public void Land(){
	}

	public void Jump(){
	}

	#endregion

	public IEnumerator nextLevel(GameObject obj, string name) {
		// do teleport animation i guess?
		SceneManager.LoadSceneAsync(name);
		yield return null;
	}


	void OnTriggerEnter(Collider c)
	{
		if (isAttacking) {
			if (c.gameObject.tag == "Enemy") {
				c.gameObject.GetComponent<EnemySlimeScript> ().isHit = true;
			}
		}

		if (c.gameObject.tag == "Platform") {
			transform.parent = c.transform;
		}

		if (c.gameObject.tag == "Exit") {
			if (c.gameObject.GetComponent<ExitScript>().locked) {
				if (key.transform.parent == this.transform) { 
				
					key.transform.parent = null;
					//key.transform
					//maybe make coroutine to jetison the key upwards a whole bunch
					c.gameObject.GetComponent<ExitScript>().locked = false;
					string scene = SceneManager.GetActiveScene ().name;
					GameObject.Find ("GameData").GetComponent<GameData> ().toggleMapValue (scene);

				}
			} else {
				StartCoroutine (nextLevel (key, c.gameObject.GetComponent<ExitScript>().targetScene));
			}
		}
	}

	void OnTriggerExit(Collider c)
	{
		if (c.gameObject.tag == "Platform") {
			if(c.gameObject.transform == transform.parent){
				transform.parent = null;
			}
		}
	}

	void getTarget() {
		float minDist = 100000000000000f;
		GameObject current = null;
		foreach (GameObject obj in GameObject.FindGameObjectsWithTag ("Enemy")) {
			float dist = (obj.transform.position - this.transform.position).magnitude;
			if (dist < minDist) {
				minDist = dist;
				current = obj;
			}
		}
		if (current != null) {
			target = current;
		}
	}
}