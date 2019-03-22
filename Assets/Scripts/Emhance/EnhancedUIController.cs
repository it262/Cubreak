using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnhancedUIController : MonoBehaviour
{
    float attack = 50f;
    float defence = 50f;
    float speed = 50f;

    public GameObject AttackUI, DefenceUI, SpeedUI;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ClickAttack()
    {
        attack += 2f;
        defence -= 2f;
        speed -= 1f;

        AttackUI.GetComponent<filledImage>().changeTargetAmount(attack);
        DefenceUI.GetComponent<filledImage>().changeTargetAmount(defence);
        SpeedUI.GetComponent<filledImage>().changeTargetAmount(speed);
    }

    public void ClickDefence()
    {
        attack -= 1f;
        defence += 2f;
        speed -= 2f;

        AttackUI.GetComponent<filledImage>().changeTargetAmount(attack);
        DefenceUI.GetComponent<filledImage>().changeTargetAmount(defence);
        SpeedUI.GetComponent<filledImage>().changeTargetAmount(speed);
    }

    public void ClickSpeed()
    {
        attack -= 2f;
        defence -= 1f;
        speed += 2f;

        AttackUI.GetComponent<filledImage>().changeTargetAmount(attack);
        DefenceUI.GetComponent<filledImage>().changeTargetAmount(defence);
        SpeedUI.GetComponent<filledImage>().changeTargetAmount(speed);
    }
}
