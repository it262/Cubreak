using System.Collections;
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

    private Vector3 velocity;
	private float defaultSpeed = 5.0f;
	private float moveSpeed;
    private fpsCamera fpsCam;
	private float time = 0;
	private Quaternion syncRotBufferV,syncRotBufferH;

	// Use this for initialization
	void Start () {
		so = SocketObject.Instance;
		dw = DataWorker.Instance;
		moveSpeed = defaultSpeed;
		state = new State ();
		StartCoroutine ("SyncPosition");
		fpsCam = GetComponent<fpsCamera> ();
		anim = GetComponent<Animator> ();
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
				Vector3 toPos = dw.posSync [id];
				transform.position = toPos;
				dw.posSync.Remove (id);

			}

			if (dw.rotSync.ContainsKey (id)) {
				Vector2 toRot = dw.rotSync [id];
				Quaternion head = Quaternion.Euler (0, toRot.x, 0);
				Quaternion body = Quaternion.Euler (0, toRot.y, 0);

				headBone.transform.localRotation = head;
				transform.rotation = body;
				dw.rotSync.Remove (id);
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


		velocity = velocity.normalized * moveSpeed * Time.deltaTime;

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
		headBone.transform.localRotation = fpsCam.vRotation;
		transform.rotation = fpsCam.hRotation;

		syncRotBufferV = headBone.transform.localRotation;
		syncRotBufferH = transform.rotation;

		//transform.Rotate(new Vector3(180f,0,0));
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
				if (Vector3.Distance (transform.position, bPos) > 0) {
					var data = new Dictionary<string,string> ();
					data ["TYPE"] = "Pos";
					data ["x"] = transform.position.x.ToString ();
					data ["y"] = transform.position.y.ToString ();
					data ["z"] = transform.position.z.ToString ();
					so.EmitMessage ("ToOwnRoom", data);
					Debug.Log ("Position送信");
					bPos = transform.position;
				}

				if (Quaternion.Angle (syncRotBufferH, bBody) > 0 || Quaternion.Angle (syncRotBufferV,bHead) > 0) {
					var data = new Dictionary<string,string> ();
					data ["TYPE"] = "Rot";
					data ["bodyY"] = syncRotBufferH.eulerAngles.y.ToString();
					data ["headY"] = syncRotBufferV.eulerAngles.y.ToString();
					so.EmitMessage ("ToOwnRoom", data);
					Debug.Log ("Rotation送信");
					bHead = syncRotBufferV;
					bBody = syncRotBufferH;
				}

				yield return new WaitForSeconds (0.05f);
			}
		}
	}
}
