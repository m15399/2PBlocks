using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpcomingBlocks : MonoBehaviour {

	public static UpcomingBlocks instance {
		get; set;
	}

	const int numBlocks = 5;

	public GameObject blockPrefab;

	Block[] blocks;

	UpcomingBlocks(){
		instance = this;
	}

	void Start () {
		transform.position = new Vector3(Board.instance.width/2 + 1.5f, -Board.instance.height/2);

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

	public void CheatNextBlockType(Block.Type type){
		blocks[0].SetType(type);
	}

	Block CreateBlock(Block.Type type){
		GameObject blockObject = GameObject.Instantiate(blockPrefab);
		Block block = blockObject.GetComponent<Block>();
		block.transform.parent = transform;

		block.SetLocation(numBlocks, 0, Block.MoveType.Instant);
		block.SetType(type);
		block.Dim();

		return block;
	}

	void Update () {
		for(int i = 0; i < numBlocks; i++){
			blocks[i].SetLocation(i, 0, Block.MoveType.Fast);
		}
		for(int i = 1; i < numBlocks; i++){
			blocks[i].Dim();
		}
		blocks[0].Undim();
	}
}
