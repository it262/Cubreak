using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClickObject : MonoBehaviour
{
    [SerializeField] private Texture2D _cursor = default;
    [SerializeField] private LayerMask mask = default;
    // Start is called before the first frame update
    void Start()
    {
        Cursor.SetCursor(_cursor, new Vector2(_cursor.width / 2, _cursor.height / 2), CursorMode.ForceSoftware);
    }

    // Update is called once per frames
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit, mask))
            {
                Debug.Log(hit.collider.gameObject.name);
            }
        }
    }
}
