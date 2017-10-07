using System;
using System.Collections.Generic;
using UnityEngine;
using BoardEvents;

public class LogicalBoard {

	public class BlockData {
		public int row = -1, col = -1;
		public Block.Type type;
	}

	public class BoardState {
		public BlockData[,] blocks;
		public List<BoardEvent> events;
	}

	public int width, height;

	// TODO instead of using null, we could have BlockData with type == None
	//      Would mean no null checking anywhere

	public BlockData[,] blocks;
	List<BoardEvent> events = new List<BoardEvent>();

	// TODO may eventually be possible to have a perisistent logicalboard rather than constantly copying from the main board
	public LogicalBoard (Block[,] blocksIn) {
		height = blocksIn.GetLength(0);
		width = blocksIn.GetLength(1);

		blocks = new BlockData[height, width];
		for(int i = 0; i < height; i++){
			for(int j = 0; j < width; j++){
				blocks[i, j] = null;

				Block b = blocksIn[i, j];
				BlockData b2 = null;

				if(b){
					b2 = new BlockData();
					SetBlockPosition(b2, i, j);
					b2.type = b.type;
				}
			}
		}
	}

	public LogicalBoard (LogicalBoard board) {
		height = board.height;
		width = board.width;

		blocks = new BlockData[height, width];
		Array.Copy(board.blocks, blocks, blocks.Length);
	}

	public void ResetEvents(){
		events = new List<BoardEvent>();
	}

	public void ApplyEvents(Board board){
		Debug.Log("Applying events: " + events.Count);
		foreach(BoardEvent e in events){
			board.ApplyEventBase(e);
		}
		ResetEvents();
	}

	void PushEvent(BoardEvent e){
		events.Add(e);
	}

	BoardState SaveState(){
		BoardState ret = new BoardState();
		ret.blocks = new BlockData[height, width];
		Array.Copy(blocks, ret.blocks, blocks.Length);

		ret.events = new List<BoardEvent>(events);

		return ret;
	}

	void RestoreState(BoardState state){
		Array.Copy(state.blocks, blocks, blocks.Length);
		events = new List<BoardEvent>(state.events);
	}

	BlockData CreateBlock(){
		BlockData block = new BlockData();
		block.type = Utils.RandomEnum<Block.Type>(1);
		return block;
	}

	public bool InFailureState(){
		for(int i = 0; i < width; i++){
			if(blocks[0, i] != null || blocks[height-1, i] != null)
				return true;
		}
		return false;
	}

	public void Populate(){
		int rowsToPopulate = 8;
		for(int dir = -1; dir <= 1; dir += 2){
			for(int i = 0; i < rowsToPopulate / 2; i++){
				int row = height / 2 + dir * i;
				if(dir == -1)
					row--;

				for(int j = 0; j < width; j++){

					int numGroups = NumLargeGroups();
					BlockData block = null;

					for(int k = 100; k >= 0; k--){
						BoardState saved = SaveState();

						block = CreateBlock();

						SetBlockPosition(block, row, j);

						// TODO incorporate this into CreateBlock
						PushEvent(new SpawnEvent(block.type, block.row, block.col));

						if(k == 0 || NumLargeGroups() == numGroups){
							break;
						}

						RestoreState(saved);
					}
				}
			}
		}
	}

	// TODO need to send a destroy event
//	public void DestroyBlock(BlockData block){
//		if(OnBoard(block)){
//			blocks[block.row, block.col] = null;
//		}
//	}

	public void SetBlockPosition(BlockData block, int row, int col){
		if(OnBoard(block)){
			blocks[block.row, block.col] = null;

			PushEvent(new MoveEvent(block.row, block.col, row, col));
		}

		if(block != null){
			block.row = row;
			block.col = col;
		}

		if(OnBoard(row, col)){
			blocks[row, col] = block;
		}
	}

	public bool OnBoard(int row, int col){
		return row >= 0 && row < height && col >= 0 && col < width;
	}

	public bool OnBoard(BlockData block){
		return block != null && OnBoard(block.row, block.col);
	}

	public bool Occupied(int row, int col, bool offBoardIsOccupied){
		bool onBoard = OnBoard(row, col);
		if(offBoardIsOccupied && !onBoard){
			return true;
		}
		return GetBlock(row, col) != null;
	}

	public BlockData GetBlock(int row, int col){
		if(OnBoard(row, col)){
			return blocks[row, col];
		}
		return null;
	}

	public int NumLargeGroups(){
		BoardGroups groups = new BoardGroups(blocks);
		return groups.FindLargeGroups().Count;
	}

//	void RemoveConnectedBlocks(){
//		List<Group> groups = FindLargeGroups();
//
//		foreach(Group group in groups){
//			//			Debug.Log("Found a group of " + group.size + " (" + group.blocks[0].type.ToString() + ")");
//			foreach(BlockData block in group.blocks){
//				DestroyBlock(block);
//			}
//		}
//
//		SlideBlocksDown();
//	}
//
//	void SlideBlockDown(BlockData block, int row, int col, int dir){
//		int midHeight = height/2;
//
//		if(block == null)
//			return;
//
//		if(dir < 0){
//			if(row > midHeight){
//				if (blocks[row - 1, col] == null){
//					blocks[row, col] = null;
//					SetBlockPosition(block, row - 1, col);
//
//					SlideBlockDown(block, row - 1, col, dir);
//				}
//			}
//		} else {
//			if(row < midHeight - 1){
//				if (blocks[row + 1, col] == null){
//					blocks[row, col] = null;
//					SetBlockPosition(block, row + 1, col);
//
//					SlideBlockDown(block, row + 1, col, dir);
//				}
//			}
//		}
//	}
//
//	void SlideBlocksDown(){
//		int midHeight = height / 2;
//
//		for(int i = midHeight - 2; i >= 0; i--){
//			for(int j = 0; j < width; j++){
//				SlideBlockDown(blocks[i, j], i, j, 1);
//			}
//		}
//
//		for(int i = midHeight + 1; i < height; i++){
//			for(int j = 0; j < width; j++){
//				SlideBlockDown(blocks[i, j], i, j, -1);
//			}
//		}
//	}

	bool PushBlock(BlockData block, int rowDir, int colDir){
		if(block == null)
			return false;
		
		int row = block.row;
		int col = block.col;
		int row2 = row + rowDir;
		int col2 = col + colDir;

		if(Occupied(row2, col2, false)){
			bool pushedNeighbor = PushBlock(GetBlock(row2, col2), rowDir, colDir);
			if(pushedNeighbor){
				return PushBlock(block, rowDir, colDir);
			} else {
				return false;
			}
		} else {
			SetBlockPosition(block, row2, col2);
			return true;
		}
	}

//	public void ThrowBlock(Block.Type type, int targetColumn, int dir){
//		int targetRow = -1;
//		int midHeight = height/2;
//
//		if(dir > 0){
//			targetRow = midHeight - 1;
//			for(int i = 0; i < midHeight; i++){
//				if(blocks[i, targetColumn] != null){
//					targetRow = i-1;
//					break;
//				}
//			}
//		} else {
//			// TODO
//		}
//
//		if(targetRow >= 0 && targetRow < height){
//			BlockData block = new BlockData();
//			block.type = type;
//			SetBlockPosition(block, targetRow, targetColumn);
//		}
//	}

	public void SpawnBlockRow(){

		for(int i = 0; i < width; i++){
			PushBlock(GetBlock(height/2 - 1, i), -1, 0);
		}
			
		for(int i = 0; i < width; i++){
			int numGroups = NumLargeGroups();

			for(int j = 10; j >= 0; j--){
				BoardState saved = SaveState();

				// TODO move to its a Spawn method
				BlockData block = CreateBlock();
				SetBlockPosition(block, height/2 - 1, i);

				PushEvent(new SpawnEvent(block.type, block.row, block.col));

				if(j == 0 || NumLargeGroups() == numGroups){
					break;
				}

				RestoreState(saved);
			}
		}
	}

	public void PrintBoard(){
		string result = "";
		for(int i = height-1; i >= 0; i--){
			for(int j = 0; j < width; j++){
				BlockData block = blocks[i, j];
				if(block != null){
					Debug.Assert(block.row == i);
					Debug.Assert(block.col == j);
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
}
