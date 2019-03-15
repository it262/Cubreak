using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NameIndicater : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		GetComponent<TextMesh> ().text = transform.parent.gameObject.GetComponent<PlayerScript> ().name;
		if (Camera.main) {
			transform.LookAt (Camera.main.transform.position);
		}
	}
}
