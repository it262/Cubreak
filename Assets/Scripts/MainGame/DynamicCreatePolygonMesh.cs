using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(MeshFilter))]
public class DynamicCreatePolygonMesh : MonoBehaviour
{
	[SerializeField] private Material _mat;

	[SerializeField] private int VerticesCount = 5;

	[SerializeField] private float ATK = 1f,DEF = 1f,SPD = 1f;

    // Start is called before the first frame update
    void Start()
	{
		if (VerticesCount < 3) {
			Debug.Log ("頂点が2以下です");
			return;
		}

		List<Vector3> vertices = new List<Vector3>();
		List<int> triamgles = new List<int> ();

		vertices.Add (Vector3.zero);

		for (int i = 1; i <= this.VerticesCount; i++) {
			float rad = (90f - (360f / (float)this.VerticesCount) * (i - 1)) * Mathf.Deg2Rad;
			float x = Mathf.Cos (rad);
			float y = Mathf.Sin (rad);
			vertices.Add (new Vector3 (x, y, 0));
			triamgles.Add (0);
			triamgles.Add (i);
			triamgles.Add (i == this.VerticesCount ? 1 : i + 1);
		}

		Mesh mesh = new Mesh ();
		mesh.vertices = vertices.ToArray ();
		mesh.triangles = triamgles.ToArray ();

		MeshFilter filter = GetComponent<MeshFilter> ();
		filter.sharedMesh = mesh;

		MeshRenderer renderer = GetComponent<MeshRenderer> ();
		renderer.material = _mat;
    }

    // Update is called once per frame
    void Update()
    {
		if (VerticesCount < 3) {
			Debug.Log ("頂点が2以下です");
			return;
		}

		List<Vector3> vertices = new List<Vector3>();
		List<int> triamgles = new List<int> ();

		vertices.Add (Vector3.zero);

		for (int i = 1; i <= this.VerticesCount; i++) {
			float rad = (90f - (360f / (float)this.VerticesCount) * (i - 1)) * Mathf.Deg2Rad;
			float x = Mathf.Cos (rad);
			float y = Mathf.Sin (rad);
			vertices.Add (new Vector3 (x*((i==1)? ATK:(i==2)? DEF:SPD), y*((i==1)? ATK:(i==2)? DEF:SPD), 0));
			triamgles.Add (0);
			triamgles.Add (i);
			triamgles.Add (i == this.VerticesCount ? 1 : i + 1);
		}

		Mesh mesh = new Mesh ();
		mesh.vertices = vertices.ToArray ();
		mesh.triangles = triamgles.ToArray ();

		MeshFilter filter = GetComponent<MeshFilter> ();
		filter.sharedMesh = mesh;

		MeshRenderer renderer = GetComponent<MeshRenderer> ();
		renderer.material = _mat;
    }
}
