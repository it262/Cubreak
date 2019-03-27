using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathCam : MonoBehaviour
{

	GameObject cam;
	public bool die = false;

	[SerializeField]GameObject explosion;

    // Start is called before the first frame update
    void Start()
    {
		cam = GetComponent<PlayerScript> ().cam;
    }

    // Update is called once per frame
    void Update()
    {
		if (die) {
			if (cam.transform.parent != null) {
				cam.transform.parent = null;
				cam.transform.position = transform.position + new Vector3 (0,5f,-5f);
				//GetComponent<PlayerScript> ().enabled = false;
				GetComponent<Rigidbody> ().useGravity = false;
				GetComponent<Rigidbody> ().constraints = RigidbodyConstraints.None;
				GetComponent<Rigidbody> ().angularVelocity = new Vector3 (Random.Range(0,100f),Random.Range(0,100f),Random.Range(0,100f));
				GetComponent<AudioSource> ().Play ();
				Instantiate (explosion);
			}

			cam.transform.LookAt(this.gameObject.transform.position);
		}
    }
}
