using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerScript : MonoBehaviour {

	public State state;

	public GameObject cam;

	public static SocketObject so;
	public static DataWorker dw;
	public string id,name;

	public GameObject headBone;

    private Vector3 velocity;
	private float defaultSpeed = 5.0f;
	private float moveSpeed;
    public fpsCamera fpsCam;

	public bool isPlayer = false;

	public bool destroy = false;
	float time = 0;
	public GameObject damage;

	// Use this for initialization
	void Start () {
		so = SocketObject.Instance;
		dw = DataWorker.Instance;
		moveSpeed = defaultSpeed;
		state = new State ();
		StartCoroutine ("SyncPosition");
	}
	
	// Update is called once per frame
	void Update () {

		time += Time.deltaTime;

		if (so == null)
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

		if (time > 5) {
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
			transform.position += fpsCam.hRotation * velocity;
		}


		headBone.transform.localRotation = fpsCam.vRotation;
		transform.rotation = fpsCam.hRotation;

	}


	void exitPlayer(){
		if (time > 10 && destroy) {
			dw.exclusion (id);
		} else if (time > 10 && !destroy) {
			time = 0;
			destroy = true;
		}
	}

	void OnCollisionEnter(Collision collision){
		/*
		if (isPlayer) {
			if (collision.gameObject.CompareTag ("Obstacle")) {
				var data = new Dictionary<string,string> ();
				data ["TYPE"] = "Hit";
				data ["trg"] = id;
				data["damage"] = "10";
				so.EmitMessage ("ToOwnRoom", data);
			}
		}
		*/
	}

	public void hitting(){
		/*
		if (isPlayer) {
			damage.GetComponent<Damage> ().a = 1.0f;
			damage.GetComponent<Damage> ().def += 0.1f;
			if (damage.GetComponent<Damage> ().def >= 1.0f) {
				so.GetComponent<DataWorker> ().disconnectUser (id);
			}
		}
		*/
	}

	IEnumerator SyncPosition(){
		if (isPlayer) {
			Vector3 bPos = transform.position;
			Quaternion bHead = headBone.transform.localRotation;
			Quaternion bBody = transform.rotation;
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

				if (Quaternion.Angle (transform.rotation, bBody) > 0 || Quaternion.Angle (headBone.transform.localRotation,bHead) > 0) {
					var data = new Dictionary<string,string> ();
					data ["TYPE"] = "Rot";
					data ["bodyY"] = transform.rotation.eulerAngles.y.ToString();
					data ["headY"] = headBone.transform.localRotation.eulerAngles.y.ToString();
					so.EmitMessage ("ToOwnRoom", data);
					Debug.Log ("Rotation送信");
					bHead = headBone.transform.localRotation;
					bBody = transform.rotation;
				}

				yield return new WaitForSeconds (0.05f);
			}
		}
	}
}
