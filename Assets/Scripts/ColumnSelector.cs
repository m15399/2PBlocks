using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColumnSelector : MonoBehaviour {

	public Block blockObject;

	void Start(){
		blockObject.SetType(Utils.RandomEnum<Block.Type>(1));
	}

	public void SetBlockType(Block.Type type){
		blockObject.SetType(type);
	}

	public Block.Type GetBlockType(){
		return blockObject.type;
	}
}
