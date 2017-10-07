using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BoardEvents;
using System.Reflection;
using System;

public class Board : MonoBehaviour {

	public static Board instance {
		get; set;
	}

	public GameObject blockPrefab;
	public GameObject midLine;

	public int width;
	public int height;

	float delayDropTime = 0;

	private Block[,] blocks;

	Board(){
		instance = this;
	}
		
	void Start () {
		transform.localPosition += new Vector3(.5f - width/2.0f, .5f - height/2.0f, 0);
		midLine.transform.localScale = new Vector3(width, .02f, 1);
		midLine.transform.localPosition = new Vector3(width/2, height/2 - .5f, 0);

		blocks = new Block[height, width];

		LogicalBoard lb = new LogicalBoard(blocks);
		lb.Populate();
		lb.ApplyEvents(this);
	}

	//
	// Events
	//

	public void ApplyEventBase(BoardEvent e){ 
		Type type = e.GetType();
		MethodInfo method = typeof(Board).GetMethod("ApplyEvent", new Type[] { type });
		method.Invoke(this, new object[] { e });
	}

	public void ApplyEvent(SpawnEvent e){
		Block block = CreateBlock(e.type);
		SetBlockPosition(block, e.row, e.col, Block.MoveType.None);
	}

	public void ApplyEvent(MoveEvent e){
		SetBlockPosition(blocks[e.row, e.col], e.row2, e.col2, Block.MoveType.None);
	}

	// // // //

	public bool InFailureState(){
		return new LogicalBoard(blocks).InFailureState();
	}

	Block CreateBlock(Block.Type type){
		GameObject blockObject = GameObject.Instantiate(blockPrefab);
		Block block = blockObject.GetComponent<Block>();
		block.transform.parent = transform;

		block.SetType(type);

		return block;
	}

	void DestroyBlock(Block block){
		if(block){
			GameObject.Destroy(block.gameObject);
			if(OnBoard(block)){
				blocks[block.row, block.col] = null;
			}
		}
	}

	void SetBlockPosition(Block block, int row, int col, Block.MoveType moveType = Block.MoveType.None){
		if(block){
			if(OnBoard(block)){
				blocks[block.row, block.col] = null;
			}
			block.SetLocation(row, col, moveType);
			block.onBoard = true;
		}

		Block existing = blocks[row, col];
		if(existing != null){
			existing.row = -1;
			existing.col = -1;
			existing.onBoard = false;
		}
		blocks[row, col] = block;
	}

	void RemoveFromBoard(Block block){
		if(block && OnBoard(block)){
			block.onBoard = false;
			blocks[block.row, block.col] = null;
		}
	}

	bool OnBoard(Block block){
//		return block && block.row >= 0 && block.row < height && block.col >= 0 && block.col < width;
		return block && block.onBoard;
	}
//
//	void RemoveConnectedBlocks(){
//		List<Group> groups = FindLargeGroups();
//
//		foreach(Group group in groups){
////			Debug.Log("Found a group of " + group.size + " (" + group.blocks[0].type.ToString() + ")");
//			foreach(Block block in group.blocks){
//				DestroyBlock(block);
//			}
//		}
//			
//		SlideBlocksDown();
//	}

	int NumLargeGroups(){
		LogicalBoard lb = new LogicalBoard(blocks);
		return lb.NumLargeGroups();
	}

	void SlideBlock(Block block, int midHeight, int row, int col, int dir){
		if(!block)
			return;
		
		if(dir < 0){
			if(row > midHeight){
				if (blocks[row - 1, col] == null){
					blocks[row, col] = null;
					SetBlockPosition(block, row - 1, col);

					SlideBlock(block, midHeight, row - 1, col, dir);
				}
			}
		} else {
			if(row < midHeight - 1){
				if (blocks[row + 1, col] == null){
					blocks[row, col] = null;
					SetBlockPosition(block, row + 1, col);

					SlideBlock(block, midHeight, row + 1, col, dir);
				}
			}
		}
	}

	void SlideBlocksDown(){
		int midHeight = height / 2;

		for(int i = midHeight - 2; i >= 0; i--){
			for(int j = 0; j < width; j++){
				SlideBlock(blocks[i, j], midHeight, i, j, 1);
			}
		}

		for(int i = midHeight + 1; i < height; i++){
			for(int j = 0; j < width; j++){
				SlideBlock(blocks[i, j], midHeight, i, j, -1);
			}
		}

		delayDropTime = .5f;
	}

	public void ThrowBlock(Block.Type type, int targetColumn, int dir){
		int targetRow = -1;
		int midHeight = height/2;

		if(dir > 0){
			targetRow = midHeight - 1;
			for(int i = 0; i < midHeight; i++){
				if(blocks[i, targetColumn] != null){
					targetRow = i-1;
					break;
				}
			}
		} else {
			// TODO
		}

		if(targetRow >= 0 && targetRow < height){
			Block block = CreateBlock(type);
			if(dir > 0){
				block.transform.localPosition = new Vector3(targetColumn, 0, 0);
			} else {
				// TODO
			}
			SetBlockPosition(block, targetRow, targetColumn, Block.MoveType.Fast);
		}
	}

	void LateUpdate(){
		if(Input.GetKeyDown(KeyCode.P)){
			var lb = new LogicalBoard(blocks);
			lb.SpawnBlockRow();
			lb.PrintBoard();
			lb.ApplyEvents(this);
		}

		if(Input.GetKeyDown(KeyCode.T)){
		
		}

		// Drop if delay is up, and no blocks are moving
		//
		if(delayDropTime <= 0){
			
			bool blocksMoving = false;
			foreach(Block block in blocks){
				if(block && block.moving){
					blocksMoving = true;
					break;
				}
			}

			if(!blocksMoving){
//				 RemoveConnectedBlocks();
			}

		} else {
			delayDropTime -= Time.deltaTime;
		}
	}

}
