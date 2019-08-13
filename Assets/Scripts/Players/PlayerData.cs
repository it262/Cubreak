using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerData
{

    public string id, name;
    public bool isPlayer = false;

    public int atk;
    public int dif;
    public int spd;

    private int max = 50;
    private int min = 1;

    private int plus = 3;
    private int minus = 1;

    public float attackRange = 5f;

    public float impact_Restriction = 10;
    public float moveSpeedRate = 0.05f;
    public float attackSpeedRate = 0.035f;

    public PlayerData()
    {
        atk = 25;
        dif = 25;
        spd = 25;
    }

    public void changeState(string str)
    {
        switch (str)
        {
            case "atk":
                atk_plus();
                break;
            case "dif":
                dif_plus();
                break;
            case "spd":
                spd_plus();
                break;
            default:
                black_minus();
                break;
        }
    }

    public void atk_plus()
    {
        atk += plus;
        dif = (dif - minus <= min) ? min : dif - minus;
        spd = (spd - minus <= min) ? min : spd - minus;
        if (atk > max)
            atk = max;
    }
    public void dif_plus()
    {
        dif += plus;
        atk = (atk - minus <= min) ? min : atk - minus;
        spd = (spd - minus <= min) ? min : spd - minus;
        if (dif > max)
            dif = max;
    }
    public void spd_plus()
    {
        spd += plus;
        atk = (atk - minus <= min) ? min : atk - minus;
        dif = (dif - minus <= min) ? min : dif - minus;
        if (spd > max)
            spd = max;
    }

    public void black_minus()
    {
        int sub = Random.Range(0, minus+1);
        atk = (atk - sub <= 0) ? min : atk - sub;
        sub = Random.Range(0, minus+1);
        dif = (dif - sub <= 0) ? min : dif - sub;
        sub = Random.Range(0, minus+1);
        spd = (spd - sub <= 0) ? min : spd - sub;

        Debug.Log("[State]"+atk+":"+dif+";"+spd);
    }

    public float getMoveSpeed()
    {
        return 3 + (spd * moveSpeedRate);
    }

    public float getAttackSpeed()
    {
        return 2 - (spd * attackSpeedRate);
    }

    public Vector3 getImpactVector(Vector3 vector, PlayerData target)
    {
        float force = atk - target.dif;
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
        return vector * 3 + vector * force*0.3f;
    }

}
