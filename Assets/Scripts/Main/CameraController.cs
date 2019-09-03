using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

public class CameraController : SingletonMonoBehavior<CameraController>
{
    GameManager gm;

    public GameObject lookAt,cam_menu1_pos,cam_menu2_pos, cam_menu3_pos;
    public GameObject Menu01, Menu02,Menu03;
    public Vector3 setPos;
    // Start is called before the first frame update
    void Start()
    {
        //setPos = transform.localPosition;
        

        gm = GameManager.Instance;
        gm._GameState
            .DistinctUntilChanged()
            .Where(x => x == GameState.Menu)
            .Subscribe(_ => setMenu01());
        gm._GameState
            .DistinctUntilChanged()
            .Where(x => x == GameState.ConnectionComp)
            .Subscribe(_ => setMenu02());
        gm._GameState
            .DistinctUntilChanged()
            .Where(x => x == GameState.RoomSerching)
            .Subscribe(_ => setMenu03());
        gm._GameState
            .DistinctUntilChanged()
            .Where(x => x == GameState.StartCount)
            .Subscribe(_ => setPlayer());
        transform.parent = cam_menu1_pos.transform;
        Debug.Log(transform.parent);
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log(transform.position);
        if (transform.localPosition != Vector3.zero)
        {
            transform.localPosition = Vector3.Lerp(transform.localPosition, Vector3.zero, 0.05f);
            if (Vector3.Distance(transform.localPosition, Vector3.zero) < 0.1f)
            {
                transform.localPosition = Vector3.zero;
            }
        }

        if (transform.localEulerAngles != Vector3.zero)
        {
            transform.localEulerAngles = Vector3.Lerp(transform.localEulerAngles, Vector3.zero, 0.05f);
            if (Vector3.Distance(transform.localEulerAngles, Vector3.zero) < 0.1f)
            {
                transform.localEulerAngles = Vector3.zero;
            }
        }

        if (lookAt != null)
        {
            transform.LookAt(lookAt.transform);
        }
        else
        {
            transform.localEulerAngles = Vector3.zero;
        }

        if( Input.GetKeyDown(KeyCode.Escape)&&  transform.parent == cam_menu1_pos.transform && gm._GameState.Value == GameState.Menu)
        {
            setMenu01();
        }
    }

    void setMenu01()
    {
        transform.parent = cam_menu1_pos.transform;
        Menu01.GetComponent<Animator>().SetBool("On", true);
        Menu02.GetComponent<Animator>().SetBool("On", false);
    }

    void setMenu02()
    {
        transform.parent = cam_menu2_pos.transform;
        Menu01.GetComponent<Animator>().SetBool("On", false);
        Menu02.GetComponent<Animator>().SetBool("On", true);
    }

    void setMenu03()
    {
        transform.parent = cam_menu3_pos.transform;
        Menu01.GetComponent<Animator>().SetBool("On", false);
        Menu02.GetComponent<Animator>().SetBool("On", false);
    }

    void setPlayer()
    {
        if (DataWorker.Instance != null && DataWorker.Instance.me != null)
        {
            transform.parent = DataWorker.Instance.me.GetComponent<PlayerScript>().cam.transform;
        }
    }
}
