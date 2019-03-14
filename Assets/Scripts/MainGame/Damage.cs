using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Damage : MonoBehaviour {

	public float a = 0;
	public float def = 0;
	float time = 0;

	// Update is called once per frame
	void Update () {
		time += Time.deltaTime;
		if (time > 1f) {
			def = (def < 0)? 0:(def-0.1f);
			time = 0;
		}
		a = Mathf.Lerp (a, def, 0.1f);
		GetComponent<Image> ().color = new Color (1,0,0,a);
	}
}
