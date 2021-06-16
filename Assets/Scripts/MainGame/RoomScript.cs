using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using System;
using UniRx;

public class RoomScript : SingletonMonoBehavior<RoomScript> {

	static SocketObject so;
	static DataWorker dw;
    static GameManager gm;

	[SerializeField]GameObject content,pref;
    [SerializeField]GameObject playerpref;
    [SerializeField] GameObject[] positions = new GameObject[4];
    GameObject[] playerInstance = new GameObject[4];

    float time = 0;
    bool wait = false;

	//List<Room> room = new List<Room>();

	// Use this for initialization
	void Start () {
		so = SocketObject.Instance;
		dw = DataWorker.Instance;
        gm = GameManager.Instance;

		//StartCoroutine ("RequestRoomData");
        /*
        gm._GameState
            .DistinctUntilChanged()
            .Where(x => x == GameState.ConnectionComp)
            .Subscribe(_ => setMenu01Player());
            */

        gm._GameState
            .DistinctUntilChanged()
            .Where(x => x == GameState.CheckRoomData)
            .Subscribe(_ => QuickStart());

        gm._GameState
            .DistinctUntilChanged()
            .Where(x => x == GameState.RoomDataUpdate)
            .Subscribe(_ => RoomDataCheck());
    }

    // Update is called once per frame
    void FixedUpdate() {
        //QuickStart ();
        time = (time<10)? time+Time.deltaTime:0;

        if (wait && time > 3)
        {
            time = 0;
            wait = false;
            Debug.Log("comp");
            //removeMenuPlayers();
            gm._GameState.Value = GameState.RoomSettingComp;
        }

        if (time > 1 && gm._GameState.Value == GameState.RoomSerching)
        {
            time = 0;
            Debug.Log("ルーム検索中...");
            var data = new Dictionary<string, string>();
            data["to"] = "";
            so.EmitMessage("Quick", data);
        }
    }

    void QuickStart()
    {

        /*
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
        */
        var state = dw._roomState;
        dw._roomState = null;

        //入室可能ルームが見つかったかどうか
        bool hit = false;
        for (int i = 0; i < state.list.Count; i++)
        {
            if (!so.id.Equals(state.keys[i]))
            {
                string roomName = state.keys[i].ToString();
                int cnt = int.Parse(state.list[i]["length"].ToString());
                if (roomName.Contains("[ROOM]"))
                {
                    Debug.Log(roomName);
                    if (dw._max > cnt)
                    {
                        if (state.list[i]["playing"].ToString().Equals("false"))
                        {
                            Debug.Log("Room:[" + roomName + "] " + cnt + "/" + dw._max);
                            //ルーム入室リクエスト送信（未確定）
                            var data = new Dictionary<string, string>();
                            data["to"] = roomName;
                            data["name"] = so.name;
                            data["max"] = dw._max.ToString();
                            so.EmitMessage("Quick", data);
                            hit = true;
                            break;
                        }
                    }
                }
            }
        }
        //もし入室可能な部屋が見つからなかったら新しい部屋を作る
        if (!hit)
        {
            var data = new Dictionary<string, string>();
            data["to"] = "JOIN";
            data["name"] = so.name;
            Debug.Log(dw._max);
            data["max"] = dw._max.ToString();
            so.EmitMessage("Quick", data);
        }

        gm._GameState.Value = GameState.WaitingOtherPlayer;

    }

    void RoomDataCheck()
    {
        Debug.Log("check");
        if(dw._myRoom.cnt == dw._max)
        {
            time = 0;
            wait = true;
            //removeMenu02Players();
            //setMenu02Players();
            dw.MenuInvisible();
            /*
            time = 0;
            Debug.Log("comp");
            if (time > 3)
            {
                removeMenuPlayers();
                gm._GameState.Value = GameState.RoomSettingComp;
            }
            */
        }
        else
        {
            Debug.Log("wait");
            //removeMenu02Players();
            //setMenu02Players();
            gm._GameState.Value = GameState.WaitingOtherPlayer;
            Debug.Log("ルームメンバー募集中...");
        }
    }

    /*
	IEnumerator RequestRoomData(){
		while (true) {
			if (gm._GameState.Value == GameState.RoomSerching) {
				Debug.Log("ルーム検索中...");
				var data = new Dictionary<string,string> ();
				data ["to"] = "";
				so.EmitMessage ("Quick", data);
			}
				yield return new WaitForSeconds (1f);
		}
	}
    */

    void setMenu01Player()
    {
        if (playerInstance[0] == null)
        {
            GameObject g = Instantiate(playerpref);
            playerInstance[0] = g;
            g.transform.parent = positions[0].transform;
            g.transform.localPosition = new Vector3(0, 0, 0);
            g.transform.localEulerAngles = new Vector3(0, 180, 0);
            //g.GetComponent<Animator>().SetTrigger("On");
        }
    }

    void setMenu02Players()
    {
        for (int i=1; i<dw._myRoom.member.Count; i++){
            GameObject g = Instantiate(playerpref);
            playerInstance[i] = g;
            g.transform.parent = positions[i].transform;
            g.transform.localPosition = new Vector3(0, 0, 0);
            g.transform.localEulerAngles = new Vector3(0, 180, 0);
        }
    }

    public void removeMenu01Player()
    {
        Destroy(playerInstance[0]);
        playerInstance = new GameObject[4];
    }

    public void removeMenu02Players()
    {
        for (int i = 1; i < playerInstance.Length; i++)
        {
            if (playerInstance[i] != null)
            {
                Destroy(playerInstance[i]);
                playerInstance[i] = null;
            }
        }
    }

    void removeMenuPlayers()
    {
        foreach (GameObject g in playerInstance)
        {
            Destroy(g);
        }
        playerInstance = new GameObject[4];
    }

}