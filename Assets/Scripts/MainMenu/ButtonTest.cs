using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonTest : MonoBehaviour {

	static SocketObject so;
	static DataWorker dw;
    static GameManager gm;

    float time = 0;

	// Use this for initialization
	void Start () {
		so = SocketObject.Instance;
		dw = DataWorker.Instance;
        gm = GameManager.Instance;
		if (this.gameObject.name.Equals("NameInputField") && so.connecting) {
			GetComponent<InputField> ().text = SocketObject.Instance.name;
		}
	}

	public void connect(string s){
		if (!so.connecting && !s.Equals ("")) {
			so.name = s;
			so.Connect ();
		} else if (so.connecting && !s.Equals ("")) {
			so.name = s;
		} else if (so.connecting && s.Equals ("")) {
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
			if (gm._GameState.Value == GameState.ConnectionComp && so.id!="" && so.name!="") {
                dw.roomState = null;
                gm._GameState.Value = GameState.RoomSerching;
            } 
		}
	}

}