using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControls : MonoBehaviour {

	Board board;
	BlockSelection blockSelection;

	Block.Type cheatType = Block.Type.None;
	int keyboardDir = 1;

	bool boardInput = false;

	void Start () {
		board = GameObject.FindObjectOfType<Board>();
		blockSelection = GameObject.FindObjectOfType<BlockSelection>();

	}

	void CheckBlockSelectionInput(){
		int targetBlockNum = -1;

		// TEST
		//targetBlockNum = 1;

		if(Input.GetKeyDown(KeyCode.W)){
			targetBlockNum = 0;
		} else if(Input.GetKeyDown(KeyCode.E)){
			targetBlockNum = 1;
		} else if(Input.GetKeyDown(KeyCode.R)){
			targetBlockNum = 2;
		}

		if(targetBlockNum >= 0){

			blockSelection.SelectBlock(targetBlockNum);

			boardInput = true;
		}
	}

	void CheckBoardInput(){
		// TODO with multiple players need to be careful not to apply both moves on same frame

		int dir = keyboardDir;
		Block.Type type = cheatType;
		if(type == Block.Type.None)
			type = blockSelection.selectedType;

		#if false

		bool throwing = Input.GetKeyDown(KeyCode.UpArrow);
		if(Input.GetKeyDown(KeyCode.LeftArrow)){
			blockSelection.transform.position -= new Vector3(1, 0, 0);
		}
		if(Input.GetKeyDown(KeyCode.RightArrow)){
			blockSelection.transform.position += new Vector3(1, 0, 0);
		}

		int targetColumn = (int)blockSelection.transform.position.x + Board.board.width/2;

		#else

		int targetColumn = -1;


		if(Input.GetKeyDown(KeyCode.Q)){
			targetColumn = 0;
		} else if(Input.GetKeyDown(KeyCode.W)){
			targetColumn = 1;
		} else if(Input.GetKeyDown(KeyCode.E)){
			targetColumn = 2;
		} else if(Input.GetKeyDown(KeyCode.R)){
			targetColumn = 3;
		} else if(Input.GetKeyDown(KeyCode.T)){
			targetColumn = 4;
		}

		bool throwing = targetColumn != -1;

		#endif
		if(throwing) {
			board.ThrowBlock(type, targetColumn, dir);

			cheatType = Block.Type.None;
			boardInput = false;
			blockSelection.UpdateBlocks();
		}
	}
	
	void Update () {
		if(Input.GetKeyDown(KeyCode.Alpha1)){
			cheatType = Block.Type.Red;
		} else if(Input.GetKeyDown(KeyCode.Alpha2)){
			cheatType = Block.Type.Green;
		} else if(Input.GetKeyDown(KeyCode.Alpha3)){
			cheatType = Block.Type.Blue;
		}

		if(boardInput){
			CheckBoardInput();
		} else {
			CheckBlockSelectionInput();
		}
	}
}
