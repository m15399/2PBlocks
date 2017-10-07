using System;
using System.Collections.Generic;
using UnityEngine;

using BlockData = LogicalBoard.BlockData;

public class BoardGroups {

	public class Group {
		public int size = 0;
		public List<BlockData> blocks;
		BoardGroups groups;

		public Group(BoardGroups groupsIn, BlockData firstBlock){
			groups = groupsIn;
			blocks = new List<BlockData>();
			blocks.Add(firstBlock);
			groups.SetBlockGroup(firstBlock, this);
			size = 1;
		}

		public void Merge(Group group){
			List<BlockData> tempList = group.blocks;
			group.blocks = new List<BlockData>();
			group.size = 0;

			foreach(BlockData block in tempList){
				groups.SetBlockGroup(block, this);
				blocks.Add(block);
				size++;
			}
		}
	}

	int width, height;
	readonly BlockData[,] blocks;
	Group[,] groups;

	public BoardGroups (BlockData[,] blocksIn) {
		blocks = blocksIn;
		height = blocks.GetLength(0);
		width = blocks.GetLength(1);
		groups = new Group[height, width];
	}

	void SetBlockGroup(BlockData block, Group theGroup){
		groups[block.row, block.col] = theGroup;
	}

	Group GetBlockGroup(BlockData block){
		return groups[block.row, block.col];
	}

	public List<Group> FindGroups(){
		List<Group> groups = new List<Group>();
			
		foreach (BlockData block in blocks){
			if(block != null){
				Group newGroup = new Group(this, block);
				SetBlockGroup(block, newGroup);
				groups.Add(newGroup);
			}
		}

		for(int i = 0; i < height; i++){
			for(int j = 0; j < width; j++){
				BlockData current = blocks[i, j];
				if(current != null){
					if(i > 0){
						BlockData lower = blocks[i - 1, j];
						if(lower != null && current.type == lower.type){
							GetBlockGroup(lower).Merge(GetBlockGroup(current));
						}
					}
					if(j > 0){
						BlockData left = blocks[i, j - 1];
						if(left != null && current.type == left.type){
							GetBlockGroup(left).Merge(GetBlockGroup(current));
						}
					}
				}
			}
		}

		return groups;
	}

	public List<Group> FindLargeGroups(){
		List<Group> groups = FindGroups();
		List<Group> largeGroups = new List<Group>();

		foreach(Group group in groups){
			if(group.size >= 3){
				largeGroups.Add(group);
			}
		}

		return largeGroups;
	}
}

