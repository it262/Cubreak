using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathCam : MonoBehaviour
{
    private DataWorker _dataWorker;

    [SerializeField] private GameObject explosion = default;

    private PlayerScript _playerScript;
    private GameObject _camera;
    private bool _end = false;

    internal bool Die { get; private set; } = false;

    // Start is called before the first frame update
    void Start()
    {
        _dataWorker = DataWorker.Instance;
        _camera = GetComponent<PlayerScript>()._camera;
        _playerScript = GetComponent<PlayerScript>();
    }

    // Update is called once per frame
    void Update()
    {
        if (_dataWorker == null)
            return;
        if (_dataWorker.pushSwitch.ContainsKey(_playerScript.PlayerData._id))
        {
            if (_end)
            {
                _dataWorker.pushSwitch.Remove(_playerScript.PlayerData._id);
                if (_playerScript.PlayerData._isPlayer)
                {
                    Destroy(_camera);
                    _dataWorker.DisconnectUser(_playerScript.PlayerData._id);
                }
            }
            if (_playerScript.PlayerData._isPlayer)
            {
                if (_camera.transform.parent != _dataWorker._gameInstance.transform)
                {
                    _dataWorker._exping = true;
                    _camera.transform.parent = _dataWorker._gameInstance.transform;
                    _camera.transform.position = transform.position + new Vector3(0, 5f, -5f);
                    //GetComponent<PlayerScript> ().enabled = false;
                    GetComponent<Rigidbody>().useGravity = false;
                    GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
                    GetComponent<Rigidbody>().angularVelocity = new Vector3(Random.Range(0, 100f), Random.Range(0, 100f), Random.Range(0, 100f));
                    DeadAsync().Forget();
                }
                _camera.transform.LookAt(this.gameObject.transform.position);
            }
            else
            {
                if (!Die)
                {
                    DeadAsync().Forget();
                    _dataWorker._exping = false;
                    Die = true;
                }
            }
        }
    }

    private async UniTask DeadAsync()
    {
        GameObject g = (GameObject)Instantiate(explosion, transform.position, Quaternion.identity);
        g.transform.parent = _dataWorker._gameInstance.transform;
        Destroy(g, 5f);
        GetComponent<AudioSource>().Play();
        await UniTask.Delay(5000);
        _end = true;
    }
}
