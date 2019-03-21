using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attack : MonoBehaviour
{
	[SerializeField]GameObject Arm;

	public float speed = 1f;

	Vector3 buffer;

    // Start is called before the first frame update
    void Start()
    {
		buffer = Arm.transform.localPosition;
    }

    // Update is called once per frame
    void Update()
    {
		if (Input.GetMouseButton (0)) {
			if (transform.localPosition.z < Arm.transform.localPosition.z) {
				Arm.transform.RotateAround (transform.position, Vector3.up, -speed);
			} else {
				Arm.transform.localPosition = buffer;
			}
			return;
		}
		Arm.transform.localPosition = buffer;
    }
}
