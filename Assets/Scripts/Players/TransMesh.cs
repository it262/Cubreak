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


    [SerializeField] Mesh originMesh;

    [SerializeField] Material mat;

    [SerializeField] GameObject effect;

    Mesh copyMesh;

    public Vector3 start, end;

    // Start is called before the first frame update
    void Start()
    {
        so = SocketObject.Instance;
        dw = DataWorker.Instance;
        ps = transform.parent.GetComponent<PlayerScript>();

        copyMesh = Instantiate(originMesh);
        mf = GetComponent<SkinnedMeshRenderer>();
        mc = GetComponent<MeshCollider>();
        mf.sharedMesh = copyMesh;
        mf.sharedMesh.RecalculateBounds();
        mf.sharedMesh.RecalculateNormals();
        mc.sharedMesh = mf.sharedMesh;

        for(int i=0; i<GetComponent<Renderer>().sharedMaterials.Length; i++)
        {
            GetComponent<Renderer>().sharedMaterials[i] = mat;
        }

        start = end = Vector3.zero;

        ps.model = this;

    }
    // Update is called once per frame
    void Update()
    {
        if (start != Vector3.zero && end != Vector3.zero)
        {
            Vector3 startW = mf.transform.localToWorldMatrix.MultiplyPoint(start);
            Vector3 endW = mf.transform.localToWorldMatrix.MultiplyPoint(end);
            transformMesh(start, end);
            if (ps.isPlayer)
            {
                ps.impact = (endW - startW).normalized;
            }
            Destroy(Instantiate(effect, endW, Quaternion.identity), 5);
            start = end = Vector3.zero;
            return;
        }
    }

    private void transformMesh(Vector3 start, Vector3 end)
    {
        //ローカル座標を受け取る
        test = copyMesh.vertices;
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
        mf.sharedMesh.RecalculateNormals();
        mc.sharedMesh = mf.sharedMesh;
    }

    void OnDrawGizmos()
    {
        Vector3 s = mf.transform.localToWorldMatrix.MultiplyPoint(start);
        Vector3 e = mf.transform.localToWorldMatrix.MultiplyPoint(end);
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(s, e);
    }
}
