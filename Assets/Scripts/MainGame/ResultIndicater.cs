using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResultIndicater : MonoBehaviour
{

    [SerializeField] Text ranks,score;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void setRanks(string r)
    {
        ranks.text = r;
    }

    public void setScore(int s)
    {
        score.text = "score:+"+s.ToString();
    }
}
