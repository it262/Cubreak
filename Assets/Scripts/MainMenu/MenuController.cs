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
				this.gameObject.SetActive (!dw.playing);
    }
}
