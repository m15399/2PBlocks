using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockSelection : MonoBehaviour {

	public GameObject blockPrefab;

	public Block.Type selectedType;
	Block selectedBlock;

	public bool acceptingInput = true;

	const int numBlocks = 3;

	Block[] blocks;

	Block CreateBlock(Block.Type type){
		GameObject blockObject = GameObject.Instantiate(blockPrefab);
		Block block = blockObject.GetComponent<Block>();
		block.transform.parent = transform;

		block.SetType(type);

		return block;
	}
		
	void Start () {
		blocks = new Block[numBlocks];
		float left = numBlocks / -2.0f + .5f;

		for(int i = 0; i < numBlocks; i++){
			Block block = CreateBlock(Utils.RandomEnum<Block.Type>(1));
			block.SetPositionAndSitStill(new Vector3(left + i, 0, 0));
			blocks[i] = block;
		}
	}

	public void UpdateBlocks(){
		for(int i = 0; i < numBlocks; i++){
			blocks[i].Undim();
		}

		selectedBlock.SetType(Utils.RandomEnum<Block.Type>(1));
	}

	public void SelectBlock(int number){
		selectedBlock = blocks[number];
		selectedType = selectedBlock.type;

		for(int i = 0; i < numBlocks; i++){
			if(i != number){
				blocks[i].Dim();
			}
		}
	}
	
	void Update () {

	}
}
