using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Game : MonoBehaviour {

	public static Game instance {
		get; set;
	}

	public GameObject failScreen;

	Game(){
		instance = this;
	}

	void Start () {
		
	}

	public void CheckForFailure(){
		if(Board.instance.InFailureState()){
			failScreen.SetActive(true);
		}
	}
	
	void Update () {
		
	}

}
