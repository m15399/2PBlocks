using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControls : MonoBehaviour {

	int keyboardDir = 1;

	public ColumnSelector columnSelector;
	int targetColumn;

	void Start () {
		targetColumn = Board.instance.width / 2;
	}

	Block.Type FeedBlock(){
		Block.Type type = columnSelector.GetBlockType();
		Block.Type nextType = BlockFeed.instance.PopNextBlock();
		columnSelector.SetBlockType(nextType);
		return type;
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
			Block.Type type = FeedBlock();
			Board.instance.ThrowBlock(type, targetColumn, dir);

			Game.instance.CheckForFailure();
		}
	}
	
	void Update () {
		if(Input.GetKeyDown(KeyCode.Alpha1)){
			columnSelector.SetBlockType(Block.Type.Red);
		} else if(Input.GetKeyDown(KeyCode.Alpha2)){
			columnSelector.SetBlockType(Block.Type.Green);
		} else if(Input.GetKeyDown(KeyCode.Alpha3)){
			columnSelector.SetBlockType(Block.Type.Blue);
		}

		CheckBoardInput();

		Vector3 tp = columnSelector.transform.localPosition;
		tp.x = targetColumn;
		columnSelector.transform.localPosition = tp;
	}
}
