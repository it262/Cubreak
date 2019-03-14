using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleControll : MonoBehaviour {

    public GameObject obstaclePrefab;

    public int xTarget;
    public int zTarget;
	public Vector2 targetSection;
    public GameObject stage;

    public int x_width;
    public int z_width;
    public int y_width;

    GameObject[] tagObjects;

    public float speed;

	bool leady = false;

	GameObject so;


    // Use this for initialization
	void Start(){
		// -> FirstProcessing()
		so = GameObject.Find("SocketIO");
		stage = GameObject.Find("Stage");
	}

    // Update is called once per frame
    void Update () {

		if (so.GetComponent<DataWorker>().leady && !leady) {
			FirstProcessing ();
		}
			
        //obstacleがフィールド上になかったら新しいobstacleを召喚する。
        if (isnt_There("Obstacle")) {
            xTarget = (int)Random.Range(0, stage.GetComponent<Stage>().xSection);
            zTarget = (int)Random.Range(0, stage.GetComponent<Stage>().zSection);
            targetSection = stage.GetComponent<Stage>().TargetSection(xTarget, zTarget);
            x_width = (int)Random.Range(1, 3);
            z_width = (int)Random.Range(1, 2);
            y_width = (int)Random.Range(1, 3);
            for (int i = 0; i < x_width; i++) {
                for (int j = 0; j < z_width; j++){
                    for (int k = 0; k < y_width; k++) {
                        Instantiate(obstaclePrefab,
                            new Vector3(targetSection.x + i, 10f - j, targetSection.y/*z*/ - k),
                            Quaternion.identity);
                    }
                }
            }
        }
    }

	void FirstProcessing(){
		//obstacleの初期配置
		for (int i = 0; i < 10; i++) {
			xTarget = (int)Random.Range(0, stage.GetComponent<Stage>().xSection);
			zTarget = (int)Random.Range(0, stage.GetComponent<Stage>().zSection);
			targetSection = stage.GetComponent<Stage>().TargetSection(xTarget, zTarget);

			GameObject obs = (GameObject)Instantiate(obstaclePrefab,
				new Vector3(targetSection[0], 0.5f, targetSection[1]),
				Quaternion.identity);
			obs.gameObject.tag = "fallenObstacle";
		}
		leady = true;
		return;
	}


    //targetTagが"いない"時にtrueを返す。フィールド上にobstacleが残っているかどうかを判定するのに使う。
    bool isnt_There(string targetTag) {
		if (!leady)
			return false;
        tagObjects = GameObject.FindGameObjectsWithTag(targetTag);
        if (tagObjects.Length == 0){
            return true;
        }else {
            return false;
        }
    }
}
