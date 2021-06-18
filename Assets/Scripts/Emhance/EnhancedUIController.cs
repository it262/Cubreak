using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnhancedUIController : MonoBehaviour
{
    internal static SocketObject _socketObject;
    internal static DataWorker _dataWorker;
    internal GameObject _attackUI, _diffenceUI, _speedUI;

    private static GameManager _gameManager;

    private float _attack = 50f;
    private float _defence = 50f;
    private float _speed = 50f;
    private PlayerScript _mine;
    private filledImage _filledImageA, _filledImageD, _filledImageS;

    // Start is called before the first frame update
    void Start()
    {
        _socketObject = SocketObject.Instance;
        _dataWorker = DataWorker.Instance;
        _gameManager = GameManager.Instance;
        _filledImageA = _attackUI.GetComponent<filledImage>();
        _filledImageD = _diffenceUI.GetComponent<filledImage>();
        _filledImageS = _speedUI.GetComponent<filledImage>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!_socketObject.id.Equals("") && _dataWorker._me != null)
        {
            if (_mine == null)
            {
                _mine = _dataWorker._me.GetComponent<PlayerScript>();
            }
            FillUpdate(_mine.PlayerData);
        }

    }
    /*
    public void ClickAttack()
    {
        attack += 2f;
        defence -= 2f;
        speed -= 1f;

		fillUpdate ();
    }

    public void ClickDefence()
    {
        attack -= 1f;
        defence += 2f;
        speed -= 2f;

		fillUpdate ();
    }

    public void ClickSpeed()
    {
        attack -= 2f;
        defence -= 1f;
        speed += 2f;

		fillUpdate ();
    }
*/
    private void FillUpdate(PlayerData state)
    {
        _filledImageA.changeTargetAmount(state._atk);
        _filledImageD.changeTargetAmount(state._dif);
        _filledImageS.changeTargetAmount(state._spd);
    }
}
