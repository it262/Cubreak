using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class DataWorker : SingletonMonoBehavior<DataWorker> {

	public int MAX =2;

	[SerializeField]GameObject PlayerPrefab,StagePrefab,cubesController,sphereController,TitleCamera,TitleText;

	public Dictionary<string,Vector3> posSync = new Dictionary<string,Vector3>();
	public Dictionary<string,Vector2> rotSync = new Dictionary<string,Vector2>();
	public Dictionary<string,bool> heatbeat = new Dictionary<string,bool>();
	public Dictionary<string,bool> pushSwitch = new Dictionary<string,bool>();
	public Queue<Dictionary<string,string>> chatQue = new Queue<Dictionary<string,string>>();
	public Queue<Dictionary<string,string>> roomQue = new Queue<Dictionary<string,string>>();
	public Queue<Dictionary<string,string>> hitQue = new Queue<Dictionary<string,string>>();
	public Queue<Dictionary<string,string>> elimQue = new Queue<Dictionary<string,string>>();

	public JSONObject roomState;
	public Dictionary<string,GameObject> players = new Dictionary<string,GameObject> ();
	public GameObject me;

	public bool playing = false;

	public Room myRoom;

	public bool searching,leady,wait;

	public int score;

	public GameObject InstanceStage,InstanceObsCon;
	public GameObject Menu;

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {

		if (playing) {

			Dictionary<string,string> d;

			//位置同期

			//回転同期

			if (hitQue.Count > 0) {
				d = hitQue.Dequeue ();
				GameObject g = players [d ["trg"].ToString ()];
				//g.GetComponent<Rigidbody> ().AddForce (new Vector3(float.Parse(d["x"]),float.Parse(d["y"]),float.Parse(d["z"])),ForceMode.Impulse);
				g.GetComponent<PlayerScript> ().impact = new Vector3 (float.Parse (d ["x"]), float.Parse (d ["y"]), float.Parse (d ["z"]));
			}

			if (elimQue.Count > 0) {
				d = elimQue.Dequeue ();
				if (d ["trg"].ToString ().Equals (GetComponent<SocketObject> ().id)) {
					//死亡
					MenuSetting ();
				} else {
					exclusion (d ["trg"].ToString ());
				}
			}

			/*
			//チャットデータ更新
			if (chatQue.Count > 0) {
				d = chatQue.Dequeue ();
				chatText.GetComponent<ChatScript> ().setMessage (d ["name"].ToString (), d ["message"].ToString ());
			}
			*/


			if ((MAX!=1) &&(players.Count == 1)) {
				score += 300;
				MenuSetting ();
			}


		} else {

			//ゲームスタート
			/*
			if (players.Count > 0 && startFlug && !playing) {
				if (players.Count == MAX) {
					startFlug = false;
					playing = true;
					myRoom.playing = true;
					Debug.Log ("Start[ルーム名：" + myRoom.roomName + "/人数：" + players.Count + "]");
					SceneManager.LoadScene (1);
				} else if (roomState == null) {
					GetComponent<SocketObject> ().EmitMessage ("getRooms", new Dictionary<string,string> ());
					Debug.Log ("Waiting...[" + players.Count + "/" + MAX + "]");
				}
			}*/
			if (myRoom != null && myRoom.cnt == MAX) {
				players.Clear ();
				foreach (KeyValuePair<string,string> member in myRoom.member) {
					players.Add (member.Key, null);
				}
				playing = true;
				Debug.Log ("Start[ルーム名：" + myRoom.roomName + "/人数：" + players.Count + "]");
				searching = false;
				GameSettings ();
			}
		}	
	}

	void GameSettings(){
		TitleText.SetActive (false);
		TitleCamera.SetActive (false);
		sphereController.SetActive (false);
		cubesController.GetComponent<CubesController> ().GameStart ();
		InstanceStage = (GameObject)Instantiate (StagePrefab,Vector3.zero,Quaternion.identity);
	}

	public void PlayerCreate(GameObject obCon,List<Vector2> pos){
		int cnt = 1;
		players.Clear ();
		foreach (KeyValuePair<string,string> data in myRoom.member) {
			Debug.Log ("Player"+(cnt++)+"：" + data.Key+"("+data.Value+")");
			obCon.GetComponent<ObstacleControllSync> ().state.Add (data.Key, "0");
			GameObject g = (GameObject)Instantiate (PlayerPrefab,new Vector3(pos[cnt-1].x,5,pos[cnt-1].y),Quaternion.identity);
			if (GetComponent<SocketObject> ().id.Equals (data.Key)) {
				TitleCamera.SetActive (false);
				g.GetComponent<PlayerScript> ().isPlayer = true;
				g.tag = "Player";
				//g.GetComponent<PlayerScript> ().damage = GameObject.Find ("Image");
				me = g;
			} else {
				g.GetComponent<PlayerScript> ().cam.SetActive(false);
				g.tag = "Others";
			}
			g.GetComponent<PlayerScript> ().id = data.Key;
			g.GetComponent<PlayerScript> ().name = data.Value;
			players.Add (data.Key, g);
		}
		leady = true;
	}

	void MenuSetting(){
		Destroy (InstanceStage);
		InstanceObsCon.GetComponent<ObstacleControllSync> ().DestroyAll ();
		Destroy (InstanceObsCon);
		foreach (GameObject data in players.Values) {
			Destroy (data);
		}
		DataClear ();
		TitleCamera.SetActive (true);
		TitleText.SetActive (true);
		sphereController.SetActive (true);
		cubesController.GetComponent<CubesController> ().CubeSetting ();
		Menu.SetActive (true);
	}

	public void disconnectUser(string id){
		var data = new Dictionary<string,string> ();
		data ["TYPE"] = "Dead";
		data ["id"] = id;
		GetComponent<SocketObject> ().EmitMessage ("ToOwnRoom", data);
		score += (players.Count > 2) ? 150 :(players.Count > 3) ? 50 : 0;
		MenuSetting ();
	}

	public void exclusion(string id){
		if (players.ContainsKey (id)) {
			Destroy (players [id]);
			players.Remove (id);
		}
	}

	public void DataClear(){
		//退室処理
		var data = new Dictionary<string,string> ();
		data ["to"] = "LEAVE";
		GetComponent<SocketObject>().EmitMessage ("Quick", data);
		Debug.Log ("[DataWorker]退室しました");

		playing = false;
		searching = false;
		leady = false;
		wait = false;
		myRoom = null;
		me = null;
		posSync.Clear ();
		rotSync.Clear ();
		heatbeat.Clear ();
		roomQue.Clear ();
		hitQue.Clear ();
		players.Clear ();

		Cursor.lockState = CursorLockMode.None;
		Cursor.SetCursor (null,Vector2.zero,CursorMode.ForceSoftware);

	}
}


