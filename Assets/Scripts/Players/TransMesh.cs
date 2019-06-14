using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransMesh : MonoBehaviour
{
    SocketObject so;
    DataWorker dw;
    PlayerScript ps;

    Dictionary<string, string> d;

    SkinnedMeshRenderer mf;
    //MeshFilter mf;
    MeshCollider mc;
    Vector3[] test;

    // Start is called before the first frame update
    void Start()
    {
        so = SocketObject.Instance;
        dw = DataWorker.Instance;
        ps = transform.parent.GetComponent<PlayerScript>();

        mf = GetComponent<SkinnedMeshRenderer>();
        mc = GetComponent<MeshCollider>();
        test = mf.sharedMesh.vertices;
        
    }
    // Update is called once per frame
    void Update()
    {
        if (dw.hitQue.Count > 0 && ps.id.Equals(dw.hitQue.Peek()["trg"]))
        {
            d = dw.hitQue.Dequeue();
            Vector3 start = new Vector3(float.Parse(d["startX"]), float.Parse(d["startY"]), float.Parse(d["startZ"]));
            Vector3 end = new Vector3(float.Parse(d["endX"]), float.Parse(d["endY"]), float.Parse(d["endZ"]));
            transformMesh(start, end);
        }

        /*
        if (Input.GetMouseButton(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, Mathf.Infinity))
            {
                mf = hit.transform.GetComponent<MeshFilter>();
                mc = hit.transform.GetComponent<MeshCollider>();

                //検出したオブジェクトのローカル座標に変換
                Vector3 start = mf.transform.worldToLocalMatrix.MultiplyPoint(Camera.main.transform.position);
                Vector3 end = mf.transform.worldToLocalMatrix.MultiplyPoint(hit.point);


                transformMesh(start,end);
            }
        }*/
    }

    private void transformMesh(Vector3 start,Vector3 end)
    {
        //ローカル座標を受け取る
        test = mf.sharedMesh.vertices;
        for (int i = 0; i < test.Length; i++)
        {
            Vector3 transPoint = test[i];
            float distance = Vector3.Distance(end, transPoint);
            if (distance < 5)
            {
                test[i] += (transPoint - start).normalized * (1 / Mathf.Sqrt(distance * distance)) * Time.deltaTime;
            }
        }
        mf.sharedMesh.vertices = test;
        mf.sharedMesh.RecalculateBounds();    //メッシュコンポーネントのプロパティboundsを再計算する
        mc.sharedMesh = mf.sharedMesh;
    }
}
