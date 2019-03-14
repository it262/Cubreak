using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObsUpdate : MonoBehaviour
{

	static SocketObject so;
    public LayerMask mask;
    public float speed;

	public int id;

	public GameObject particle;

    // Use this for initialization
    void Start()
    {
		so = SocketObject.Instance;
    }

    // Update is called once per frame
    void Update()
    {
        Ray ray = new Ray(transform.position, -transform.up);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, Mathf.Infinity))
        {
            float dis = hit.distance;
            if (dis <= 0.5)
            {
				if (hit.transform.gameObject.CompareTag ("Stage")) {
					transform.position = new Vector3 (
						transform.position.x,
						hit.transform.position.y + 0.5f,
						transform.position.z
					);
				} else {
					transform.position = new Vector3 (
						transform.position.x,
						hit.transform.position.y + 1f,
						transform.position.z
					);
				}
                gameObject.tag = "fallenObstacle";
            }
            else {
                fallen();
            }
        }
        else {
            fallen();
        }

        if (transform.CompareTag("fallenObstacle")) {
            //nothing
        }

		if (transform.position.y < -10f) {
			Destroy (gameObject);
		}

        //Debug.DrawRay(ray.origin, ray.direction, Color.red, Mathf.Infinity);
    }

    //落とす関数。今回は簡略化。
    private void fallen()
    {
        transform.Translate(0, -speed * Time.deltaTime, 0);
    }

    private void OnCollisionEnter(Collision collision)
    {
		if (collision.gameObject.CompareTag ("Bullet") || collision.gameObject.CompareTag ("MyBullet")) {
			Destroy (collision.gameObject);
			var data = new Dictionary<string,string> ();
			data ["TYPE"] = "DestroyObs";
			data ["n"] = id.ToString ();
			so.EmitMessage ("ToOwnRoom", data);
			Debug.Log ("Send:" + id + "破壊");
			Destroy ();
		} else if(collision.gameObject.CompareTag("Player") && transform.CompareTag("Obstacle")){
			var data = new Dictionary<string,string> ();
			data ["TYPE"] = "Hit";
			data ["trg"] = so.id;
			data["damage"] = "10";
			so.EmitMessage ("ToOwnRoom", data);
			Debug.Log ("Send:" + id + "破壊");
			Destroy ();
		}
    }

	public void Destroy(){
		particle.GetComponent<Renderer> ().material = GetComponent<Renderer> ().material;
		Instantiate (particle, transform.position, Quaternion.identity);
		Destroy(this.gameObject);
	}
}