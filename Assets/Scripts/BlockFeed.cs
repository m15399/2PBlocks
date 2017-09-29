using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockFeed : MonoBehaviour {

	public static BlockFeed instance {
		get; set;
	}

	const int numBlocks = 5;

	public GameObject blockPrefab;

	Block[] blocks;

	BlockFeed(){
		instance = this;
	}

	void Start () {
		transform.position = new Vector3(Board.instance.width/2 + 1.5f, transform.position.y, 0);

		blocks = new Block[numBlocks];

		for(int i = 0; i < numBlocks; i++){
			blocks[i] = CreateBlock(Utils.RandomEnum<Block.Type>(1));
			// TODO loc
		}
	}

	public Block.Type PopNextBlock(){
		Block popped = blocks[0];
		for(int i = 0; i < numBlocks - 1; i++){
			blocks[i] = blocks[i+1];
		}
		blocks[numBlocks - 1] = CreateBlock(Utils.RandomEnum<Block.Type>(1));

		GameObject.Destroy(popped.gameObject);
		return popped.type;
	}

	public Block.Type NextBlockType(){
		return blocks[0].type;
	}

	Block CreateBlock(Block.Type type){
		GameObject blockObject = GameObject.Instantiate(blockPrefab);
		Block block = blockObject.GetComponent<Block>();
		block.transform.parent = transform;
		block.transform.localScale = new Vector3(1, 1, 1);

		block.SetLocation(numBlocks, 0, Block.MoveType.Instant);
		block.SetType(type);
		block.Dim();

		return block;
	}

	void Update () {
		for(int i = 0; i < numBlocks; i++){
			blocks[i].SetLocation(i, 0, Block.MoveType.Fast);
		}
		for(int i = 0; i < numBlocks; i++){
			blocks[i].Dim();
		}
		// blocks[0].Undim();
	}
}
