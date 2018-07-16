using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class EnemySlimeScript : MonoBehaviour {

	public Material baseMat;
	public Material damagedMat;
	public Material angryMat;

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
	public bool isRetreating = false;
	public float attackRange = 1f;
	public bool isDead = false;
	public int jumpSinceAttack = 0;

	// Use this for initialization
	void Start () {
		baseMat = this.gameObject.transform.GetChild (0).GetComponent<MeshRenderer> ().material;
		state = PatrolState.AGGRO;
		isHit = false;
		isAttacking = false;
		isRetreating = false;
		immuneTimer = 1.5f;
		health = data.size;
		speed = data.moveSpeed;
		target = transform;
		attackRange = speed * data.size * data.scale + 1f;
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

		if (immuneTimer > 0f) {
			immuneTimer -= Time.deltaTime;
			isHit = false;
			int i = (int)(immuneTimer * 10);
			if (i % 3 == 0) {
				damaged = true;
			}
		} else if (isAttacking) {
			isHit = false;
			//is also immune Touch damage during it's attack
		}
		else if (isHit) {
			this.gameObject.transform.GetChild(5).LookAt (GameObject.Find("B_Head").GetComponent<Transform>());
			this.gameObject.transform.GetChild(5).GetComponent<ParticleSystem>().Emit(50);
			if (health > 1) {
				health--;
			} else {
				
				StartCoroutine (die ());
			}
			immuneTimer = 1.5f;
			isHit = false;
		}

		if(velocity.magnitude > 0.1f) {
			transform.position += new Vector3 (velocity.x * Time.deltaTime, velocity.y * Time.deltaTime, velocity.z * Time.deltaTime);
		}

		if (damaged) {
			//this.gameObject.transform.GetChild (0).GetComponent<MeshRenderer> ().material.SetColor ("_Color", new Color (0.8113f, 0.6647f, 0f, 0.4f));
			this.gameObject.transform.GetChild (0).GetComponent<MeshRenderer> ().material = damagedMat;
		} else if (isAttacking) {
			this.gameObject.transform.GetChild (0).GetComponent<MeshRenderer> ().material = angryMat;
		} else {
			this.gameObject.transform.GetChild (0).GetComponent<MeshRenderer> ().material = baseMat;	
		}
		Jumping();

	}

	void FixedUpdate(){
		CheckForGrounded();
		//Apply gravity.
		//rb.AddForce(0, gravity, 0, ForceMode.Acceleration);
		if(!isGrounded) {
			velocity.y += gravity * Time.deltaTime;
		} else if( velocity.y < 0f) {
			velocity.y = 0f;
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
		Vector3 offset = new Vector3(0f, -0.51f * transform.localScale.y, 0f);
		if(Physics.Raycast((transform.position + offset), -Vector3.up, out hit, 100f)) {
			distanceToGround = hit.distance;
			if (distanceToGround < slopeAmount * 5f) {
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

			if(canJump && !isJumping && !isAttacking && !isRetreating && immuneTimer <= 0f) {
				if ((target.position - transform.position).magnitude < attackRange && jumpSinceAttack > 2) {
					StartCoroutine (Attack ());
				} else {
					StartCoroutine (Jump ());
				}
			}
		}
	}


	public IEnumerator Attack() {
		jumpSinceAttack = 0;
		isJumping = true;
		isAttacking = true;
		yield return new WaitForSeconds(0.6f);
		//Apply the current movement to launch velocity.
		Vector3 attackDir = (target.position - transform.position);
		velocity += speed * new Vector3(0f, 4f, 0f); //ATTACK!!
		velocity += speed * (attackDir.magnitude > attackDir.normalized.magnitude ? attackDir : attackDir.normalized) * 0.9f;
		//velocity += speed * (Player.transform.position - transform.position);
		yield return new WaitForSeconds(0.2f);
		isRetreating = true;
		while (!isGrounded) { //retreat the direction it came
			yield return null;
		}
		isAttacking = false;
		jumpSinceAttack ++;
		velocity += speed * new Vector3(0f, 2f, 0f);
		velocity -= speed * attackDir.normalized ;
		yield return new WaitForSeconds(0.5f);
		while (!isGrounded) { //retreat from player
			yield return null;
		}
		yield return new WaitForSeconds(0.2f); //allow time to rotate a bit
		isAttacking = false;
		jumpSinceAttack ++;
		velocity += speed * new Vector3(0f, 2f, 0f);
		velocity -= speed * (target.position - transform.position).normalized ;
		yield return new WaitForSeconds(0.2f);
		while (!isGrounded) { //set to not retreating or jumping so it can jump or attack again
			yield return null;
		}
		isJumping = false;
		isRetreating = false;
	}


	public IEnumerator Jump() {
		jumpSinceAttack ++;
		isJumping = true;
		//Apply the current movement to launch velocity.
		velocity += speed * new Vector3(0f, 3f, 0f);
		velocity += speed * (target.position - transform.position).normalized;
		//velocity += speed * (Player.transform.position - transform.position);
		yield return new WaitForSeconds((target.position - transform.position).magnitude > attackRange ? 2f : 0.5f);
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
