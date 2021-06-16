using System.Collections.Generic;
using UnityEngine;
using System.Collections;

public class Attack : MonoBehaviour
{
    private static SocketObject _socketObject;
    private static DataWorker _dataWoker;

    [SerializeField] private LayerMask _layerMask = default;
    [SerializeField] private GameObject _hand = default;

    private PlayerScript _mine;
    private Vector3 _handPos;

    // Start is called before the first frame update
    private void Start()
    {
        _socketObject = SocketObject.Instance;
        _dataWoker = DataWorker.Instance;
        _mine = GetComponent<PlayerScript>();
        _handPos = _hand.transform.localPosition;

        StartCoroutine(AttackByTime());
    }

    private IEnumerator AttackByTime()
    {
        while (true)
        {
            if (Input.GetMouseButton(0))
            {
                if (Physics.Raycast(_mine._camera.transform.position, Camera.main.transform.forward, out RaycastHit hit, _mine.PlayerData._attackRange, _layerMask))
                {
                    Debug.Log(hit.collider.gameObject.tag);
                    if (hit.collider.gameObject.CompareTag("OthersAvater"))
                    {
                        GameObject parent = hit.transform.parent.gameObject;
                        var mf = hit.transform.GetComponent<SkinnedMeshRenderer>();
                        //検出したオブジェクトのローカル座標に変換
                        Vector3 start = mf.transform.worldToLocalMatrix.MultiplyPoint(Camera.main.transform.position);
                        Vector3 end = mf.transform.worldToLocalMatrix.MultiplyPoint(hit.point);
                        var data = new Dictionary<string, string>();
                        data["TYPE"] = "Hit";
                        data["trg"] = parent.GetComponent<PlayerScript>().PlayerData._id;
                        data["startX"] = start.x.ToString();
                        data["startY"] = start.y.ToString();
                        data["startZ"] = start.z.ToString();
                        data["endX"] = end.x.ToString();
                        data["endY"] = end.y.ToString();
                        data["endZ"] = end.z.ToString();
                        _socketObject.EmitMessage("ToOwnRoom", data);
                    }
                    else if (hit.collider.gameObject.CompareTag("fallenObstacle") || hit.collider.gameObject.CompareTag("Obstacle"))
                    {
                        var data = new Dictionary<string, string>();
                        data["TYPE"] = "DestroyObs";
                        data["n"] = hit.collider.gameObject.GetComponent<ObsUpdate>().id.ToString();
                        data["attacker"] = _socketObject.id.ToString();
                        _socketObject.EmitMessage("ToOwnRoom", data);
                        Debug.Log("Send:" + hit.collider.gameObject.GetComponent<ObsUpdate>().id.ToString() + "破壊");
                        //hit.collider.gameObject.GetComponent<ObsUpdate>().Destroy ();
                    }
                    else if (hit.collider.gameObject.CompareTag("Switch"))
                    {
                        var data = new Dictionary<string, string>();
                        data["TYPE"] = "PushSwitch";
                        data["trg"] = hit.collider.gameObject.transform.parent.gameObject.GetComponent<PlayerScript>().PlayerData._id;
                        _socketObject.EmitMessage("ToOwnRoom", data);
                        Debug.Log("Push:" + hit.collider.gameObject.GetComponent<ObsUpdate>().id.ToString() + "爆破");
                    }
                    else if (hit.collider.gameObject.CompareTag("Debug"))
                    {
                        GameObject parent = hit.transform.parent.gameObject;
                        var mf = hit.transform.GetComponent<SkinnedMeshRenderer>();
                        //検出したオブジェクトのローカル座標に変換
                        Vector3 start = mf.transform.worldToLocalMatrix.MultiplyPoint(Camera.main.transform.position);
                        Vector3 end = mf.transform.worldToLocalMatrix.MultiplyPoint(hit.point);
                        Vector3 impact = (end - start).normalized;
                        Debug.Log("Debug_Impact");
                        Vector3 force = GetComponent<PlayerScript>().PlayerData.GetImpactVector(impact, parent.GetComponent<DebugPlayer>().pd);
                        parent.GetComponent<Rigidbody>().AddForce(force, ForceMode.Impulse);
                    }
                    yield return new WaitForSeconds(_mine.PlayerData.GetAttackSpeed());

                }
            }
            yield return null;
        }
    }

    private Vector3 GetAngleVec(Vector3 from, Vector3 to)
    {
        Vector3 fromVec = new Vector3(from.x, 0, from.z);
        Vector3 toVec = new Vector3(to.x, 0, to.z);
        return Vector3.Normalize(toVec - fromVec);
    }
}
