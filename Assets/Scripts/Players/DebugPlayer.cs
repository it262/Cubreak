using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugPlayer : MonoBehaviour
{
    public static SocketObject so;
    public static DataWorker dw;
    static GameManager gm;

    Animator anim;
    public PlayerData pd;
    private fpsCamera fpsCam;

    public GameObject cam, headBone;

    public bool destroy = false;
    public bool debug = false;

    public float jumpPower;

    private float time = 0;
    private Quaternion syncRotBufferV, syncRotBufferH;
    private bool isGroubded = true;
    private Quaternion bufferHead, bufferBody;
    private Vector3 toPos;

    [SerializeField]
    private GameObject avater;

    public TransMesh model;

    public int atk, dif, spd;

    // Use this for initialization
    void Start()
    {
        so = SocketObject.Instance;
        dw = DataWorker.Instance;
        gm = GameManager.Instance;
        pd = new PlayerData ();
        //toPos = transform.position;
        //transform.LookAt(new Vector3(0, transform.position.y, 0));
        //fpsCam.hRotation = transform.rotation;
    }

    private void Update()
    {
        pd._atk = atk;
        pd._dif = dif;
        pd._spd = spd;
    }
}
