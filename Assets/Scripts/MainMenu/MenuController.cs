using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuController : MonoBehaviour
{
	static DataWorker dw;
    // Start is called before the first frame update
    void Start()
    {
		dw = DataWorker.Instance;
    }

    // Update is called once per frame
    void Update()
    {
		if (dw.playing) {
			GetComponent<RectTransform> ().localPosition = Vector3.Lerp (GetComponent<RectTransform> ().localPosition, new Vector3 (0, -150f, 0), 0.8f);
			if (GetComponent<RectTransform> ().localPosition.y < 149f)
				this.gameObject.SetActive (false);
		} else {
			GetComponent<RectTransform> ().localPosition = Vector3.Lerp (GetComponent<RectTransform> ().localPosition, new Vector3 (0, 0, 0), 0.8f);
		}
    }
}
