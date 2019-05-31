using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathCam : MonoBehaviour
{
	DataWorker dw;

	GameObject cam;
	public bool die = false;
	bool end = false;

	[SerializeField]GameObject explosion;

    // Start is called before the first frame update
    void Start()
    {
		dw = DataWorker.Instance;
		cam = GetComponent<PlayerScript> ().cam;
    }

    // Update is called once per frame
    void Update()
    {
		if (dw == null)
			return;
		if (dw.pushSwitch.ContainsKey (GetComponent<PlayerScript> ().id)) {
			dw.Exping = true;
			if (end) {
				dw.pushSwitch.Remove (GetComponent<PlayerScript> ().id);
				if (GetComponent<PlayerScript> ().isPlayer) {
					Destroy (cam);
					dw.disconnectUser (GetComponent<PlayerScript> ().id);
				}
			}
			if (GetComponent<PlayerScript> ().isPlayer) {
				if (cam.transform.parent != null) {
					cam.transform.parent = null;
					cam.transform.position = transform.position + new Vector3 (0, 5f, -5f);
					//GetComponent<PlayerScript> ().enabled = false;
					GetComponent<Rigidbody> ().useGravity = false;
					GetComponent<Rigidbody> ().constraints = RigidbodyConstraints.None;
					GetComponent<Rigidbody> ().angularVelocity = new Vector3 (Random.Range (0, 100f), Random.Range (0, 100f), Random.Range (0, 100f));
					StartCoroutine (dead ());
				}
				cam.transform.LookAt (this.gameObject.transform.position);
			} else {
				if (!die) {
					StartCoroutine (dead ());
					die = true;
				}
			}
		}
    }

	IEnumerator dead(){
		GameObject g = (GameObject)Instantiate (explosion,transform.position,Quaternion.identity);
		Destroy (g, 5f);
		GetComponent<AudioSource> ().Play ();
		yield return new WaitForSeconds (5f);
		end = true;
	}
}
