using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObsUpdate : MonoBehaviour
{

	static SocketObject so;
    public LayerMask mask;
	private float preSpeed = 0;
    public float speed;
	//public int type;

	public int id;

	public GameObject particle;
	public string type;

	bool falling = true;

    public GameObject core;

    public GameObject pre,next;

    ObsUpdate preComponent;

    public bool moving = true;

    public bool active = false;

    public bool fall = false;


    // Use this for initialization
    void Start()
    {
        so = SocketObject.Instance;
        Ray ray = new Ray(transform.position, -transform.up);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, Mathf.Infinity, mask))
        {
            if (hit.transform.gameObject.CompareTag("Stage"))
            {
                pre = null;
                preComponent = null;
            }
            else
            {
                pre = hit.transform.gameObject;
                preComponent = pre.GetComponent<ObsUpdate>();
                if (preComponent.fall)
                {
                    fall = true;
                }
                else
                {
                    preComponent.next = this.gameObject;
                }
            }
            return;
        }
        fall = true;
        if (core == null)
        {
            core = this.gameObject;
        }
    }


    // Update is called once per frame
    void Update()
    {

        if (fall)
        {
            fallen();
            if(transform.position.y<-100)
                Destroy();
            return;
        }

        if (pre == null){
            if (transform.position.y == 1f)
            {
                return;
            }
            moving = true;
        }
        else if(preComponent.moving)
        {
            
            moving = true;
        }
        
        fallen();
        if (pre == null && moving == true)
        {
            if (transform.position.y < 1f)
            {
                transform.position = new Vector3(
                            transform.position.x,
                            1f,
                            transform.position.z
                        );
                moving = false;
            }
        }else if (Mathf.Abs(pre.transform.position.y - transform.position.y) < 1f)
        {
            transform.position = new Vector3(
                        transform.position.x,
                        pre.transform.position.y + 1f,
                        transform.position.z
                    );
            moving = false;
        }

        /*
        if (transform.CompareTag("fallenObstacle")) {
            //nothing
        }
        */


        //Debug.DrawRay(ray.origin, ray.direction, Color.red, Mathf.Infinity);

    }
	

	
    //落とす関数。今回は簡略化。
    private void fallen()
    {
        if (moving)
        {
            preSpeed += 0.1f;
            transform.Translate(0, -preSpeed * Time.deltaTime, 0);
        }
        else
        {
            preSpeed = 0;
        }
    }


	public void Destroy(){
        if (next != null)
        {
            if (pre == null)
            {
                next.GetComponent<ObsUpdate>().setPrevious(null);
            }
            else
            {
                next.GetComponent<ObsUpdate>().setPrevious(pre);
                preComponent.next = next;
            }
        }
        else if(pre!=null)
        {
            preComponent.next = null;
        }
		particle.GetComponent<Renderer> ().material = core.GetComponent<Renderer>().material;
		GameObject p = Instantiate (particle, transform.position, Quaternion.identity);
		Destroy (p, 2f);
		Destroy(this.gameObject);
	}

    public void setPrevious(GameObject g)
    {
        if (g != null)
        {
            pre = g;
            preComponent = g.GetComponent<ObsUpdate>();
            moving = true;
        }
        else
        {
            pre = null;
            preComponent = null;
        }
    }
}