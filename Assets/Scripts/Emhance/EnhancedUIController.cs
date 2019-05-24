using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnhancedUIController : MonoBehaviour
{
    float attack = 50f;
    float defence = 50f;
    float speed = 50f;

	public static SocketObject so;
	public static DataWorker dw;
	State state;

    public GameObject AttackUI, DefenceUI, SpeedUI;
    // Start is called before the first frame update
    void Start()
    {
		so = SocketObject.Instance;
		dw = DataWorker.Instance;
    }

    // Update is called once per frame
    void Update()
    {
		if (dw.leady) {
			transform.localPosition = new Vector3(0,0,0);
		} else {
			transform.localPosition = new Vector3(-200,0,0);
		}
		if (!so.id.Equals("") && dw.players.ContainsKey (so.id)) {
			state = dw.players [so.id].GetComponent<PlayerScript> ().state;
			AttackUI.GetComponent<filledImage>().changeTargetAmount(state.atk);
			DefenceUI.GetComponent<filledImage>().changeTargetAmount(state.dif);
			SpeedUI.GetComponent<filledImage>().changeTargetAmount(state.spd);
		}
        
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
