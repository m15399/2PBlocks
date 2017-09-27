using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControls : MonoBehaviour {

	int keyboardDir = 1;

	public GameObject targetIndicator;
	int targetColumn;

	void Start () {
		targetColumn = Board.instance.width / 2;
	}

	void CheckBoardInput(){
		// TODO with multiple players need to be careful not to apply both moves on same frame

		int dir = keyboardDir;

		bool throwing = Input.GetKeyDown(KeyCode.UpArrow);
		if(Input.GetKeyDown(KeyCode.LeftArrow)){
			targetColumn--;
		}
		if(Input.GetKeyDown(KeyCode.RightArrow)){
			targetColumn++;
		}

		targetColumn = Mathf.Clamp(targetColumn, 0, Board.instance.width-1);
			
		if(throwing) {
			Block.Type type = UpcomingBlocks.instance.PopNextBlock();
			Board.instance.ThrowBlock(type, targetColumn, dir);
		}
	}
	
	void Update () {
		if(Input.GetKeyDown(KeyCode.Alpha1)){
			UpcomingBlocks.instance.CheatNextBlockType(Block.Type.Red);
		} else if(Input.GetKeyDown(KeyCode.Alpha2)){
			UpcomingBlocks.instance.CheatNextBlockType(Block.Type.Green);
		} else if(Input.GetKeyDown(KeyCode.Alpha3)){
			UpcomingBlocks.instance.CheatNextBlockType(Block.Type.Blue);
		}

		CheckBoardInput();

		Vector3 tp = targetIndicator.transform.localPosition;
		tp.x = targetColumn;
		targetIndicator.transform.localPosition = tp;
	}
}
