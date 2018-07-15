using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class EnemySlimeScript : MonoBehaviour {

	private GameObject Player;
	public SlimeData data;
	public int health = 0;
	public float immuneTimer = 0f;
	public bool damaged = false;
	//public int size;

	//Movement
	public float speed;
	public float drag = 0f;
	//float moveSpeed;
	float rotationSpeed = 5f;
	Vector3 jumpVector = new Vector3(1f,2f,1f);
	Vector3 newVelocity;
	Vector3 velocity;
	public float gravity = -9.8f;
	public float slopeAmount = 0.12f;

	public PatrolState state;
	public Transform target;

	//Jumping
	public bool canJump;
	bool isJumping = false;
	public bool isGrounded;
	bool isFalling;
	float fallingVelocity = -0.1f;
	public float distanceToGround;

	//Actions
	public bool isHit = false;
	public bool isAttacking = false;
	public bool isDead = false;

	// Use this for initialization
	void Start () {
		state = PatrolState.AGGRO;
		isHit = false;
		immuneTimer = 3f;
		health = data.size;
		speed = data.moveSpeed;
		target = transform;
		Player = GameObject.Find("Player");
		this.gameObject.transform.GetChild (5).GetComponent<ParticleSystem> ().collision.SetPlane (0, GameObject.Find ("BouncePlane").transform);
		this.gameObject.transform.GetChild (4).GetComponent<ParticleSystem> ().collision.SetPlane (0, GameObject.Find ("BouncePlane").transform);
	}
	
	// Update is called once per frame
	void Update () {
		damaged = false;
		switch (state) {
		case PatrolState.AGGRO:
			target = Player.GetComponent<Transform> ();
			break;
		case PatrolState.PATROLLING: 
			target = gameObject.GetComponent<EnemyPatrolScript> ().next.transform;
			break;

		default: 
			break;
		};

		if (immuneTimer > 0) {
			immuneTimer -= Time.deltaTime;
			isHit = false;
			int i = (int) (immuneTimer * 10);
			if (i % 3 == 0) {
				damaged = true;
			}
			//put immuinity animation here :)
		}
		else if (isHit) {
			this.gameObject.transform.GetChild(5).LookAt (GameObject.Find("B_Head").GetComponent<Transform>());
			this.gameObject.transform.GetChild(5).GetComponent<ParticleSystem>().Emit(50);
			if (health > 1) {
				health--;
			} else {
				
				StartCoroutine (die ());
			}
			immuneTimer = 2f;
			isHit = false;
		}

		if(velocity.magnitude > 0.1f) {
			transform.position += new Vector3 (velocity.x * Time.deltaTime, velocity.y * Time.deltaTime, velocity.z * Time.deltaTime);
		}
		if (damaged) {
			
			this.gameObject.transform.GetChild (0).GetComponent<MeshRenderer> ().material.SetColor ("_Color", new Color (0.8113f, 0.6647f, 0f, 0.4f));
		} else {
			this.gameObject.transform.GetChild (0).GetComponent<MeshRenderer> ().material.SetColor ("_Color", new Color (0.0745f, 0.9019f, 0.9333f, 0.4f));	
		}
		Jumping();
	}

	void FixedUpdate(){
		CheckForGrounded();
		//Apply gravity.
		//rb.AddForce(0, gravity, 0, ForceMode.Acceleration);
		if(!isGrounded) {
			velocity.y += gravity * Time.deltaTime;
		} else if( velocity.y < 0) {
			velocity.y = 0;
		}

		//Check if character can move.
		if(!isDead){
		//	speed = UpdateMovement();  
		}
		//Check if falling.
		if(velocity.y < fallingVelocity) {
			isFalling = true;
			canJump = false;
		}
		else {
			isFalling = false;
		}

		if (Vector3.Distance (Player.transform.position, transform.position) > 20f) {
			state = PatrolState.PATROLLING;
		} else {
			state = PatrolState.AGGRO;
		}

	}
	void CheckForGrounded(){
		bool land = false;
		if (!isGrounded) {
			land = true;
		}
		RaycastHit hit;
		//RaycastHit hit2;
		Vector3 offset = new Vector3(0, -0.51f * transform.localScale.y, 0);
		if(Physics.Raycast((transform.position + offset), -Vector3.up, out hit, 100f)) {
			distanceToGround = hit.distance;
			if (distanceToGround < slopeAmount * 5) {
				velocity.y *= velocity.y < 0f ? 0.9f : 1f;
			}
			if (distanceToGround < slopeAmount) {
				if (land) {
					velocity = new Vector3 (velocity.x * drag, velocity.y, velocity.z * drag);
					this.gameObject.transform.GetChild(4).GetComponent<ParticleSystem>().Emit(20);
				}
				isGrounded = true;
				if (!isJumping) {
					canJump = true;
				}
				isFalling = false;
			} else {
				isGrounded = false;
			}
		}
		/*else if (Physics.Raycast((transform.position + offset), Vector3.up, out hit, 100f)){
			velocity.y = 1;
		}*/
	}

	void Jumping() {
		if(isGrounded) {
			transform.rotation = Quaternion.Slerp(
				transform.rotation,
				Quaternion.LookRotation(new Vector3(target.position.x - transform.position.x, 0, target.position.z - transform.position.z)),
				Time.deltaTime * rotationSpeed
			);
			//transform.LookAt (new Vector3 (target.position.x, 0, target.position.z));
			if(canJump && !isJumping) {
				StartCoroutine(_Jump());
			}
		}
	}

	public IEnumerator _Jump() {
		isJumping = true;
		//Apply the current movement to launch velocity.
		velocity += speed * new Vector3(0f, 3f, 0f);
		velocity += speed * (target.position - transform.position).normalized;
		//velocity += speed * (Player.transform.position - transform.position);
		yield return new WaitForSeconds(2f);
		isJumping = false;
	}

	float UpdateMovement() {
		Vector3 motion = jumpVector;
		if(isGrounded){
			//Reduce input for diagonal movement.
			if(motion.magnitude > 1){
				motion.Normalize();
			}
			if(!isDead){
				//Set speed by walking
				newVelocity = motion * speed;
			}
		}
		else{
			//If falling, use momentum.
			newVelocity = velocity;
		}
		newVelocity.y = velocity.y;
		velocity = newVelocity;
		//Return movement value for Animator component.
		return jumpVector.magnitude;
	}

	public IEnumerator die()
	{

		for (int i = 0; i < data.split; i++) {
			StartCoroutine (gameObject.GetComponentInChildren<SlimeSpawnerScript> ().spawn (new SlimeData (data.type, (data.size - 1))));
		}
		while (this.gameObject.transform.GetChild (5).GetComponent<ParticleSystem> ().isPlaying) {
			foreach (Transform t in GetComponentsInChildren<Transform>()) {
				t.localScale -= Time.deltaTime * t.localScale;
			}
			yield return false;
		}
		Destroy (this.gameObject);
		yield return true;
	}


}
