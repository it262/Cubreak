using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Stage : MonoBehaviour {

	static DataWorker dw;

	public GameObject ObsController;

	public bool debug = false;

    public int half_zSection = 5;
    public int half_xSection = 10;
    public int zSection;
    public int xSection;
    float zScale;
    float xScale;
    int[] zGridArray;
    int[] xGridArray;

	public List<Vector2> spawnPoints = new List<Vector2> ();

    // Use this for initialization
    void Start () {

		dw = DataWorker.Instance;

        zSection = half_zSection * 2;
        xSection = half_xSection * 2;
        zScale = zSection / 10.0f;
        xScale = xSection / 10.0f;
        
        transform.localScale = new Vector3(xScale, 1f, zScale);

        //targetObstacleを求めるための座標を格納する配列を作る。
        zGridArray = new int[zSection + 1];

        for (int i = 0; i < zGridArray.Length; i++) {
            zGridArray[i] = -1 * half_zSection + i;
        }

        xGridArray = new int[xSection + 1];

        for (int i = 0; i < xGridArray.Length; i++) {
            xGridArray[i] = -1 * half_xSection + i;
        }

		GameObject obs = (GameObject)Instantiate (ObsController, Vector3.zero, Quaternion.identity);
		obs.GetComponent<ObstacleControllSync> ().stage = this.gameObject;
		dw.InstanceObsCon = obs;

		List<Vector2> spawnPoints = new List<Vector2> ();
		spawnPoints.Add (TargetSection(0,0));
		spawnPoints.Add (TargetSection(xSection-1,zSection-1));
		spawnPoints.Add (TargetSection(0,zSection-1));
		spawnPoints.Add (TargetSection(xSection-1,0));
		dw.PlayerCreate (obs,spawnPoints);

	}
	
	// Update is called once per frame
	void Update () {
		
	}

    //obstacleの召喚の起点となるtargetObstacleのx,yを求めるための関数
	public Vector2 TargetSection(int targetX, int targetZ) {

		Vector2 result;
		result.x = (xGridArray[targetX] + xGridArray[targetX + 1]) / 2.0f;
		result.y = (zGridArray[targetZ] + zGridArray[targetZ + 1]) / 2.0f;

        return result;
    }
}
