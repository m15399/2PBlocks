using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block : MonoBehaviour {

	public const byte DimAlpha = 180;
	public const byte FullAlpha = 255;

	public enum Type {
		None,
		Red,
		Green,
		Blue
	}

	public enum MoveType {
		None,    // No opinion (keep previous or default)
		Default,
		Fast,
		Instant
	}

	public GameObject spriteObject;
	SpriteRenderer mainSprite;

	public Type type;

	public bool onBoard = false;
	public int row = -1, col = -1;
	Vector3 targetPos;
	public bool moving = true;
	MoveType moveType = MoveType.None;

	byte alpha = 255;
	float alphaCurr = 255;
	float alphaRate = 255 * 4;
	byte targetAlpha = 255;


	void Start () {
		mainSprite = spriteObject.GetComponent<SpriteRenderer>();

		UpdateVisual();
	}

	public void SetType(Type newType){
		type = newType;
		UpdateVisual();
	}

	public void SetLocation(int row, int col, MoveType desiredMoveType = MoveType.None){
		this.row = row;
		this.col = col;
		targetPos = new Vector3(col, row, 0);

		// None means keep previous
		//
		if(desiredMoveType != MoveType.None){
			moveType = desiredMoveType;
		}

		if(desiredMoveType == MoveType.Instant){
			transform.localPosition = targetPos;
			moveType = MoveType.Default;
		}
	}

	public void SetAlpha(byte a){
		alphaCurr = a;
		targetAlpha = a;

		if(alpha != a){
			alpha = a;
			UpdateVisual();
		}
	}

	public void SetTargetAlpha(byte target, float rate = 0){
		if(rate != 0){
			alphaRate = rate;
		}
		targetAlpha = target;
	}

	void UpdateVisual(){
		Color32 color = Color.white;

		switch(type){
		case Type.Red:
			color = new Color32(218, 64, 64, alpha);
			break;
		case Type.Green:
			color = new Color32(36, 214, 36, alpha);
			break;
		case Type.Blue:
			color = new Color32(124, 124, 255, alpha);
			break;
		}

		if(mainSprite){
			mainSprite.color = color;
		}
	}
	
	void Update () {
		if((transform.localPosition - targetPos).magnitude > .1){
			moving = true;
		} else {
			moving = false;
		}

		float speed = 6;

		switch(moveType){
		case MoveType.Fast:
			speed = 60;
			break;
		case MoveType.Instant:
			speed = 0; // Should already be there
			break;
		}

		transform.localPosition = Vector3.MoveTowards(transform.localPosition, targetPos, speed * Time.deltaTime);

		// If resting, switch to default move type
		//
		if(moveType != MoveType.Default && (transform.localPosition - targetPos).magnitude < .001){
			moveType = MoveType.Default;
		}

		// Alpha
		// 
		alphaCurr = Mathf.MoveTowards(alphaCurr, (float) targetAlpha, alphaRate * Time.deltaTime);
		byte a = (byte) alphaCurr;
		if(a != alpha){
			alpha = a;
			UpdateVisual();
		}
	}
}
