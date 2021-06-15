using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClickObject : MonoBehaviour
{
  [SerializeField] Texture2D cursor;
  [SerializeField] LayerMask mask;
  RaycastHit hit;
  // Start is called before the first frame update
  void Start()
  {
    Cursor.SetCursor(cursor, new Vector2(cursor.width / 2, cursor.height / 2), CursorMode.ForceSoftware);
  }

  // Update is called once per frames
  void Update()
  {
    if(Input.mouse)
    if (Input.GetMouseButtonDown(0))
    {
      Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
      if (Physics.Raycast(ray, out hit,mask))
      {
        Debug.Log(hit.collider.gameObject.name);
      }
    }
  }
}
