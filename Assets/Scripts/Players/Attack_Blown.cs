using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attack_Blown : MonoBehaviour
{
	SocketObject so;

    // Start is called before the first frame update
    void Start()
    {
		so = SocketObject.Instance;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

	void OnCollisionEnter(Collision collision){
		/*
		if (collision.gameObject.CompareTag ("Others")) {
			collision.gameObject.GetComponent<PlayerScript> ();
		}else if(collision.gameObject.CompareTag ("fallenObstacle") || collision.gameObject.CompareTag ("Obstacles")){
			collision.gameObject.GetComponent<ObsUpdate> ();
			so.EmitMessage()
		}
		*/
	}
}
