using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HostIndicater : MonoBehaviour {

	static DataWorker dw;

	// Use this for initialization
	void Start () {
		dw = DataWorker.Instance;
	}

	// Update is called once per frame
	void Update () {
		if (dw != null) {
			foreach (string key in dw.players.Keys) {
				GetComponent<Text> ().text = key;
				break;
			}
		}
	}
}
