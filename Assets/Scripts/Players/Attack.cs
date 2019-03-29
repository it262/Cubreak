using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attack : MonoBehaviour
{
	static SocketObject so;

	public float forceHeight;
	public float forcePower;
	public float attackSpeed;
	public float attackRange;

	private float time;

    // Start is called before the first frame update
    void Start()
    {
		so = SocketObject.Instance;
		StartCoroutine (AttackByTime());
    }

    // Update is called once per frame
    void Update()
    {

    }

	IEnumerator AttackByTime(){
		while(true){
			RaycastHit hit;
			if (Input.GetMouseButton(0) && Physics.Raycast (GetComponent<PlayerScript> ().cam.transform.position, Camera.main.transform.forward, out hit, attackRange)) {
				if (hit.collider.gameObject.CompareTag ("Others")) {
					/*
					Vector3 toVec = getAngleVec (transform.position, hit.collider.gameObject.transform.position);
					toVec += new Vector3 (0, forceHeight, 0);
					Vector3 vec = toVec * forcePower;
					var data = new Dictionary<string,string> ();
					data ["TYPE"] = "Hit";
					data ["trg"] = hit.collider.gameObject.GetComponent<PlayerScript> ().id;
					data ["x"] = vec.x.ToString ();
					data ["y"] = vec.y.ToString ();
					data ["z"] = vec.z.ToString ();
					so.EmitMessage ("ToOwnRoom", data);
					*/
					Debug.Log (hit.collider.gameObject.tag);
					yield return new WaitForSeconds (attackSpeed);
				}
			}
			yield return null;
		}
	}

	Vector3 getAngleVec(Vector3 from,Vector3 to){
		Vector3 fromVec = new Vector3 (from.x, 0, from.z);
		Vector3 toVec = new Vector3 (to.x, 0, to.z);
		return Vector3.Normalize (toVec - fromVec);
	}

}
