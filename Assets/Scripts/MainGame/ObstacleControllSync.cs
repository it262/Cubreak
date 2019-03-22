using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleControllSync : MonoBehaviour {

	static SocketObject so;
	static DataWorker dw;

	public GameObject obstaclePrefab,SummonPref;

	public Material emit;

	public int xTarget;
	public int zTarget;
	public Vector2 targetSection;
	public GameObject stage;

	GameObject[] tagObjects;

	public float speed;

	private int _Y = 30;

	bool FPComplete = false;
	bool send = false;

	public Dictionary<string,string> first = new Dictionary<string,string>();
	public Dictionary<string,string> obs = new Dictionary<string,string>();
	public Dictionary<string,string> state = new Dictionary<string,string>();//0:完了 1:処理中

	public Queue<Dictionary<string,string>> victim = new Queue<Dictionary<string,string>>();

	public Dictionary<int,GameObject> obstacle = new Dictionary<int,GameObject>();

	int idCounter = 0;

	public float SendObsInterval = 0.5f;

	// Use this for initialization
	void Start(){
		// -> FirstProcessing()
		so = SocketObject.Instance;
		dw = DataWorker.Instance;
		StartCoroutine("SendObsData");
	}

	// Update is called once per frame
	void Update () {

		if (stage == null)
			return;

		if (stage.GetComponent<Stage>().debug)
			return;

		if (dw.leady && !FPComplete) {
			FirstProcessing ();
			return;
		}

		//obstacleがフィールド上になかったら新しいobstacleを召喚する。
		ObsUpdate();
	}

	void FirstProcessing(){
		//obstacleの初期配置
		if(!send){
			if (isMaster()) {
				foreach (string check in state.Values) {
					if (check.Equals ("1"))
						return;
				}
				stateSync (1);
				var data = new Dictionary<string,string> ();
				data ["TYPE"] = "FirstObs";
				for (int i = 0; i < 10; i++) {
					xTarget = (int)Random.Range (0, stage.GetComponent<Stage> ().xSection);
					zTarget = (int)Random.Range (0, stage.GetComponent<Stage> ().zSection);
					targetSection = stage.GetComponent<Stage> ().TargetSection (xTarget, zTarget);

					data ["x" + i] = targetSection [0].ToString ();
					data ["z" + i] = targetSection [1].ToString ();
					data ["color" + i] = ((int)Random.Range (0,5)).ToString();//0:normal 1:attack 2:diffence 3:speed 4:heal

				}
				so.EmitMessage ("ToOwnRoom", data);
				Debug.Log ("オブジェクト初期位置送信完了");
				send = true;
				return;
			}
		}
		if(first.Count > 0){
			
			stateSync(1);

			for (int i = 0; i < 10; i++) {
				string x = "x"+i;
				string z = "z"+i;
				GameObject obs = (GameObject)Instantiate (obstaclePrefab,
					new Vector3 (float.Parse(first[x]), stage.transform.position.y+1f, float.Parse(first[z])),
					                Quaternion.identity);
				obs.gameObject.tag = "fallenObstacle";
				obstacle.Add (idCounter, obs);
				obs.GetComponent<ObsUpdate> ().id = idCounter++;
				Color color = SettingColor (obs, int.Parse (first ["color" + i]));
				GameObject summon = (GameObject)Instantiate (SummonPref, obs.transform.position, Quaternion.identity);
				summon.GetComponent<ParticleSystem> ().startColor = color;
				Destroy (summon, 3f);

			}

			first.Clear ();
			stateSync(0);
			FPComplete = true;
			send = false;
			Debug.Log ("初期オブジェクト設置完了");
		}

	}

	void ObsUpdate(){
		if (victim.Count > 0) {
			Debug.Log ("Receive:"+victim.Peek()["n"] + "破壊");
			obstacle [int.Parse(victim.Dequeue() ["n"])].GetComponent<ObsUpdate>().Destroy();
		}

		/*
		if (isnt_There ("Obstacle")) {
			stateSync(0);
		}
		/*
		if (!send) {
			if (isMaster()) {
				if (isnt_There ("Obstacle")) {
					if (!dw.players.Count.Equals (state.Count)) {
						List<string> keys = new List<string> ();
						foreach (string id in state.Keys) {
							keys.Add (id);
						}
						foreach (string id in keys) {
							if (!dw.players.ContainsKey (id)) {
								state.Remove (id);
							}
						}
					}
					foreach (KeyValuePair<string,string> check in state) {
						Debug.Log (check.Key + ":" + (check.Value.Equals ("0") ? "完了" : "設置中"));
						if (check.Value.Equals ("1")) {
							Debug.Log ("他プレイヤーの設置完了待ち...");
							return;
						}
					}
					//イテレータ SendObsData
					send = true;
					Debug.Log ("送信完了");
				}
				return;
			}
		}
		*/
		if (obs.Count > 0) {
			//if (isnt_There ("Obstacle")) {
			//stateSync(1);
			if (obs.ContainsKey ("xTarget" + idCounter.ToString ())) {
				int xT = int.Parse (obs ["xTarget" + idCounter.ToString ()]);
				int zT = int.Parse (obs ["zTarget" + idCounter.ToString ()]);
				int xW = int.Parse (obs ["x_width" + idCounter.ToString ()]);
				int yW = int.Parse (obs ["y_width" + idCounter.ToString ()]);
				int zW = int.Parse (obs ["z_width" + idCounter.ToString ()]);
				string colorData = obs ["color" + idCounter.ToString ()];

				obs.Remove (obs ["xTarget" + idCounter.ToString ()]);
				obs.Remove (obs ["zTarget" + idCounter.ToString ()]);
				obs.Remove (obs ["x_width" + idCounter.ToString ()]);
				obs.Remove (obs ["y_width" + idCounter.ToString ()]);
				obs.Remove (obs ["z_width" + idCounter.ToString ()]);
				obs.Remove (obs ["color" + idCounter.ToString ()]);

				targetSection = stage.GetComponent<Stage> ().TargetSection (xT, zT);
				int x_width = xW;
				int y_width = yW;
				int z_width = zW;
				char[] c = colorData.ToCharArray ();
				int cnt = 0;
				for (int i = 0; i < x_width; i++) {
					for (int j = 0; j < z_width; j++) {
						for (int k = 0; k < y_width; k++) {
							GameObject obs = (GameObject)Instantiate (obstaclePrefab,
								                 new Vector3 (targetSection.x + i, _Y - j, targetSection.y/*z*/ - k),
								                 Quaternion.identity);
							obstacle.Add (idCounter, obs);
							obs.GetComponent<ObsUpdate> ().id = idCounter++;
							Color color = SettingColor (obs, int.Parse (c [cnt++].ToString ()));
							GameObject summon = (GameObject)Instantiate (SummonPref, obs.transform.position, Quaternion.identity);
							summon.GetComponent<ParticleSystem> ().startColor = color;
							Destroy (summon, 3f);
						}
					}
				}
				//obs.Clear ();
				send = false;
				//}
				//return;
			}
		}
	}

	bool isMaster(){
		bool master = false;
		foreach (string key in dw.players.Keys) {
			if (key.Equals (so.id)) {
				master = true;
			}
			break;
		}
		return master;
	}

	void stateSync(int s){
		var data = new Dictionary<string,string> ();
		data ["TYPE"] = "StateUpdate";
		data ["state"] = s.ToString();
		so.EmitMessage ("ToOwnRoom", data);
	}


	//targetTagが"いない"時にtrueを返す。フィールド上にobstacleが残っているかどうかを判定するのに使う。
	bool isnt_There(string targetTag) {
		if (!FPComplete)
			return false;
		tagObjects = GameObject.FindGameObjectsWithTag(targetTag);
		if (tagObjects.Length == 0){
			return true;
		}else {
			return false;
		}
	}

	Color SettingColor(GameObject obs,int n){
		switch (n) {
		case 0:
			obs.GetComponent<Renderer> ().material.SetColor ("_EmissionColor",
				new Color (1, 0, 1));
			return new Color (1,0,1);
			break;
		case 1:
			obs.GetComponent<Renderer> ().material.SetColor ("_EmissionColor",
				new Color (1, 0, 0));
			return new Color (1,0,0);
			break;
		case 2:
			obs.GetComponent<Renderer> ().material.SetColor ("_EmissionColor",
				new Color (0, 0, 1));
			return new Color (0,0,1);
			break;
		case 3:
			obs.GetComponent<Renderer> ().material.SetColor ("_EmissionColor",
				new Color (1, 1, 0));
			return new Color (1,1,0);
			break;
		case 4:
			obs.GetComponent<Renderer> ().material.SetColor ("_EmissionColor",
				new Color (0, 1, 0));
			return new Color (0,1,0);
			break;
		}
		return new Color (0,0,0);
	}

	public void DestroyAll(){
		foreach (GameObject obj in obstacle.Values) {
			Destroy (obj);
		}
	}

	IEnumerator SendObsData(){
			while (true) {
			if (isMaster () && FPComplete) {
				var data = new Dictionary<string,string> ();
				data ["TYPE"] = "Obs";
				data ["xTarget"+idCounter.ToString()] = ((int)Random.Range (0, stage.GetComponent<Stage> ().xSection)).ToString ();
				data ["zTarget"+idCounter.ToString()] = ((int)Random.Range (0, stage.GetComponent<Stage> ().zSection)).ToString ();
				int x_width = (int)Random.Range (1, 3);
				int y_width = (int)Random.Range (1, 2);
				int z_width = (int)Random.Range (1, 3);
				data ["x_width"+idCounter.ToString()] = x_width.ToString ();
				data ["y_width"+idCounter.ToString()] = y_width.ToString ();
				data ["z_width"+idCounter.ToString()] = z_width.ToString ();
				int max = x_width * y_width * z_width;
				string s = "";
				for (int i = 0; i < max; i++) {
					s += ((int)Random.Range (0, 5)).ToString ();
				}
				data ["color"+idCounter.ToString()] = s;
				so.EmitMessage ("ToOwnRoom", data);
			}
			yield return new WaitForSeconds (SendObsInterval);
		}
	}
}
