using System;
using UnityEngine;

namespace BoardEvents {

	public abstract class BoardEvent {
		public void ApplyToBoard(Board board){
			board.ApplyEventBase(this);
		}
	}

	//
	// Events
	//

	public class SpawnEvent : BoardEvent {
		public Block.Type type;
		public int row, col;

		public SpawnEvent(Block.Type typeIn, int rowIn, int colIn){
			type = typeIn; row = rowIn; col = colIn;
		}
	}

	public class MoveEvent : BoardEvent {
		public int row, col;   // from
		public int row2, col2; // to

		public MoveEvent(int rowIn, int colIn, int row2In, int col2In){
			row = rowIn; col = colIn;
			row2 = row2In; col2 = col2In;
		}
	}

}
