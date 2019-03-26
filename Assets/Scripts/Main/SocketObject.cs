﻿#region License
/*
 * TestSocketIO.cs
 *
* The MIT License
*
* Copyright (c) 2014 Fabio Panettieri
*
* Permission is hereby granted, free of charge, to any person obtaining a copy
* of this software and associated documentation files (the "Software"), to deal
* in the Software without restriction, including without limitation the rights
* to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
* copies of the Software, and to permit persons to whom the Software is
* furnished to do so, subject to the following conditions:
	*
	* The above copyright notice and this permission notice shall be included in
	* all copies or substantial portions of the Software.
	*
	* THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
	* IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
	* FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
	* AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
	* LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
	* OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
	* THE SOFTWARE.
	*/
	#endregion

using System.Collections;
using UnityEngine;
using SocketIO;
using System.Collections.Generic;
using System;
using UnityEngine.UI;

public class SocketObject : SingletonMonoBehavior<SocketObject>
{
	private SocketIOComponent socket;

	string url = "ws://52.194.134.160:1337/socket.io/?EIO=4&transport=websocket";

	public string id,name;

	public bool connecting = false;

	void Awake(){
		GameObject[] sockets = GameObject.FindGameObjectsWithTag ("SocketObject");
		if (sockets.Length > 1) {
			Destroy (this.gameObject);
		}

		Cursor.lockState = CursorLockMode.None;
		Cursor.SetCursor (null,Vector2.zero,CursorMode.ForceSoftware);
	}

	public void Start() 
	{
		DontDestroyOnLoad (this.gameObject);
	}

	public void joinRoom(string roomName){
		var data = new Dictionary<string,string>();
		data["name"] = name;
		data["room"] = roomName;
		socket.Emit ("updateRoom",new JSONObject(data));
	}

	public void leaveRoom(){
		var data = new Dictionary<string,string>();
		data["name"] = name;
		data["room"] = "";
		socket.Emit ("updateRoom",new JSONObject(data));
	}

	public void EmitMessage(string s,Dictionary<string,string> d){
		if (connecting) {
			socket.Emit (s, new JSONObject (d));
		} else {
			Debug.Log ("[ERROR]オンライン状態ではありません");
		}
	}

	public void Connect(){

		if (!connecting) {
			
			try {
			
				GetComponent<SocketIOComponent> ().url = url;
				GetComponent<SocketIOComponent> ().Standby ();

				socket = GetComponent<SocketIOComponent> ();

				socket.On ("open", SocketOpen);
				socket.On ("ID", ReceiveID);
				socket.On ("UpdateRoom", UpdateRoom);
				socket.On ("GetRooms", GetRooms);
				socket.On ("PingName",PingName);
				socket.On ("PongName", PongName);
				socket.On ("Quick", Quick);
				socket.On ("Leady", Leady);
				socket.On ("GameStart", GameStart);
				socket.On ("Message", Message);
				socket.On ("Pos", Pos);
				socket.On ("Rot", Rot);
				socket.On ("Hit", Hit);
				socket.On ("PlayerEliminate", PlayerEliminate);
				socket.On ("FirstObs", FirstObs);
				socket.On ("Obs", Obs);
				socket.On ("DestroyObs", DestroyObs);
				socket.On ("StateUpdate", StateUpdate);
				socket.On ("Dead", Dead);
				socket.On ("HeartBeat", HeartBeat);
				socket.On ("error", ReceiveError);
				socket.On ("close", SocketClose);

				socket.Connect ();

			} catch (Exception e) {
				connecting = false;
			}
		}
	}

	public void SocketOpen(SocketIOEvent e)
	{
		if (!connecting) {
			connecting = true;
			var data = new Dictionary<string,string> ();
			data ["name"] = WWW.EscapeURL (name);
			Debug.Log (data ["name"]);
			socket.Emit ("ID", new JSONObject (data));
		}

		Debug.Log("[SocketIO] Open received: " + e.name + " " + e.data);
	}

	public void ReceiveError(SocketIOEvent e)
	{
		connecting = false;
		Debug.Log("[SocketIO] Error received: " + e.name + " " + e.data);
	}

	public void SocketClose(SocketIOEvent e)
	{
		connecting = false;
		Debug.Log("[SocketIO] Close received: " + e.name + " " + e.data);
	}

	public void ReceiveID(SocketIOEvent e)
	{
		Dictionary<string,string> d = new JSONObject (e.data.ToString()).ToDictionary();
		if (id.Equals("")) {
			id = d ["id"];
			GetComponent<DataWorker> ().MAX = int.Parse(d["max"]);
			Debug.Log ("[SocketIO] ID received: " + e.name + " " + e.data);
		}
		//ルーム情報のリクエスト
		//socket.GetComponent<SocketObject> ().EmitMessage ("GetRooms",new Dictionary<string,string>());
	}
		
	public void UpdateRoom(SocketIOEvent e){
		//他プレイヤーへのROOM更新要請
		GetComponent<DataWorker>().roomQue.Enqueue(new JSONObject(e.data.ToString ()).ToDictionary());
	}

	public void GetRooms(SocketIOEvent e){
		GetComponent<DataWorker>().roomState = e.data;
		Debug.Log (e.data);
	}

	public void PingName(SocketIOEvent e){
		var data = new Dictionary<string,string> ();
		data ["name"] = WWW.EscapeURL(name);
		EmitMessage ("PongName", data);
	}

	public void PongName(SocketIOEvent e){
		Dictionary<string,string> d = new JSONObject (e.data.ToString ()).ToDictionary ();
		GetComponent<DataWorker> ().myRoom.member [d ["id"]] = WWW.UnEscapeURL (d ["name"]);
	}

	public void Quick(SocketIOEvent e){
		GetComponent<DataWorker>().roomState = e.data;
		GetComponent<DataWorker> ().wait = false;
	}

	public void Leady(SocketIOEvent e){
		GetComponent<DataWorker>().leady = true;
		GetComponent<DataWorker> ().wait = false;
		var data = new JSONObject (e.data.ToString ());
		Room r = new Room (data ["name"].ToString());
		foreach (KeyValuePair<string,string> d in data ["sockets"].ToDictionary()) {
			r.member.Add(d.Key,d.Value);
		}
		r.cnt = int.Parse (data["length"].ToString ());
		GetComponent<DataWorker> ().myRoom = r;
		Debug.Log ("[入室]"+r.roomName);
	}
		
	public void GameStart(SocketIOEvent e){
		//GetComponent<DataWorker> ().startFlug = true;
	}

	public void Message(SocketIOEvent e)
	{
		GetComponent<DataWorker>().chatQue.Enqueue(new JSONObject(e.data.ToString ()).ToDictionary());
	}

	public void Pos(SocketIOEvent e)
	{
		Dictionary<string,string> d = new JSONObject (e.data.ToString ()).ToDictionary ();
		GetComponent<DataWorker>().posSync.Add(d["id"],new Vector3(float.Parse(d["x"]),float.Parse(d["y"]),float.Parse(d["z"])));
        Debug.Log("ポジション受信");
	}

	public void Rot(SocketIOEvent e)
	{
		Dictionary<string,string> d = new JSONObject (e.data.ToString ()).ToDictionary ();
		GetComponent<DataWorker>().rotSync.Add(d["id"],new Vector2(float.Parse(d["headY"]),float.Parse(d["bodyY"])));
        Debug.Log("ローテーション受信");
    }

	public void Hit(SocketIOEvent e)
	{
		Dictionary<string,string> d = new JSONObject (e.data.ToString ()).ToDictionary ();
		GetComponent<DataWorker>().hitQue.Enqueue(new JSONObject(e.data.ToString ()).ToDictionary());
	}

	public void PlayerEliminate(SocketIOEvent e){
		Dictionary<string,string> d = new JSONObject (e.data.ToString ()).ToDictionary ();
		GetComponent<DataWorker> ().elimQue.Enqueue (new JSONObject(e.data.ToString ()).ToDictionary());
	}

	public void FirstObs(SocketIOEvent e)
	{
		Debug.Log ("初期オブジェクト位置受信");
		GetComponent<DataWorker>().InstanceObsCon.GetComponent<ObstacleControllSync> ().first = new JSONObject (e.data.ToString ()).ToDictionary ();
	}

	public void Obs(SocketIOEvent e)
	{
		Debug.Log ("OBSデータ受信");
		Dictionary<string,string> d = new JSONObject (e.data.ToString ()).ToDictionary ();
		GetComponent<DataWorker>().InstanceObsCon.GetComponent<ObstacleControllSync> ().obs.Enqueue(d["json"]);
	}

	public void DestroyObs(SocketIOEvent e)
	{
		GetComponent<DataWorker>().InstanceObsCon.GetComponent<ObstacleControllSync> ().victim.Enqueue(new JSONObject (e.data.ToString ()).ToDictionary ());
	}

	public void StateUpdate(SocketIOEvent e)
	{
		GetComponent<DataWorker>().InstanceObsCon.GetComponent<ObstacleControllSync> ().state[new JSONObject (e.data.ToString ()).ToDictionary ()["id"]] = new JSONObject (e.data.ToString ()).ToDictionary ()["state"];
	}

	public void Dead(SocketIOEvent e)
	{
		GetComponent<DataWorker>().exclusion(new JSONObject(e.data.ToString ()).ToDictionary()["id"]);
	}

	public void HeartBeat(SocketIOEvent e)
	{
		GetComponent<DataWorker>().heatbeat.Add(new JSONObject(e.data.ToString ()).ToDictionary()["id"],true);
	}

	public void Disconnection(){
		socket.Close ();
		connecting = false;
		name = "";
		id = "";
	}

}