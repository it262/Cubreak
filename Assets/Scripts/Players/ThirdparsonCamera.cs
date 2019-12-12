using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThirdparsonCamera : MonoBehaviour
{
  public GameObject player;
  [SerializeField] float distance = 45f;
  Camera cam;
  // Start is called before the first frame update
  void Start()
  {
    cam = Camera.main;
  }

  // Update is called once per frame
  void Update()
  {
    distance += Input.GetAxis("Mouse ScrollWheel");
    int x = (player.transform.position.x >= 0) ? 1 : -1;
    int z = (player.transform.position.z >= 0) ? 1 : -1;
    Vector3 pos = new Vector3(distance * x,distance, distance * z);
    cam.transform.position = Vector3.Lerp(cam.transform.position, player.transform.position + pos, 0.1f);
    cam.transform.LookAt(player.transform);
  }
}
