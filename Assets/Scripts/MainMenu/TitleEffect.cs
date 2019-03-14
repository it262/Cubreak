using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TitleEffect : MonoBehaviour
{

	IntermittentChaos IChaos;

    // Start is called before the first frame update
    void Start()
    {
		IChaos = new IntermittentChaos ();
		StartCoroutine ("Chaos");
    }
		
	IEnumerator Chaos(){
		while (true) {
			GetComponent<TextMeshPro> ().outlineWidth = IChaos.getChaos (GetComponent<TextMeshPro> ().outlineWidth);
			yield return new WaitForSeconds (0.1f);
		}
	}
}
