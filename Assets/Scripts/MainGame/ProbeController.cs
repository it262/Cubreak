using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProbeController : MonoBehaviour
{
	ReflectionProbe probe;

	void Start(){
		probe = GetComponent<ReflectionProbe> ();
	}

	void Update(){
		probe.transform.position = new Vector3 (
			transform.position.x,
			transform.position.y*-1,
			transform.position.z
		);

		probe.RenderProbe ();
	}
}
