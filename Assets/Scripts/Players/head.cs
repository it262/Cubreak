using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class head : MonoBehaviour {
    public fpsCamera fpsCam;
	GameObject so;
	Quaternion b;

	[SerializeField]GameObject headBone; 

    // Use this for initialization
    void Start () {
		so = GameObject.Find ("SocketIO");
		b = transform.rotation;
	}
	
	// Update is called once per frame
	void Update () {
		if (so == null) {
			transform.rotation = fpsCam.hRotation * fpsCam.vRotation;
			return;
		}

		/*

		if (transform.root.gameObject.GetComponent<player> ().isPlayer) {
			transform.rotation = fpsCam.hRotation * fpsCam.vRotation;
			if (Quaternion.Angle (transform.rotation, b) > 0) {
				Debug.Log ("Rotation送信");
				SyncRotation (transform.rotation);
			}
			b = transform.rotation;
		} else {
			string id = transform.root.gameObject.GetComponent<player> ().id;
			if(so.GetComponent<DataWorker> ().rotSync.ContainsKey (id)){
				Quaternion toRot = so.GetComponent<DataWorker> ().rotSync [id];
				if (Quaternion.Angle (transform.rotation, toRot) > 0.1f) {
					transform.rotation = Quaternion.Lerp (transform.rotation, toRot, 0.8f);
				} else {
					transform.rotation = toRot;
					so.GetComponent<DataWorker> ().rotSync.Remove (id);
				}
			}
		}
		*/
    }

	void SyncRotation(Quaternion rot){
		var data = new Dictionary<string,string> ();
		data ["TYPE"] = "rot";
		data ["x"] = rot.x.ToString ();
		data ["y"] = rot.y.ToString ();
		data ["z"] = rot.z.ToString ();
		data ["w"] = rot.w.ToString ();
		so.GetComponent<SocketObject> ().EmitMessage("toOwnRoom",data);
	}
}
