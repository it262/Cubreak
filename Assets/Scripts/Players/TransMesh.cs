using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransMesh : MonoBehaviour
{
    [SerializeField] private Mesh _originMesh = default;
    [SerializeField] private Material _mat = default;
    [SerializeField] private GameObject _effect = default;
    [SerializeField] bool _debug = false;

    private SocketObject _socketObject;
    private DataWorker _dataWorker;
    private PlayerScript _playerScript;
    private Dictionary<string, string> _dictionary;
    private SkinnedMeshRenderer _skinnedMesh;
    private MeshCollider _meshCollider;
    private Vector3[] _test;
    private Mesh _copyMesh;
    private Vector3 _start, _end;
    private GameObject _target;

    // _start is called before the first frame update
    void Start()
    {
        _socketObject = SocketObject.Instance;
        _dataWorker = DataWorker.Instance;
        if (!_debug)
            _playerScript = transform.parent.GetComponent<PlayerScript>();

        _copyMesh = Instantiate(_originMesh);
        _skinnedMesh = GetComponent<SkinnedMeshRenderer>();
        _meshCollider = GetComponent<MeshCollider>();
        _skinnedMesh.sharedMesh = _copyMesh;
        _skinnedMesh.sharedMesh.RecalculateBounds();
        _skinnedMesh.sharedMesh.RecalculateNormals();
        _meshCollider.sharedMesh = _skinnedMesh.sharedMesh;

        for (int i = 0; i < GetComponent<Renderer>().sharedMaterials.Length; i++)
        {
            GetComponent<Renderer>().sharedMaterials[i] = _mat;
        }

        _start = _end = Vector3.zero;
        if (!_debug)
            _playerScript._transmesh = this;

    }
    // Update is called once per frame
    void Update()
    {
        if (_debug)
            return;

        if (_start != Vector3.zero && _end != Vector3.zero)
        {
            Vector3 _startW = _skinnedMesh.transform.localToWorldMatrix.MultiplyPoint(_start);
            Vector3 _en_dataWorker = _skinnedMesh.transform.localToWorldMatrix.MultiplyPoint(_end);
            TransformMesh(_start, _end);
            if (_playerScript.PlayerData._isPlayer)
            {
                Vector3 impact = (_en_dataWorker - _startW).normalized;
                Debug.Log("impact");
                transform.parent.gameObject.GetComponent<Rigidbody>().AddForce(_playerScript.PlayerData.GetImpactVector(impact, _target.GetComponent<PlayerScript>().PlayerData), ForceMode.Impulse);
            }
            Destroy(Instantiate(_effect, _en_dataWorker, Quaternion.identity), 5);
            _start = _end = Vector3.zero;
        }
    }

    internal void SetImpactData(Vector3 _start, Vector3 _end, GameObject target)
    {
        this._start = _start;
        this._end = _end;
        this._target = target;
    }

    private void TransformMesh(Vector3 _start, Vector3 _end)
    {
        //ローカル座標を受け取る
        _test = _copyMesh.vertices;
        for (int i = 0; i < _test.Length; i++)
        {
            Vector3 transPoint = _test[i];
            float distance = Vector3.Distance(_end, transPoint);
            if (distance < 5)
            {
                _test[i] += (transPoint - _start).normalized * (1 / Mathf.Sqrt(distance * distance)) * Time.deltaTime;
            }
        }
        _skinnedMesh.sharedMesh.vertices = _test;
        _skinnedMesh.sharedMesh.RecalculateBounds();    //メッシュコンポーネントのプロパティboundsを再計算する
        _skinnedMesh.sharedMesh.RecalculateNormals();
        _meshCollider.sharedMesh = _skinnedMesh.sharedMesh;
    }
}
