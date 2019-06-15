﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerScript : MonoBehaviour {

	Animator anim;
	public State state;

	public GameObject cam,headBone;
	public static SocketObject so;
	public static DataWorker dw;
	public string id,name;
	public bool isPlayer = false;
	public bool destroy = false;
	public bool debug = false;
	public Vector3 impact;
	public float jumpPower;

    private Vector3 velocity;
	private float defaultSpeed = 5.0f;
	private float moveSpeed;
    private fpsCamera fpsCam;
	private float time = 0;
	private Quaternion syncRotBufferV,syncRotBufferH;
	private bool isGroubded = true;
	private Quaternion bufferHead, bufferBody;
	private Vector3 toPos;

    [SerializeField]
    private GameObject avater;

	// Use this for initialization
	void Start () {
		so = SocketObject.Instance;
		dw = DataWorker.Instance;
		moveSpeed = defaultSpeed;
		state = new State ();
		StartCoroutine ("SyncPosition");
		fpsCam = GetComponent<fpsCamera> ();
		anim = GetComponent<Animator> ();
		toPos = transform.position;
		transform.LookAt (new Vector3(0,transform.position.y,0));
		fpsCam.hRotation = transform.rotation;
		fpsCam.GetComponent<fpsCamera> ().owner = gameObject;

        //avater.GetComponent<MeshCollider>().sharedMesh = avater.GetComponent<SkinnedMeshRenderer>().sharedMesh;
	}
	
	// Update is called once per frame
	void Update () {

		time += Time.deltaTime;

		if (so == null && !debug)
			return;

		if (!isPlayer) {
			
			if (dw.heatbeat.ContainsKey (id)) {
				destroy = false;
				dw.heatbeat.Remove (id);
			}


			if (dw.posSync.ContainsKey (id)) {
				toPos = dw.posSync [id];
				//transform.position = toPos;
				dw.posSync.Remove (id);
			}

			transform.position = Vector3.Lerp (transform.position, toPos, 0.5f);
            //rot -> LateUpdate()

            if (Vector3.Distance(transform.position,toPos) > 0.1f)
            {
                anim.SetBool("Walk", true);
            }
            else
            {
                transform.position = toPos;
                anim.SetBool("Walk", false);
            }

            exitPlayer ();
			return;
		}

		if (time > 5 && !debug) {
			var data = new Dictionary<string,string> ();
			so.EmitMessage ("HeartBeat", data);
			time = 0;
		}
		

		if (transform.position.y < -10f) {
			dw.disconnectUser (id);
		}

		if (impact != Vector3.zero) {
			GetComponent<Rigidbody> ().AddForce (impact, ForceMode.Impulse);
			impact = Vector3.zero;
			return;
		}

		RaycastHit hit;
		if (Physics.Raycast (transform.position, Vector3.down, out hit, 3f)) {
			if (hit.collider.gameObject.CompareTag ("Stage") || hit.collider.gameObject.CompareTag ("fallenObstacle")) {
				Debug.Log ("地上！");
				if (Input.GetKeyDown (KeyCode.Space)) {
					GetComponent<Rigidbody> ().AddForce (new Vector3 (0, jumpPower, 0), ForceMode.Impulse);
				}
			} else {
				Debug.Log("空中！");
			}
		}


		//左shiftでスニーク（？）
		moveSpeed = (Input.GetKey (KeyCode.LeftShift)) ? 1.0f : defaultSpeed;

		velocity = Vector3.zero;

		if (Input.GetKey (KeyCode.W)) {
			velocity.z += 1;
		}

		if (Input.GetKey (KeyCode.A)) {
			velocity.x -= 1;
		}

		if (Input.GetKey (KeyCode.S)) {
			velocity.z -= 1;
		}

		if (Input.GetKey (KeyCode.D)) {
			velocity.x += 1;
		}


		//velocity = velocity.normalized * moveSpeed * Time.deltaTime;
		velocity = velocity.normalized * (1+state.spd*0.1f) * Time.deltaTime;

		if (velocity.magnitude > 0) {
			anim.SetBool ("Walk",true);
			transform.position += fpsCam.hRotation * velocity;
		} else {
			anim.SetBool ("Walk",false);
		}

		Attack ();
	}

	//Animatorで制御されているボーンを強制的に動作させるLateUpdate
	void LateUpdate(){
		if (GetComponent<DeathCam> ().die)
			return;
		if (isPlayer) {
			headBone.transform.localRotation = fpsCam.vRotation;
			transform.rotation = fpsCam.hRotation;

			syncRotBufferV = headBone.transform.localRotation;
			syncRotBufferH = transform.rotation;
		} else {
			if (dw!=null&& dw.rotSync.ContainsKey (id)) {
				Vector2 toRot = dw.rotSync [id];
				Quaternion head = Quaternion.Euler (0, toRot.x, 0);
				Quaternion body = Quaternion.Euler (0, toRot.y, 0);

				bufferHead = head;
				bufferBody = body;
				dw.rotSync.Remove (id);
			}
			headBone.transform.localRotation = Quaternion.Lerp(headBone.transform.localRotation, bufferHead,0.5f);
			transform.rotation = Quaternion.Lerp(transform.rotation, bufferBody,0.5f);
		}
	}


	void exitPlayer(){
		if (time > 10 && destroy) {
			dw.exclusion (id);
		} else if (time > 10 && !destroy) {
			time = 0;
			destroy = true;
		}
	}
		
	void Attack(){
		AnimatorStateInfo stateInfo = anim.GetCurrentAnimatorStateInfo (0);
		if (Input.GetMouseButton (0)) {
			anim.SetTrigger ("Attack");
		}
	}

	IEnumerator SyncPosition(){
		if (isPlayer && !debug) {
			Vector3 bPos = transform.position;
			Quaternion bHead = syncRotBufferV;
			Quaternion bBody = syncRotBufferH;
			while (true) {
				if (Vector3.Distance (transform.position, bPos) > 0.1f || Quaternion.Angle(syncRotBufferH, bBody) > 0.1f || Quaternion.Angle(syncRotBufferV, bHead) > 0.1f) {
					var data = new Dictionary<string,string> ();
					data ["TYPE"] = "Transform";
					data ["x"] = transform.position.x.ToString ();
					data ["y"] = transform.position.y.ToString ();
					data ["z"] = transform.position.z.ToString ();
                    data["bodyY"] = syncRotBufferH.eulerAngles.y.ToString();
                    data["headY"] = syncRotBufferV.eulerAngles.y.ToString();
                    so.EmitMessage ("ToOwnRoom", data);
					Debug.Log ("Transform送信");
					bPos = transform.position;
                    bHead = syncRotBufferV;
                    bBody = syncRotBufferH;
                }
				yield return new WaitForSeconds (0.05f);
			}
		}
	}
}
