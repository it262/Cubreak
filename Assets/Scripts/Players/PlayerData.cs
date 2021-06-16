using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerData
{
    internal string _id;
    internal string _name;
    internal bool _isPlayer = false;

    internal int _atk;
    internal int _dif;
    internal int _spd;

    internal float _attackRange = 5f;

    internal float _impactRestriction = 10f;
    internal float _moveSpeedRate = 0.05f;
    internal float _attackSpeedRate = 0.035f;

    private int _max = 50;
    private int _min = 1;

    private int _plus = 3;
    private int _minus = 1;

    internal PlayerData()
    {
        _atk = 25;
        _dif = 25;
        _spd = 25;
    }

    internal void ChangeState(string str)
    {
        switch (str)
        {
            case "atk":
                AtkPlus();
                break;
            case "dif":
                DifPlus();
                break;
            case "spd":
                SpdPlus();
                break;
            default:
                BlackMinus();
                break;
        }
    }

    internal void AtkPlus()
    {
        _atk += _plus;
        _dif = (_dif - _minus <= _min) ? _min : _dif - _minus;
        _spd = (_spd - _minus <= _min) ? _min : _spd - _minus;
        if (_atk > _max)
        {
            _atk = _max;
        }
    }
    internal void DifPlus()
    {
        _dif += _plus;
        _atk = (_atk - _minus <= _min) ? _min : _atk - _minus;
        _spd = (_spd - _minus <= _min) ? _min : _spd - _minus;
        if (_dif > _max)
        {
            _dif = _max;
        }
    }
    internal void SpdPlus()
    {
        _spd += _plus;
        _atk = (_atk - _minus <= _min) ? _min : _atk - _minus;
        _dif = (_dif - _minus <= _min) ? _min : _dif - _minus;
        if (_spd > _max)
        {
            _spd = _max;
        }
    }

    internal void BlackMinus()
    {
        _atk = GetRandomState(_atk);
        _dif = GetRandomState(_dif);
        _spd = GetRandomState(_spd);

        int GetRandomState(int state)
        {
            int sub = Random.Range(0, _minus + 1);
            return (state - sub <= 0) ? _min : state - sub;
        }

        Debug.Log("[State]" + _atk + ":" + _dif + ";" + _spd);
    }

    internal float GetMoveSpeed()
    {
        return 3f + (_spd * _moveSpeedRate);
    }

    internal float GetAttackSpeed()
    {
        return 2f - (_spd * _attackSpeedRate);
    }

    internal Vector3 GetImpactVector(Vector3 vector, PlayerData target)
    {
        float force = _atk - target._dif;
        if (force <= 0)
        {
            force = 1;
        }
        /*
        else if (force > impact_Restriction)
        {
            force = impact_Restriction;
        }
        */
        return vector * 3f + vector * force * 0.3f;
    }

}