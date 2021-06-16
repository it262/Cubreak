using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerScript : MonoBehaviour
{
    [SerializeField] private GameObject avater = default;

    internal static SocketObject _socketObject;
    internal static DataWorker _dataWorker;

    internal GameObject _camera;
    internal GameObject _headBone;
    internal bool _destroy = false;
    internal bool _debug = false;
    internal float _jumpPower;
    internal TransMesh _transmesh;

    private static GameManager _gameManager;
    private Animator _anim;
    private fpsCamera _fpsCam;
    private float _time = 0;
    private Quaternion _syncRotBufferV, _syncRotBufferH;
    private bool _isGroubded = true;
    private Quaternion _bufferHead, _bufferBody;
    private Vector3 _toPos;

    internal PlayerData PlayerData;

    // Use this for initialization
    void Start()
    {
        _socketObject = SocketObject.Instance;
        _dataWorker = DataWorker.Instance;
        _gameManager = GameManager.Instance;
        SyncPosition().Forget();
        _fpsCam = GetComponent<fpsCamera>();
        _anim = GetComponent<Animator>();
        _toPos = transform.position;
        transform.LookAt(new Vector3(0, transform.position.y, 0));
        _fpsCam.hRotation = transform.rotation;
    }

    // Update is called once per frame
    void Update()
    {

        /*
        PlayerData.atk = atk;
        PlayerData.dif = dif;
        PlayerData.spd = spd;
        */

        _time += Time.deltaTime;

        if (!_debug)
        {
            if (_socketObject == null)
                return;

            if (_gameManager._GameState.Value != GameState.Playing)
                return;

            if (!PlayerData._isPlayer)
            {

                if (_dataWorker.heatbeat.ContainsKey(PlayerData._id))
                {
                    _destroy = false;
                    _dataWorker.heatbeat.Remove(PlayerData._id);
                }


                if (_dataWorker.posSync.ContainsKey(PlayerData._id))
                {
                    _toPos = _dataWorker.posSync[PlayerData._id];
                    //transform.position = toPos;
                    _dataWorker.posSync.Remove(PlayerData._id);
                }

                transform.position = Vector3.Lerp(transform.position, _toPos, 0.5f);
                //rot -> LateUpdate()

                if (Vector3.Distance(transform.position, _toPos) > 0.1f)
                {
                    _anim.SetBool("Walk", true);
                }
                else
                {
                    transform.position = _toPos;
                    _anim.SetBool("Walk", false);
                }

                ExitPlayer();
                return;
            }

            if (Input.GetMouseButtonDown(0))
                CameraController.Instance.transform.parent = _camera.transform;

            if (_time > 5 && !_debug)
            {
                var data = new Dictionary<string, string>();
                _socketObject.EmitMessage("HeartBeat", data);
                _time = 0;
            }

            if (transform.position.y < -10f)
            {
                _dataWorker.DisconnectUser(PlayerData._id);
            }
        }

        RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.down, out hit, 3f))
        {
            if (hit.collider.gameObject.CompareTag("Stage") || hit.collider.gameObject.CompareTag("fallenObstacle"))
            {
                Debug.Log("地上！");
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    GetComponent<Rigidbody>().AddForce(new Vector3(0, _jumpPower, 0), ForceMode.Impulse);
                }
            }
            else
            {
                Debug.Log("空中！");
            }
        }

        //左shiftでスニーク（？）
        //moveSpeed = (Input.GetKey (KeyCode.LeftShift)) ? 1.0f : defaultSpeed;

        Vector3 velocity = Vector3.zero;

        transform.Rotate(transform.up * Input.GetAxis("Horizontal") * 3f);

        velocity = transform.forward * Input.GetAxis("Vertical");

        //Debug.DrawLine(transform.position,transform.position+ velocity*100f,Color.green);

        velocity = velocity.normalized * Time.deltaTime;
        velocity *= _debug ? 3f : PlayerData.GetMoveSpeed();

        if (velocity.magnitude > 0)
        {
            _anim.SetBool("Walk", true);
            //transform.position += _fpsCam.hRotation * velocity;
            transform.position += velocity;
        }
        else
        {
            _anim.SetBool("Walk", false);
        }
    }

    //Animatorで制御されているボーンを強制的に動作させるLateUpdate
    void LateUpdate()
    {
        if (PlayerData == null || GetComponent<DeathCam>().Die)
        {
            return;
        }

        if (PlayerData._isPlayer)
        {
            _headBone.transform.localRotation = _fpsCam.vRotation;
            transform.rotation = _fpsCam.hRotation;

            _syncRotBufferV = _headBone.transform.localRotation;
            _syncRotBufferH = transform.rotation;
        }
        else
        {
            Debug.Log(PlayerData._id);
            if (_dataWorker != null && _dataWorker.rotSync.ContainsKey(PlayerData._id))
            {
                Vector2 toRot = _dataWorker.rotSync[PlayerData._id];
                Quaternion head = Quaternion.Euler(0, toRot.x, 0);
                Quaternion body = Quaternion.Euler(0, toRot.y, 0);

                _bufferHead = head;
                _bufferBody = body;
                _dataWorker.rotSync.Remove(PlayerData._id);
            }
            _headBone.transform.localRotation = Quaternion.Lerp(_headBone.transform.localRotation, _bufferHead, 0.5f);
            transform.rotation = Quaternion.Lerp(transform.rotation, _bufferBody, 0.5f);
        }
    }


    private void ExitPlayer()
    {
        if (_time > 10f && _destroy)
        {
            if (_destroy)
            {
                _dataWorker.Exclusion(PlayerData._id);
            }
            else
            {
                _time = 0f;
                _destroy = true;
            }
        }
    }

    private async UniTask SyncPosition()
    {
        if (PlayerData._isPlayer && !_debug)
        {
            Vector3 bPos = transform.position;
            Quaternion bHead = _syncRotBufferV;
            Quaternion bBody = _syncRotBufferH;
            while (true)
            {
                if (Vector3.Distance(transform.position, bPos) > 0.1f || Quaternion.Angle(_syncRotBufferH, bBody) > 0.1f || Quaternion.Angle(_syncRotBufferV, bHead) > 0.1f)
                {
                    var data = new Dictionary<string, string>();
                    data["TYPE"] = "Transform";
                    data["x"] = transform.position.x.ToString();
                    data["y"] = transform.position.y.ToString();
                    data["z"] = transform.position.z.ToString();
                    data["bodyY"] = _syncRotBufferH.eulerAngles.y.ToString();
                    data["headY"] = _syncRotBufferV.eulerAngles.y.ToString();
                    _socketObject.EmitMessage("ToOwnRoom", data);
                    Debug.Log("Transform送信");
                    bPos = transform.position;
                    bHead = _syncRotBufferV;
                    bBody = _syncRotBufferH;
                }
                await UniTask.Delay(50);
            }
        }
    }
}
