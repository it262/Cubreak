using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Scorer : MonoBehaviour {

	static DataWorker dw;

	// Use this for initialization
	void Start () {
		dw = DataWorker.Instance;
	}

	// Update is called once per frame
	void Update () {
		GetComponent<TextMeshProUGUI> ().text = "SCORE:" + dw.score;
	}
}
