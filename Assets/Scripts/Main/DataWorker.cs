using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UniRx;

public class DataWorker : SingletonMonoBehavior<DataWorker>
{
    [SerializeField] private GameObject _startCamerapos = default;
    [SerializeField] private GameObject _playerPrefab=default;
    [SerializeField] private GameObject _stagePrefab = default;
    [SerializeField] private GameObject _cubesController = default;
    [SerializeField] private GameObject _sphereController = default;
    [SerializeField] private GameObject _titleCamera = default;
    [SerializeField] private GameObject _titleText = default;
    [SerializeField] private GameObject _ga_meInstancePrefab = default;
    [SerializeField] private GameObject __menuStage = default;
    [SerializeField] private GameObject _debugPrefab = default;
    [SerializeField] private GameObject _shutter = default;
    [SerializeField] private GameObject _resultUI = default;

    internal int _max = 2;
    internal GameObject _gameInstance;
    internal Dictionary<string, Vector3> posSync = new Dictionary<string, Vector3>();
    internal Dictionary<string, Vector2> rotSync = new Dictionary<string, Vector2>();
    internal Dictionary<string, bool> heatbeat = new Dictionary<string, bool>();
    internal Dictionary<string, bool> pushSwitch = new Dictionary<string, bool>();
    internal Queue<Dictionary<string, string>> chatQue = new Queue<Dictionary<string, string>>();
    internal Queue<Dictionary<string, string>> roomQue = new Queue<Dictionary<string, string>>();
    internal Queue<Dictionary<string, string>> hitQue = new Queue<Dictionary<string, string>>();
    internal Queue<Dictionary<string, string>> elimQue = new Queue<Dictionary<string, string>>();
    internal string _roomMaster;
    internal JSONObject _roomState;
    internal Dictionary<string, GameObject> _players = new Dictionary<string, GameObject>();
    internal GameObject _me;
    internal bool _exping = false;
    internal bool _watching = false;
    internal Room _myRoom;
    internal int _score;
    internal GameObject _instanceStage, _instanceObsCon;
    internal GameObject _enhanced;
    internal Animator _startCount;

    private SocketObject _socketObject;
    private static GameManager _gameManager;
    private CameraController _cameraController;
    private float _time = 0f;
    private bool _die = false;

    // Use this for initialization
    void Start()
    {
        /*
		Object o = (Object)this;
		Debug.Log (o);

        _cameraController = CameraController.Instance;
        _cameraController.transform.parent = titleCamerapos.transform;
        */

        _socketObject = SocketObject.Instance;
        _gameManager = GameManager.Instance;
        _cameraController = CameraController.Instance;

        _gameManager._GameState
            .DistinctUntilChanged()
            .Where(x => x == GameState.RoomSettingComp)
            .Subscribe(_ => PlayerListSet());

        _gameManager._GameState
            .DistinctUntilChanged()
            .Where(x => x == GameState.StartCount)
            .Subscribe(_ => _startCount.SetTrigger("Start"));

        /*
        _gameManager._GameState
            .DistinctUntilChanged()
            .Where(x => x == GameState.ConnectionComp)
            .Subscribe(_ => inactive__shutter());

        _gameManager._GameState
            .DistinctUntilChanged()
            .Where(x => x == GameState.Menu)
            .Subscribe(_ => active__shutter());
            */

    }

    // Update is called once per fra_me
    void Update()
    {

        Debug.Log(GameManager.Instance._GameState.Value);

        if (GameManager.Instance._GameState.Value == GameState.StartCount)
        {
            AnimatorStateInfo anim = _startCount.GetCurrentAnimatorStateInfo(0);
            if (anim.fullPathHash == Animator.StringToHash("Base Layer.StartUI_ON") && anim.normalizedTime > 0.6f)
            {
                GameManager.Instance._GameState.Value = GameState.Playing;
            }
            return;
        }

        if (GameManager.Instance._GameState.Value == GameState.Playing)
        {

            Dictionary<string, string> d;

            if (hitQue.Count > 0)
            {
                d = hitQue.Dequeue();
                var g = _players[d["trg"]];
                Vector3 start = new Vector3(float.Parse(d["startX"]), float.Parse(d["startY"]), float.Parse(d["startZ"]));
                Vector3 end = new Vector3(float.Parse(d["endX"]), float.Parse(d["endY"]), float.Parse(d["endZ"]));
                g.GetComponent<PlayerScript>()._transmesh.SetImpactData(start, end, g);

            }

            if (elimQue.Count > 0)
            {
                d = elimQue.Dequeue();
                Exclusion(d["trg"].ToString());
            }

            Debug.Log(_watching);

            if (_watching && Input.GetKeyDown(KeyCode.Escape))
            {

                MenuSetting();
            }
            /*
			if (!Exping && (_max!=1) &&(_players.Count == 1)) {
				score += 300;
				MenuSetting ();
			}else if(_max==1 && _players.Count == 0)
            {
                MenuSetting();
            }
            */


        }
        else
        {
            if (_gameManager._GameState.Value == GameState.ConnectionComp && Input.GetKeyDown(KeyCode.Escape))
            {
                _socketObject.Disconnection();
                _socketObject.id = "";
                _socketObject.name = "";
                _gameManager._GameState.Value = GameState.Menu;
                RoomScript.Instance.removeMenu01Player();
            }
            else if (_gameManager._GameState.Value == GameState.WaitingOtherPlayer && Input.GetKeyDown(KeyCode.Escape))
            {
                Debug.Log("検索中止:[退室]" + _myRoom.roomName);
                var data = new Dictionary<string, string>();
                data["to"] = "LEAVE";
                _socketObject.EmitMessage("Quick", data);
                _myRoom = null;
                _gameManager._GameState.Value = GameState.ConnectionComp;
                RoomScript.Instance.removeMenu02Players();
            }
        }
    }

    internal void PlayerCreate(GameObject obCon, List<Vector2> pos)
    {
        int cnt = 1;
        _players.Clear();
        foreach (KeyValuePair<string, string> data in _myRoom.member)
        {
            obCon.GetComponent<ObstacleControllSync>().state.Add(data.Key, "0");
            GameObject g = (GameObject)Instantiate(_playerPrefab, new Vector3(pos[cnt - 1].x, 5, pos[cnt - 1].y), Quaternion.identity);
            PlayerScript ps = g.GetComponent<PlayerScript>();
            ps.PlayerData = new PlayerData();
            if (GetComponent<SocketObject>().id.Equals(data.Key))
            {
                //_titleCamera.SetActive (false);
                ps.PlayerData._isPlayer = true;
                g.tag = "Player";
                _me = g;
            }
            else
            {
                ps._camera.SetActive(false);
                g.GetComponent<Attack>().enabled = false;
                g.tag = "Others";
            }
            ps.PlayerData._id = data.Key;
            ps.PlayerData._name = data.Value;
            g.transform.parent = _gameInstance.transform;
            _players.Add(data.Key, g);
            Debug.Log("Player" + (cnt++) + "：" + ps.PlayerData._id + "(" + ps.PlayerData._name + ")");
        }
        if (_myRoom.member.Count == 1)
        {
            GameObject g = (GameObject)Instantiate(_debugPrefab, new Vector3(pos[0].x + 5, 5, pos[0].y + 5), Quaternion.identity);
            g.transform.localEulerAngles = Vector3.zero;
        }
        _gameManager._GameState.Value = GameState.DefaultObstacleSetting;
    }

    internal void DisconnectUser(string id)
    {
        var data = new Dictionary<string, string>();
        data["TYPE"] = "Dead";
        data["id"] = id;
        GetComponent<SocketObject>().EmitMessage("ToOwnRoom", data);
        _score += (_players.Count > 2) ? 150 : (_players.Count > 3) ? 50 : 0;
    }

    internal void Exclusion(string id)
    {
        if (_players.ContainsKey(id))
        {
            if (id.Equals(_me.GetComponent<PlayerScript>().PlayerData._id))
            {
                //CameraController.Instance.transform.parent = _startCamerapos.transform;
                //Camera.main.transform.parent = null;
                _watching = true;
                CameraController.Instance.transform.parent = _instanceStage.GetComponent<Stage>().CamPos.transform;
                _resultUI.GetComponent<ResultIndicater>().setRanks(_players.Count.ToString());
                _resultUI.GetComponent<ResultIndicater>().setScore(300 - _players.Count * 50);
                _resultUI.GetComponent<Animator>().SetBool("On", true);
                Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
            }
            else if (_players.Count == 2)
            {
                _watching = true;
                CameraController.Instance.transform.parent = _instanceStage.GetComponent<Stage>().CamPos.transform;
                _resultUI.GetComponent<ResultIndicater>().setRanks("1");
                _resultUI.GetComponent<ResultIndicater>().setScore(300);
                _resultUI.GetComponent<Animator>().SetBool("On", true);
            }
            Destroy(_players[id]);
            _players.Remove(id);
        }
    }

    internal void DataClear()
    {
        //退室処理
        var data = new Dictionary<string, string>();
        data["to"] = "LEAVE";
        GetComponent<SocketObject>().EmitMessage("Quick", data);
        Debug.Log("[DataWorker]退室しました");

        Destroy(_gameInstance);

        _gameManager._GameState.Value = GameState.ConnectionComp;

        _roomMaster = null;

        _watching = false;
        _exping = false;
        _myRoom = null;
        _me = null;
        posSync.Clear();
        rotSync.Clear();
        heatbeat.Clear();
        roomQue.Clear();
        hitQue.Clear();
        _players.Clear();

        Cursor.lockState = CursorLockMode.None;
        Cursor.SetCursor(null, Vector2.zero, CursorMode.ForceSoftware);

    }

    internal void SetPdImpact(string s)
    {
        if (_me != null)
            _me.GetComponent<PlayerScript>().PlayerData._impactRestriction = float.Parse(s);
    }

    internal void SetPdMoveSpeed(string s)
    {
        if (_me != null)
            _me.GetComponent<PlayerScript>().PlayerData._moveSpeedRate = float.Parse(s);
    }

    internal void SetPdAttackSpeed(string s)
    {
        if (_me != null)
            _me.GetComponent<PlayerScript>().PlayerData._attackSpeedRate = float.Parse(s);
    }

    internal void MenuInvisible()
    {
        //__menuStage.GetComponent<Animator>().SetBool("On", true);
        //_titleText.GetComponent<TitleEffect>().active = true;
        //CameraController.Instance.transform.parent = _startCamerapos.transform;
    }

    private void PlayerListSet()
    {
        Debug.Log("ルームエラー");
        if (_myRoom != null && _myRoom.cnt == _max && _players.Count == 0)
        {
            Debug.Log("ルームエラー");
            _players.Clear();
            foreach (KeyValuePair<string, string> member in _myRoom.member)
            {
                _players.Add(member.Key, null);
            }
            //playing = true;
            Debug.Log("Start[ルーム名：" + _myRoom.roomName + "/人数：" + _players.Count + "]");
            GameSettings();
        }
        else
        {
            Debug.Log("ルームエラー");
        }
    }

    private void GameSettings()
    {
        _gameInstance = (GameObject)Instantiate(_ga_meInstancePrefab);
        //_titleText.SetActive (false);
        //_titleText.GetComponent<TitleEffect>().active = true;
        _titleText.GetComponent<TitleEffect>().setActive_script(true);
        //_titleCamera.SetActive (false);
        //_sphereController.SetActive (false);
        //_cubesController.GetComponent<_cubesController> ().GameStart ();
        //__menuStage.SetActive(false);
        //__menuStage.GetComponent<Animator>().SetBool("On", true);
        //_shutter.SetActive(false);
        _instanceStage = (GameObject)Instantiate(_stagePrefab, Vector3.zero, Quaternion.identity);
        _instanceStage.transform.parent = _gameInstance.transform;
        _enhanced.SetActive(true);
        //Menu.SetActive(false);
    }

    private void MenuSetting()
    {

        _cameraController.transform.parent = _cameraController.cam_menu1_pos.transform;
        _resultUI.GetComponent<Animator>().SetBool("On", false);
        _enhanced.SetActive(false);
        //_cameraController.transform.parent = titleCamerapos.transform;
        Destroy(_instanceStage);
        _instanceObsCon.GetComponent<ObstacleControllSync>().DestroyAll();
        Destroy(_instanceObsCon);
        foreach (GameObject data in _players.Values)
        {
            Destroy(data);
        }
        DataClear();
        //__menuStage.SetActive(true);
        //CameraController.Instance.transform.parent = null;
        //__menuStage.GetComponent<Animator>().SetBool("On", false);
        //_titleCamera.SetActive (true);
        //_titleText.SetActive (true);
        _titleText.GetComponent<TitleEffect>().setActive_script(false);
        //_sphereController.SetActive (true);
        //_cubesController.SetActive (true);
        //_cubesController.GetComponent<_cubesController> ().CubeSetting ();
        //Menu.SetActive (true);
    }

    private void ActiveShutter()
    {
        _shutter.SetActive(true);
        _shutter.GetComponent<Animator>().SetBool("On", false);
    }

    private void InactiveShutter()
    {
        _shutter.GetComponent<Animator>().SetBool("On", true);
    }
}


