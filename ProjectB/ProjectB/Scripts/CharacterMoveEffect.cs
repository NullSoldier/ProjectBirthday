using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using ProjectB.States;

namespace ProjectB.Scripts
{
	public class CharacterMoveEffect
		: BaseEffect
	{
		public CharacterMoveEffect (Character character, float distance, Directions direction, Action action = null)
			: base (action)
		{
			this.distance = distance;
			this.direction = direction;
			this.character = character;
		}

		public override void Update (GameTime gameTime)
		{
			 this.character.PerformAction (charAction);

			if (Math.Abs(this.character.Location.X - baseAmount) > distance)
			{
				Finish();
				this.character.AcceptPhysicalInput = false;
			}	
		}

		public override void Start (GameState gameState)
		{
			this.character.AcceptPhysicalInput = false;
			baseAmount = this.character.Location.X;
			charAction = direction == Directions.Left ? CharacterAction.MoveLeft : CharacterAction.MoveRight;
		}

		private Character character;
		private float baseAmount;
		private float distance;
		private CharacterAction charAction;
		private Directions direction;
	}
}
