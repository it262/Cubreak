using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObsUpdate : MonoBehaviour
{

	static SocketObject so;
    public LayerMask mask;
	private float preSpeed = 0;
    public float speed;

	public int id;

	public GameObject particle;
	public string type;

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
				Debug.Log (hit.transform.gameObject.tag);
				if (hit.transform.gameObject.CompareTag ("Stage")) {
					transform.position = new Vector3 (
						transform.position.x,
						hit.transform.position.y + 1f,
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
				preSpeed = 0;
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
		preSpeed += 0.1f;
		transform.Translate(0, -preSpeed * Time.deltaTime, 0);
    }

    private void OnCollisionEnter(Collision collision)
    {
		if (collision.gameObject.CompareTag ("Attacker")) {
			
		} else if(collision.gameObject.CompareTag("Player") && transform.CompareTag("Obstacle")){
			var data = new Dictionary<string,string> ();
			data ["TYPE"] = "PlayerEliminate";
			data ["trg"] = so.id;
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