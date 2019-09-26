using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UniRx;

public class DataWorker : SingletonMonoBehavior<DataWorker> {

    SocketObject so;
    static GameManager gm;

    CameraController cc;
    [SerializeField] GameObject startCamerapos;

	public int MAX = 2;

	[SerializeField]GameObject PlayerPrefab,StagePrefab,cubesController,sphereController,TitleCamera,TitleText,GameInstancePrefab,MenuStage,DebugPrefab,shutter,ResultUI;

	public GameObject GameInstance;

	public Dictionary<string,Vector3> posSync = new Dictionary<string,Vector3>();
	public Dictionary<string,Vector2> rotSync = new Dictionary<string,Vector2>();
	public Dictionary<string,bool> heatbeat = new Dictionary<string,bool>();
	public Dictionary<string,bool> pushSwitch = new Dictionary<string,bool>();
	public Queue<Dictionary<string,string>> chatQue = new Queue<Dictionary<string,string>>();
	public Queue<Dictionary<string,string>> roomQue = new Queue<Dictionary<string,string>>();
	public Queue<Dictionary<string,string>> hitQue = new Queue<Dictionary<string,string>>();
	public Queue<Dictionary<string,string>> elimQue = new Queue<Dictionary<string,string>>();

    public string RoomMaster;

	public JSONObject roomState;
	public Dictionary<string,GameObject> players = new Dictionary<string,GameObject> ();
	public GameObject me;

	public bool Exping = false;

    public bool watching = false;

	public Room myRoom;

	public int score;

	public GameObject InstanceStage,InstanceObsCon;
	public GameObject Enhanced;

    public Animator startCount;

    float time = 0;

    private bool die = false;

	// Use this for initialization
	void Start () {
        /*
		Object o = (Object)this;
		Debug.Log (o);

        cc = CameraController.Instance;
        cc.transform.parent = titleCamerapos.transform;
        */

        so = SocketObject.Instance;
        gm = GameManager.Instance;
        cc = CameraController.Instance;

        gm._GameState
            .DistinctUntilChanged()
            .Where(x => x == GameState.RoomSettingComp)
            .Subscribe(_ =>PlayerListSet());

        gm._GameState
            .DistinctUntilChanged()
            .Where(x => x == GameState.StartCount)
            .Subscribe(_ => startCount.SetTrigger("Start"));

        /*
        gm._GameState
            .DistinctUntilChanged()
            .Where(x => x == GameState.ConnectionComp)
            .Subscribe(_ => inactiveShutter());

        gm._GameState
            .DistinctUntilChanged()
            .Where(x => x == GameState.Menu)
            .Subscribe(_ => activeShutter());
            */

    }
	
	// Update is called once per frame
	void Update () {

        Debug.Log(GameManager.Instance._GameState.Value);

        if(GameManager.Instance._GameState.Value == GameState.StartCount){
            AnimatorStateInfo anim = startCount.GetCurrentAnimatorStateInfo(0);
            if (anim.fullPathHash == Animator.StringToHash("Base Layer.StartUI_ON") && anim.normalizedTime > 0.6f)
            {
                GameManager.Instance._GameState.Value = GameState.Playing;
            }
            return;
        }

		if (GameManager.Instance._GameState.Value == GameState.Playing) {

			Dictionary<string,string> d;

            if (hitQue.Count > 0)
            {
                d = hitQue.Dequeue();
                var g = players[d["trg"]];
                Vector3 start = new Vector3(float.Parse(d["startX"]), float.Parse(d["startY"]), float.Parse(d["startZ"]));
                Vector3 end = new Vector3(float.Parse(d["endX"]), float.Parse(d["endY"]), float.Parse(d["endZ"]));
                g.GetComponent<PlayerScript>().model.setImpactData(start, end, g);

            }

            if (elimQue.Count > 0) {
				d = elimQue.Dequeue ();
				exclusion (d ["trg"].ToString ());
			}

			Debug.Log (watching);

            if (watching && Input.GetKeyDown(KeyCode.Escape))
            {

                MenuSetting();
            }
            /*
			if (!Exping && (MAX!=1) &&(players.Count == 1)) {
				score += 300;
				MenuSetting ();
			}else if(MAX==1 && players.Count == 0)
            {
                MenuSetting();
            }
            */


        }
        else
        {
            if (gm._GameState.Value == GameState.ConnectionComp && Input.GetKeyDown(KeyCode.Escape))
            {
                so.Disconnection();
                so.id = "";
                so.name = "";
                gm._GameState.Value = GameState.Menu;
                RoomScript.Instance.removeMenu01Player();
            }
            else if (gm._GameState.Value == GameState.WaitingOtherPlayer && Input.GetKeyDown(KeyCode.Escape))
            {
                Debug.Log("検索中止:[退室]" + myRoom.roomName);
                var data = new Dictionary<string, string>();
                data["to"] = "LEAVE";
                so.EmitMessage("Quick", data);
                myRoom = null;
                gm._GameState.Value = GameState.ConnectionComp;
                RoomScript.Instance.removeMenu02Players();
            }
        }
	}

    void PlayerListSet()
    {
        Debug.Log("ルームエラー");
        if (myRoom != null && myRoom.cnt == MAX && players.Count == 0)
        {
            Debug.Log("ルームエラー");
            players.Clear();
            foreach (KeyValuePair<string, string> member in myRoom.member)
            {
                players.Add(member.Key, null);
            }
            //playing = true;
            Debug.Log("Start[ルーム名：" + myRoom.roomName + "/人数：" + players.Count + "]");
            GameSettings();
        }
        else
        {
            Debug.Log("ルームエラー");
        }
    }

	void GameSettings(){
		GameInstance = (GameObject)Instantiate (GameInstancePrefab);
		//TitleText.SetActive (false);
		//TitleText.GetComponent<TitleEffect>().active = true;
		TitleText.GetComponent<TitleEffect>().setActive_script(true);
        //TitleCamera.SetActive (false);
        //sphereController.SetActive (false);
        //cubesController.GetComponent<cubesController> ().GameStart ();
        //MenuStage.SetActive(false);
        //MenuStage.GetComponent<Animator>().SetBool("On", true);
        //shutter.SetActive(false);
		InstanceStage = (GameObject)Instantiate (StagePrefab,Vector3.zero,Quaternion.identity);
		InstanceStage.transform.parent = GameInstance.transform;
        Enhanced.SetActive(true);
        //Menu.SetActive(false);
    }

	public void PlayerCreate(GameObject obCon,List<Vector2> pos){
		int cnt = 1;
		players.Clear ();
		foreach (KeyValuePair<string,string> data in myRoom.member) {
			obCon.GetComponent<ObstacleControllSync> ().state.Add (data.Key, "0");
			GameObject g = (GameObject)Instantiate (PlayerPrefab,new Vector3(pos[cnt-1].x,5,pos[cnt-1].y),Quaternion.identity);
            PlayerScript ps = g.GetComponent<PlayerScript>();
            ps.pd = new PlayerData();
			if (GetComponent<SocketObject> ().id.Equals (data.Key)) {
				//TitleCamera.SetActive (false);
				ps.pd.isPlayer = true;
				g.tag = "Player";
				me = g;
			} else {
				ps.cam.SetActive(false);
                g.GetComponent<Attack>().enabled = false;
				g.tag = "Others";
			}
			ps.pd.id = data.Key;
			ps.pd.name = data.Value;
			g.transform.parent = GameInstance.transform;
			players.Add (data.Key, g);
            Debug.Log("Player" + (cnt++) + "：" + ps.pd.id + "(" + ps.pd.name + ")");
        }
        if(myRoom.member.Count == 1)
        {
            GameObject g = (GameObject)Instantiate(DebugPrefab, new Vector3(pos[0].x+5, 5, pos[0].y+5), Quaternion.identity);
            g.transform.localEulerAngles = Vector3.zero;
        }
        gm._GameState.Value = GameState.DefaultObstacleSetting;
    }

	void MenuSetting()
	{
		
		cc.transform.parent = cc.cam_menu1_pos.transform;
        ResultUI.GetComponent<Animator>().SetBool("On", false);
        Enhanced.SetActive(false);
        //cc.transform.parent = titleCamerapos.transform;
        Destroy (InstanceStage);
		InstanceObsCon.GetComponent<ObstacleControllSync> ().DestroyAll ();
		Destroy (InstanceObsCon);
		foreach (GameObject data in players.Values) {
			Destroy (data);
		}
		DataClear ();
        //MenuStage.SetActive(true);
        //CameraController.Instance.transform.parent = null;
        //MenuStage.GetComponent<Animator>().SetBool("On", false);
        //TitleCamera.SetActive (true);
        //TitleText.SetActive (true);
        TitleText.GetComponent<TitleEffect>().setActive_script(false);
		//sphereController.SetActive (true);
		//cubesController.SetActive (true);
		//cubesController.GetComponent<cubesController> ().CubeSetting ();
		//Menu.SetActive (true);
	}

	public void disconnectUser(string id){
		var data = new Dictionary<string,string> ();
		data ["TYPE"] = "Dead";
		data ["id"] = id;
		GetComponent<SocketObject> ().EmitMessage ("ToOwnRoom", data);
		score += (players.Count > 2) ? 150 :(players.Count > 3) ? 50 : 0;
	}

	public void exclusion(string id){
        if (players.ContainsKey(id))
        {
            if (id.Equals(me.GetComponent<PlayerScript>().pd.id)) {
                //CameraController.Instance.transform.parent = startCamerapos.transform;
                //Camera.main.transform.parent = null;
                watching = true;
                CameraController.Instance.transform.parent = InstanceStage.GetComponent<Stage>().CamPos.transform;
                ResultUI.GetComponent<ResultIndicater>().setRanks(players.Count.ToString());
                ResultUI.GetComponent<ResultIndicater>().setScore(300 - players.Count*50);
                ResultUI.GetComponent<Animator>().SetBool("On", true);
                Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
            }
            else if(players.Count == 2)
            {
                watching = true;
                CameraController.Instance.transform.parent = InstanceStage.GetComponent<Stage>().CamPos.transform;
                ResultUI.GetComponent<ResultIndicater>().setRanks("1");
                ResultUI.GetComponent<ResultIndicater>().setScore(300);
                ResultUI.GetComponent<Animator>().SetBool("On", true);
            }
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

		Destroy (GameInstance);

        gm._GameState.Value = GameState.ConnectionComp;

        RoomMaster = null;

        watching = false;
		Exping = false;
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

    public void setPdImpact(string s)
    {
        if(me != null)
        me.GetComponent<PlayerScript>().pd.impact_Restriction = float.Parse(s);
    }

    public void setPdMoveSpeed(string s)
    {
        if (me != null)
            me.GetComponent<PlayerScript>().pd.moveSpeedRate = float.Parse(s);
    }

    public void setPdAttackSpeed(string s)
    {
        if (me != null)
            me.GetComponent<PlayerScript>().pd.attackSpeedRate = float.Parse(s);
    }

    void activeShutter()
    {
        shutter.SetActive(true);
        shutter.GetComponent<Animator>().SetBool("On", false);
    }

    void inactiveShutter()
    {
        shutter.GetComponent<Animator>().SetBool("On", true);
    }

    public void menuInvisible()
    {
        //MenuStage.GetComponent<Animator>().SetBool("On", true);
        //TitleText.GetComponent<TitleEffect>().active = true;
        //CameraController.Instance.transform.parent = startCamerapos.transform;
    }

}


