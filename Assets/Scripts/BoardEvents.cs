using System;
using UnityEngine;

namespace BoardEvents {

	public abstract class BoardEvent {
		public void ApplyToBoard(Board board){
			board.ApplyEventBase(this);
		}
	}

	public class SpawnEvent : BoardEvent {
		public Block.Type type;
		public int row, col;
	}

	public class MoveEvent : BoardEvent {
		public int row, col;   // from
		public int row2, col2; // to
	}

}
