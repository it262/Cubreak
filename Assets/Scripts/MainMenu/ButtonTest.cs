using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonTest : MonoBehaviour {

	static SocketObject so;
	static DataWorker dw;

	public GameObject name,room;

	// Use this for initialization
	void Start () {
		if (so == null)
			so = SocketObject.Instance;
		if (dw == null)
			dw = DataWorker.Instance;
		if (this.gameObject.name.Equals("NameInputField") && so.connecting) {
			GetComponent<InputField> ().text = SocketObject.Instance.name;
		}
	}

	// Update is called once per frame
	void Update () {
		if (this.gameObject.name.Equals ("Start")){
			transform.Find ("Text").gameObject.GetComponent<Text> ().text = (dw.searching)? "Cansel":"Start";
			if (!so.connecting || name.GetComponent<Text> ().text.Equals ("")) {
				GetComponent<Button> ().interactable = false;
			} else {
				GetComponent<Button> ().interactable = true;
			}
		}
		if (this.gameObject.name.Equals ("NameInputField")) {
			GetComponent<InputField> ().interactable = !dw.searching;
		}
		
	}

	public void connect(){
		if (!so.connecting && !name.GetComponent<Text> ().text.Equals ("")) {
			so.name = name.GetComponent<Text> ().text;
			so.Connect ();
		} else if (so.connecting && !name.GetComponent<Text> ().text.Equals ("")) {
			so.name = name.GetComponent<Text> ().text;
		} else if (so.connecting && name.GetComponent<Text> ().text.Equals ("")) {
			so.Disconnection ();
		}
	}

	/*
	public void join(){
		if (so.GetComponent<SocketObject> ().connecting && so.GetComponent<SocketObject> ().id != null) {
			so.GetComponent<SocketObject> ().room = room.GetComponent<Text> ().text;
			so.GetComponent<SocketObject> ().joinRoom ();
		}
	}

	public void leave(){
		if (so.GetComponent<SocketObject> ().connected && so.GetComponent<SocketObject> ().id != null) {
			so.GetComponent<SocketObject> ().room = "";
			so.GetComponent<SocketObject> ().joinRoom ();
		}
	}

	public void joinButton(){
		if (so.GetComponent<SocketObject> ().connected && so.GetComponent<SocketObject> ().id != null) {
			so.GetComponent<SocketObject> ().room = room.GetComponent<Text> ().text;
			so.GetComponent<SocketObject> ().joinRoom ();
		}
	}

	public void reload(){
		so.GetComponent<SocketObject> ().EmitMessage ("GetRooms",new Dictionary<string,string>());
	}

	*/

	public void roomSearch(){
		if (so.connecting && so.id != null) {
			if (!dw.searching) {
				dw.searching = true;
				transform.Find ("Text").gameObject.GetComponent<Text> ().text = "Cansel";
			} else {
				dw.searching = false;
				dw.leady = false;
				dw.wait = false;

				//退室処理
				Debug.Log ("検索中止:[退室]"+dw.myRoom.roomName);
				var data = new Dictionary<string,string> ();
				data ["to"] = "LEAVE";
				so.EmitMessage ("Quick", data);
				dw.myRoom = null;

				transform.Find ("Text").gameObject.GetComponent<Text> ().text = "Start";
			}
		}
	}

}