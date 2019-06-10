using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class State{

	public int atk;
	public int dif;
	public int spd;
	public int life;

	public State(){
		atk = 50;
		dif = 50;
		spd = 50;
		life = 50;
	}

	public void atk_plus(){
		atk += 3;
		dif -= (dif > 1) ? 1 : 0;
		spd -= (spd > 1) ? 1 : 0;
	}
	public void dif_plus(){
		dif += 3;
		atk -= (atk > 1) ? 1 : 0;
		spd -= (spd > 1) ? 1 : 0;
	}
	public void spd_plus(){
		spd += 3;
		atk -= (atk > 1) ? 1 : 0;
		dif -= (dif > 1) ? 1 : 0;
	}
	public void life_plus(){
		life += 10;
	}

	public void black_minus(){
		spd -= (spd > 1) ? Random.Range(0,1) : 0;
		atk -= (atk > 1) ? Random.Range(0,1) : 0;
		dif -= (dif > 1) ? Random.Range(0,1) : 0;
	}

	//Dead → true
	public bool damage(int d){
		life -= d;
		return (life <= 0) ? true : false;
	}


}
