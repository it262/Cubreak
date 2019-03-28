using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attack : MonoBehaviour
{
	static SocketObject so;
	[SerializeField]GameObject player;

	public float forceHeight;
	public float forcePower;

    // Start is called before the first frame update
    void Start()
    {
		so = SocketObject.Instance;
    }

    // Update is called once per frame
    void Update()
    {
		RaycastHit hit;
		if (Physics.Raycast (GetComponent<PlayerScript> ().cam.transform.position, Camera.main.transform.forward, out hit, 10.0f)) {
			Debug.Log (hit.collider.gameObject.name);
		}
    }
	/*
	void OnTriggerEnter(Collider collision){
		Debug.Log (collision.gameObject.tag);
		if (player.GetComponent<PlayerScript>().isPlayer && collision.gameObject.CompareTag ("Others")) {
			Vector3 toVec = getAngleVec (player.transform.position, collision.gameObject.transform.position);
			toVec += new Vector3 (0, forceHeight, 0);
			Vector3 vec = toVec * forcePower;
			//collision.gameObject.GetComponent<Rigidbody> ().AddForce (vec,ForceMode.Impulse);
			var data = new Dictionary<string,string> ();
			data ["TYPE"] = "Hit";
			data ["trg"] = collision.gameObject.GetComponent<PlayerScript> ().id;
			data ["x"] = vec.x.ToString ();
			data ["y"] = vec.y.ToString ();
			data ["z"] = vec.z.ToString ();
			so.EmitMessage ("ToOwnRoom", data);
		}
	}

	Vector3 getAngleVec(Vector3 from,Vector3 to){
		Vector3 fromVec = new Vector3 (from.x, 0, from.z);
		Vector3 toVec = new Vector3 (to.x, 0, to.z);
		return Vector3.Normalize (toVec - fromVec);
	}
	*/
}
