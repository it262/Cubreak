using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class shoot : MonoBehaviour {
    public GameObject bulletPrefab;
    public Transform muzzle;
    private float bulletPower = 230f;
	private float time = 0;
	private float rateOfFire = 0.1f;
	// Use this for initialization
	void Start () {
		if(transform.root.gameObject.GetComponent<player>().isPlayer)
			bulletPrefab.tag = "MyBullet";
	}
	
	// Update is called once per frame
	void Update () {
		//連射対応
		if (transform.root.gameObject.GetComponent<player>().isPlayer) {
			time += (time > rateOfFire) ? 0 : Time.deltaTime;
			if (Input.GetMouseButton (0) && time > rateOfFire) {
				Shot ();
				time = 0;
			}
		}
		
	}

    void Shot()
    {
        var bulletInstance = GameObject.Instantiate(bulletPrefab, muzzle.position, muzzle.rotation * bulletPrefab.transform.rotation) as GameObject;
        bulletInstance.GetComponent<Rigidbody>().AddForce(bulletInstance.transform.up * bulletPower);
		bulletInstance.GetComponent<bullet> ().player = transform.root.gameObject;
        Destroy(bulletInstance, 5f);
    }
}
