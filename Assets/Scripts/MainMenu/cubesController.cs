using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cubesController : MonoBehaviour {

	static SocketObject so;

    public GameObject prefab,cube;

	private GameObject sphire;

    List<GameObject> cubes = new List<GameObject>();

    //public Material[] materials;
	//public Material materials;

    private float intensity = 1;

    public bool isStartGame = false;

	// Use this for initialization
	void Start () {

		so = SocketObject.Instance;

		for (int i = 0; i < 15; i++) {
			for (int j = 0; j < 11; j++) {
				GameObject g = (GameObject)Instantiate(cube, new Vector3 (-7 + i, 0, 5 - j),Quaternion.identity);
				g.transform.parent = this.gameObject.transform;
				cubes.Add(g);
				SettingColor (g,Random.Range(0,5));
				g.GetComponent<cubeModel> ().setColorDef();
			}
		}
	}
	
	// Update is called once per frame
	void Update () {
		if (so.connecting && GameObject.FindGameObjectsWithTag ("ChangeColorArea_Start").Length == 0) {
			sphire = (GameObject)Instantiate (prefab,
				new Vector3 (0, 0, 0),
				Quaternion.identity);
			isStartGame = false;
		} else if(!so.connecting && GameObject.FindGameObjectsWithTag ("ChangeColorArea_Start").Length > 0){
			Destroy (sphire);
			foreach (GameObject c in cubes) {
				c.GetComponent<cubeModel> ().isStart = false;
			}
		}
	}

    public void startGame()
    {
        isStartGame = true;
    }

	void SettingColor(GameObject obs,int n){
		switch (n) {
		case 0:
			obs.GetComponent<Renderer> ().material.SetColor ("_EmissionColor",
				new Color (1, 0, 1));
			break;
		case 1:
			obs.GetComponent<Renderer> ().material.SetColor ("_EmissionColor",
				new Color (1, 0, 0));
			break;
		case 2:
			obs.GetComponent<Renderer> ().material.SetColor ("_EmissionColor",
				new Color (0, 0, 1));
			break;
		case 3:
			obs.GetComponent<Renderer> ().material.SetColor ("_EmissionColor",
				new Color (1, 1, 0));
			break;
		case 4:
			obs.GetComponent<Renderer> ().material.SetColor ("_EmissionColor",
				new Color (0, 1, 0));
			break;
		}
	}
}
