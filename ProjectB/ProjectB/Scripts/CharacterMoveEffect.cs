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
		public CharacterMoveEffect (Player player, float distance, Directions direction, Action action)
			: base (action)
		{
			this.player = player;
		}

		public override void Update (GameTime gameTime)
		{
			 player.PerformAction (charAction);

			if (Math.Abs(player.Location.X - baseAmount) > distance)
			{
				Finish();
				player.AcceptPhysicalInput = false;
			}	
		}

		public override void Start (GameState gameState)
		{
			player.AcceptPhysicalInput = false;
			baseAmount = player.Location.X;
			charAction = direction == Directions.Left ? CharacterAction.MoveLeft : CharacterAction.MoveRight;
		}

		private Player player;
		private float baseAmount;
		private float distance;
		private CharacterAction charAction;
		private Directions direction;
	}
}
