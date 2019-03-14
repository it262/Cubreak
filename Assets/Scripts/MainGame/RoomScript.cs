using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using System;

public class RoomScript : MonoBehaviour {

	static SocketObject so;
	static DataWorker dw;

	[SerializeField]GameObject content,pref;

	List<Room> room = new List<Room>();

	// Use this for initialization
	void Start () {
		so = SocketObject.Instance;
		dw = DataWorker.Instance;
		StartCoroutine ("RequestRoomData");
	}
	
	// Update is called once per frame
	void Update () {
		/*
		stateUpdate ();
		roomCheck ();
		RequestName ();
		Setting ();
		*/
		QuickStart ();
	}

	/*

	void stateUpdate(){
		if (so != null) {
			if (dw.roomQue.Count > 0) {
				var d = dw.roomQue.Dequeue();

				//退室処理
				if (!d ["leave"].ToString ().Equals ("")) {
					foreach (Room r in room) {
						if (r.roomName.Equals (d ["leave"].ToString ())) {
							r.leaveRoom (d ["id"].ToString ());
							//自分の入っているルームを無しにする
							current = null;
							break;
						}
					}
				}

				//入室処理
				if (!d ["room"].ToString ().Equals ("")) {
					bool flug = false;
					foreach (Room r in room) {
						if (r.roomName.Equals (d ["room"].ToString ())) {
							r.joinRoom (d ["id"],d["name"]);
							//自分の入っているルームを格納
							if (d ["id"].ToString ().Equals (so.id)) {
								current = r;
							}
							flug = true;
							break;
						}
					}
					//該当ルームが無ければ新しく作る
					if (!flug) {
						var r = createRoom (d ["id"],d["name"],d ["room"]);
						//自分の入っているルームを格納
						if (d ["id"].ToString ().Equals (so.id)) {
							current = r;
						}
					}
				}
				//ルーム情報更新
				so.EmitMessage ("GetRooms",new Dictionary<string,string>());
			}
		}
	}

	//ルームUI生成
	Room createRoom(string id,string name,string roomName){
		GameObject g = (GameObject)Instantiate (pref);
		g.transform.SetParent(content.transform,false);
		var r = new Room (g, roomName);
		r.joinRoom (id,name);
		room.Add (r);
		return r;
	}


	//ルームデータ更新
	void roomCheck(){
		if (so != null) {
			var state = dw.roomState;
			if (state != null) {
				
				foreach (Transform child in content.transform) {
					Destroy (child.gameObject);
				}
				room.Clear ();

				for (int i = 0; i < state.list.Count; i++) {
					if (!so.GetComponent<SocketObject>().id.Equals(state.keys [i])){
						string roomName = state.keys [i].ToString();
						List<string> member = state.list [i] ["sockets"].keys;
						int cnt = int.Parse(state.list [i] ["length"].ToString());
						if (!roomName.Equals (member [0])) {
							if (state.list [i] ["playing"].ToString ().Equals("false")) {
								GameObject g = (GameObject)Instantiate (pref);
								g.transform.SetParent (content.transform, false);
								Room r = new Room (g, roomName);
								r.member.Clear ();
								foreach (string id in member) {
									r.member.Add (id, null);
								}
								r.cnt = cnt;
								r.updateState ();
								room.Add (r);
								foreach (string str in member) {
									if (so.id.Equals (str)) {
										current = r;
										break;
									}
								}
							}
						}
					}
				}

				dw.roomState = null;
				Debug.Log ("[OK]ルーム更新が完了しました");
			}
		}
	}

	void Setting(){
		if (dw.startFlug) {
			if (current != null) {
				if (!dw.players.Keys.SequenceEqual (current.member.Keys)) {
					Debug.Log ("[OK]Roomメンバーの情報を取得しました");
					dw.players.Clear ();
					foreach (string id in current.member.Keys) {
						dw.players.Add (id, null);
					}
					dw.myRoom = current;
				}
			} else {
				Debug.Log ("[ERROR]ルームがnullです");
				//socket.GetComponent<DataWorker> ().start = false;
			}
		}
	}

	void RequestName(){
		for (int i = 0; i < room.Count; i++) {
			foreach (KeyValuePair<string,string> attach in dw.attachName) {
				if (room [i].member.ContainsKey (attach.Key)) {
					room [i].member [attach.Key] = attach.Value;
					Debug.Log ("PongName");
				}
			}
			var data = new Dictionary<string,string> ();
			foreach (KeyValuePair<string,string> m in room[i].member) {
				if (m.Value == null) {
					data.Clear ();
					data ["req"] = m.Key;
					so.EmitMessage ("PingName", data);
					Debug.Log ("PingName");
				}
			}
			room [i].updateState ();
		}
		dw.attachName.Clear ();
	}

*/

	void QuickStart(){

		if (dw.myRoom != null) {
			Debug.Log ("入室中ルーム:" + dw.myRoom.roomName);
		} else {
			Debug.Log ("ルーム情報無し");
		}

		//検索中じゃなかったら
		if (!dw.searching) {
			Debug.Log ("待機中...");
			return;
		}
		if (dw.leady) {
			Debug.Log("ルームメンバー募集中...");
			return;
		}
		if (dw.wait) {
			Debug.Log("[ルーム更新]サーバからの応答待ち...");
			return;
		}
		var state = dw.roomState;
		dw.roomState = null;
		if (dw.myRoom == null) {
			if (state != null) {
				//入室可能ルームが見つかったかどうか
				bool hit = false;
				for (int i = 0; i < state.list.Count; i++) {
					if (!so.id.Equals (state.keys [i])) {
						string roomName = state.keys [i].ToString ();
						int cnt = int.Parse (state.list [i] ["length"].ToString ());
						if (roomName.Contains ("[ROOM]")) {
							Debug.Log (roomName);
							if (dw.MAX > cnt) {
								if (state.list [i] ["playing"].ToString ().Equals ("false")) {
									Debug.Log ("Room:[" + roomName + "] " + cnt + "/" + dw.MAX);
									//ルーム入室リクエスト送信（未確定）
									var data = new Dictionary<string,string> ();
									data ["to"] = roomName;
									data ["name"] = so.name;
									so.EmitMessage ("Quick", data);
									hit = true;
									break;
								}
							}
						}
					}
				}
				//もし入室可能な部屋が見つからなかったら新しい部屋を作る
				if (!hit) {
					var data = new Dictionary<string,string> ();
					data ["to"] = "JOIN";
					data ["name"] = so.name;
					so.EmitMessage ("Quick", data);
				}
				dw.wait = true;
				state = null;
			} else {
				//コルーチン
			}
		} else {
			if (state != null) {
				dw.myRoom = null;
			}
		}
	}

	IEnumerator RequestRoomData(){
		while (true) {
			if (dw.searching && !dw.leady && !dw.wait && dw.roomState == null) {
				Debug.Log("ルーム検索中...");
				var data = new Dictionary<string,string> ();
				data ["to"] = "";
				so.EmitMessage ("Quick", data);
			}
				yield return new WaitForSeconds (1f);
		}
	}

}