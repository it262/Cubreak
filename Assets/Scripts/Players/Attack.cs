using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attack : MonoBehaviour
{
	[SerializeField]GameObject Arm;

	public float preSpeed = -20f;
	public Vector3 toScale = new Vector3(1f,1f,1f);
	Vector3 preScale;

	float speed;
	float scale;

	Vector3 buffer;

    // Start is called before the first frame update
    void Start()
    {
		buffer = Arm.transform.localPosition;
		speed = preSpeed;
		preScale = Arm.transform.localScale;
    }

    // Update is called once per frame
    void Update()
    {
		if (Input.GetMouseButton (0)) {
			if (Vector3.Angle (buffer, Arm.transform.localPosition) >= 60) {
				speed = preSpeed;
				Arm.transform.localPosition = buffer;
				Arm.transform.localScale = preScale;
			} else {
				Arm.transform.RotateAround (transform.localPosition, Vector3.up, speed=Mathf.Lerp(speed,-1,0.1f));
				Arm.transform.localScale = Vector3.Lerp (Arm.transform.localScale, toScale, 0.5f);
			}
		} else {
			Arm.transform.localPosition = buffer;
			Arm.transform.localScale = preScale;
			speed = preSpeed;
		}
    }
}
