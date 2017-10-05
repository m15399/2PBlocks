using System;
using System.Collections.Generic;
using UnityEngine;
using BoardEvents;

public class LogicalBoard {

	public class BlockData {
		public int row = -1, col = -1;
		public Block.Type type;
		public Group group;
	}

	public int width, height;
	public BlockData[,] blocks;

	List<BlockData[,]> undos = new List<BlockData[,]>();
	List<int> eventCountsForUndos = new List<int>();

	List<BoardEvent> events = new List<BoardEvent>();

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

	void PushState(){
		BlockData[,] blocksCopy = new BlockData[height, width];
		Array.Copy(blocks, blocksCopy, blocks.Length);
		undos.Add(blocksCopy);

		eventCountsForUndos.Add(events.Count);
	}

	public void Undo(){
		Array.Copy(undos[undos.Count - 1], blocks, blocks.Length);
		undos.RemoveAt(undos.Count - 1);

		int nCounts = eventCountsForUndos.Count;
		int eventMark = eventCountsForUndos[nCounts - 1];
		eventCountsForUndos.RemoveAt(nCounts - 1);

//		Debug.Log("Undo is removing " + (events.Count - eventMark) + " events");
		events.RemoveRange(eventMark, events.Count - eventMark);
	}

	BlockData CreateBlock(){
		BlockData block = new BlockData();
		block.type = Utils.RandomEnum<Block.Type>(1);
		return block;
	}

//	public bool InFailureState(){
//		for(int i = 0; i < width; i++){
//			if(blocks[0, i] != null || blocks[height-1, i] != null)
//				return true;
//		}
//		return false;
//	}
//
//	public void Populate(){
//		int rowsToPopulate = 8;
//		for(int dir = -1; dir <= 1; dir += 2){
//			for(int i = 0; i < rowsToPopulate / 2; i++){
//				int row = height / 2 + dir * i;
//				if(dir == -1)
//					row--;
//
//				for(int j = 0; j < width; j++){
//
//					int numGroups = FindLargeGroups().Count;
//					BlockData block = null;
//
//					for(int k = 0; k < 100; k++){
//						if(block != null){
//							DestroyBlock(block);
//						}
//
//						block = CreateBlock();
//						SetBlockPosition(block, row, j);
//
//						if(FindLargeGroups().Count == numGroups){
//							//							Debug.Log("Found solution");
//							break;
//						}
//					}
//				}
//			}
//		}
//	}

	public void DestroyBlock(BlockData block){
		if(OnBoard(block)){
			blocks[block.row, block.col] = null;
		}
	}

	public void SetBlockPosition(BlockData block, int row, int col){
		if(OnBoard(block)){
			blocks[block.row, block.col] = null;

			MoveEvent e = new MoveEvent();
			e.row = block.row;
			e.col = block.col;
			e.row2 = row;
			e.col2 = col;
			PushEvent(e);
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

	public class Group {
		public int size = 0;
		public List<BlockData> blocks;

		public Group(BlockData firstBlock){
			blocks = new List<BlockData>();
			blocks.Add(firstBlock);
			firstBlock.group = this;
			size = 1;
		}

		public void Merge(Group group){
			List<BlockData> tempList = group.blocks;
			group.blocks = new List<BlockData>();
			group.size = 0;

			foreach(BlockData block in tempList){
				block.group = this;
				blocks.Add(block);
				size++;
			}
		}
	}

	List<Group> FindGroups(){
		List<Group> groups = new List<Group>();

		foreach (BlockData block in blocks){
			if(block != null){
				block.group = new Group(block);
				groups.Add(block.group);
			}
		}

		for(int i = 0; i < height; i++){
			for(int j = 0; j < width; j++){
				BlockData current = blocks[i, j];
				if(current != null){
					if(i > 0){
						BlockData lower = blocks[i - 1, j];
						if(lower != null && current.type == lower.type){
							lower.group.Merge(current.group);
						}
					}
					if(j > 0){
						BlockData left = blocks[i, j - 1];
						if(left != null && current.type == left.type){
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

	public int NumLargeGroups(){
		return FindLargeGroups().Count;
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
				// TODO this is leaving us with more states than we started with - need to pop the state or something
				PushState();

				// TODO move to its a Spawn method
				BlockData block = CreateBlock();
				SetBlockPosition(block, height/2 - 1, i);
				SpawnEvent e = new SpawnEvent();
				e.row = block.row;
				e.col = block.col;
				e.type = block.type;
				PushEvent(e);

				if(j == 0 || NumLargeGroups() == numGroups){
					break;
				}

				Undo();
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
