using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PostProcessing;

public class ColorCon : MonoBehaviour {

	static SocketObject so;
	static DataWorker dw;

	IntermittentChaos IChaos;

	List<GameObject> obs = new List<GameObject>();
	List<Color> def = new List<Color> ();

	public float intensity = 999;

	// Use this for initialization
	void Start () {
		so = SocketObject.Instance;
		IChaos = new IntermittentChaos ();
		foreach (Transform child in transform) {
			if (child.gameObject.CompareTag ("Obstacle")) {
				obs.Add (child.gameObject);
				//子の色を変更するために色をバックアップしている
				def.Add (child.gameObject.GetComponent<Renderer> ().material.GetColor ("_EmissionColor"));
			}
		}
		StartCoroutine ("Chaos");
	}

	// Update is called once per frame
	void Update () {
		//SocketObjectのconnectedがtrueになったら

		/*
		var Settings =  Camera.main.gameObject.GetComponent<PostProcessingBehaviour> ().profile.vignette.settings;
		Settings.intensity = (1-intensity) + 0.2f;
		Camera.main.gameObject.GetComponent<PostProcessingBehaviour> ().profile.vignette.settings = Settings;
		*/

		for (int i=0 ; i<obs.Count ; i++) {
			//intensityで着色・脱色をしている
			obs[i].GetComponent<Renderer> ().material.SetColor ("_EmissionColor",
				new Color (def[i].r,def[i].g,def[i].b)*intensity);
		}
	}

	IEnumerator Chaos(){
		while (true) {
			if (so.connecting) {
				intensity = (dw.searching) ? IChaos.getChaos (intensity) : 1;
			} else {
				intensity = 0;
			}
			yield return new WaitForSeconds (0.1f);
		}
	}
}
