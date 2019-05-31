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
	PlayerScript mine;
	State state;

    public GameObject AttackUI, DiffenceUI, SpeedUI;
	filledImage a,d,s;
    // Start is called before the first frame update
    void Start()
    {
		so = SocketObject.Instance;
		dw = DataWorker.Instance;
		a = AttackUI.GetComponent<filledImage> ();
		d = DiffenceUI.GetComponent<filledImage> ();
		s = SpeedUI.GetComponent<filledImage> ();
    }

    // Update is called once per frame
    void Update()
    {
		if (dw.leady) {
			transform.localPosition = new Vector3(0,0,0);
		} else {
			transform.localPosition = new Vector3(-200,0,0);
		}
		if (!so.id.Equals("") && dw.me != null) {
			if (mine == null) {
				mine = dw.me.GetComponent<PlayerScript> ();
			}
			state = mine.state;
			fillUpdate (state);
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
	void fillUpdate(State state){
		a.changeTargetAmount(state.atk);
		d.changeTargetAmount(state.dif);
		s.changeTargetAmount(state.spd);
	}
}
