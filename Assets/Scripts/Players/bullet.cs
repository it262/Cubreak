using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bullet : MonoBehaviour {

	static SocketObject so;

	public GameObject player;

	void Update(){
		so = SocketObject.Instance;
		if (transform.position.y < -10f)
			Destroy (this.gameObject);
	}

	void OnCollisionEnter(Collision collision){
		if (collision.gameObject.CompareTag ("Stage")) {
			Destroy (this.gameObject);
		}
		if (collision.gameObject.CompareTag ("Others")) {
			var data = new Dictionary<string,string> ();
			data ["TYPE"] = "Hit";
			data ["trg"] = collision.gameObject.GetComponent<player>().id;
			data ["damage"] = player.GetComponent<player> ().state.atk.ToString();
			so.EmitMessage ("ToOwnRoom", data);
			Debug.Log ("Hit:" + collision.gameObject.GetComponent<player> ().id);
			Destroy (this.gameObject);
		}
	}
}
