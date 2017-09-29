﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Board : MonoBehaviour {

	public static Board instance {
		get; set;
	}

	public GameObject blockPrefab;
	public GameObject midLine;

	public int width;
	public int height;

	float delayDropTime = 0;

	Block[,] blocks;

	Board(){
		instance = this;
	}
		
	void Start () {
		blocks = new Block[height, width];

		transform.localPosition += new Vector3(.5f - width/2.0f, .5f - height/2.0f, 0);

		midLine.transform.localScale = new Vector3(width, .02f, 1);
		midLine.transform.localPosition = new Vector3(width/2, height/2 - .5f, 0);

		Populate();
	}

	public bool InFailureState(){
		for(int i = 0; i < width; i++){
			if(blocks[0, i] != null || blocks[height-1, i] != null)
				return true;
		}
		return false;
	}

	void Populate(){
		int rowsToPopulate = 8;
		for(int dir = -1; dir <= 1; dir += 2){
			for(int i = 0; i < rowsToPopulate / 2; i++){
				int row = height / 2 + dir * i;
				if(dir == -1)
					row--;

				for(int j = 0; j < width; j++){

					int numGroups = FindLargeGroups().Count;
					Block block = null;

					for(int k = 0; k < 100; k++){
						if(block){
							DestroyBlock(block);
						}

						block = CreateBlock(Utils.RandomEnum<Block.Type>(1));
						SetBlockPosition(block, row, j, Block.MoveType.Instant);

						if(FindLargeGroups().Count == numGroups){
//							Debug.Log("Found solution");
							break;
						}
					}
				}
			}
		}
	}

	Block CreateBlock(Block.Type type){
		GameObject blockObject = GameObject.Instantiate(blockPrefab);
		Block block = blockObject.GetComponent<Block>();
		block.transform.parent = transform;

		block.SetType(type);

		return block;
	}

	void DestroyBlock(Block block){
		GameObject.Destroy(block.gameObject);
		blocks[block.row, block.col] = null;
	}

	void SetBlockPosition(Block block, int row, int col, Block.MoveType moveType = Block.MoveType.None){
		block.SetLocation(row, col, moveType);
		blocks[row, col] = block;
	}

	public class Group {

		public int size = 0;

		public List<Block> blocks;

		public Group(Block firstBlock){
			blocks = new List<Block>();
			blocks.Add(firstBlock);
			firstBlock.group = this;
			size = 1;
		}

		public void Merge(Group group){
			List<Block> tempList = group.blocks;
			group.blocks = new List<Block>();
			group.size = 0;

			foreach(Block block in tempList){
				block.group = this;
				blocks.Add(block);
				size++;
			}
		}
	}

	List<Group> FindGroups(){
		List<Group> groups = new List<Group>();

		foreach (Block block in blocks){
			if(block){
				block.group = new Board.Group(block);
				groups.Add(block.group);
			}
		}

		for(int i = 0; i < height; i++){
			for(int j = 0; j < width; j++){
				Block current = blocks[i, j];
				if(current){
					if(i > 0){
						Block upper = blocks[i - 1, j];
						if(upper && current.type == upper.type){
							upper.group.Merge(current.group);
						}
					}
					if(j > 0){
						Block left = blocks[i, j - 1];
						if(left && current.type == left.type){
							left.group.Merge(current.group);
						}
					}
				}
			}
		}

		return groups;
	}

	List<Group> FindLargeGroups(){
		List<Group> groups = FindGroups();
		List<Group> largeGroups = new List<Group>();

		foreach(Group group in groups){
			if(group.size >= 3){
				largeGroups.Add(group);
			}
		}

		return largeGroups;
	}

//	bool HasConnectedBlocks(){
//		List<Group> groups = FindConnectedBlocks();
//		return groups.Count > 0;
//	}

	void RemoveConnectedBlocks(){
		List<Group> groups = FindLargeGroups();

		foreach(Group group in groups){
//			Debug.Log("Found a group of " + group.size + " (" + group.blocks[0].type.ToString() + ")");
			foreach(Block block in group.blocks){
				DestroyBlock(block);
			}
		}
			
		SlideBlocksDown();
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

	void PrintBoard(){
		string result = "";
		for(int i = height-1; i >= 0; i--){
			for(int j = 0; j < width; j++){
				Block block = blocks[i, j];
				if(block){
					int type = (int) block.type;
					result += type;
				} else {
					result += '_';
				}
			}
			result += "\n";
		}
		Debug.Log(result);
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
			PrintBoard();
		}

		if(delayDropTime <= 0){
			
			bool blocksMoving = false;
			foreach(Block block in blocks){
				if(block && block.moving){
					blocksMoving = true;
					break;
				}
			}

			if(!blocksMoving){
				RemoveConnectedBlocks();
			}

		} else {
			delayDropTime -= Time.deltaTime;
		}
	}
}