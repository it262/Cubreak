using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cubeModel : MonoBehaviour {

	static DataWorker dw;

    Color color = new Color();

    public float intensity = 0;

    public bool isEnter = false;
    public bool isStart = false;

	float chaos = 0.1f;

	// Use this for initialization
	void Start () {
		dw = DataWorker.Instance;
		StartCoroutine ("Chaos");
	}
	
	// Update is called once per frame
	void Update () {
        if (isEnter || isStart)
        {

			if (!isStart || !dw.searching) {
				

				//intensity = Mathf.Lerp(intensity, 1, 0.1f);
				chaos = 1;
				if (intensity >= 0.9) {
					if (isEnter) {
						isEnter = false;
					}

					if (isStart) {
						//色が全付いた瞬間
					}
 
				}
			}
        }
        else
        {
			chaos = 0;
			//intensity = Mathf.Lerp(intensity, 0, 0.1f);
        }

		intensity = Mathf.Lerp(intensity, chaos, 0.1f);


        gameObject.GetComponent<Renderer>().material.SetColor("_EmissionColor",
			new Color(color.r, color.g, color.b) * intensity);

	}

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("ChangeColorArea"))
        {
            isEnter = true;
        }

        if (other.CompareTag("ChangeColorArea_Start"))
        {
            isStart = true;
        }
    }

    public void setColorDef()
    {
        color = gameObject.GetComponent<Renderer>().material.GetColor("_EmissionColor");
    }

	IEnumerator Chaos(){
		while (true) {
			if (dw.searching) {
				if (intensity > 0.5f) {
					chaos = chaos + 2 * chaos * chaos;
				} else {
					chaos = chaos - 2 * (1 - chaos) * (1 - chaos);
				}
				if (chaos < 0.05f || chaos > 0.995) {
					chaos = Random.Range (0.1f, 0.9f);
				}
			}
			yield return new WaitForSeconds (0.1f);
		}
	}
}
