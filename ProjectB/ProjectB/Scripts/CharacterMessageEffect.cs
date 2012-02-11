using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using ProjectB.States;

namespace ProjectB.Scripts
{
	public class CharacterMessageEffect
		: BaseEffect
	{
		public CharacterMessageEffect (Character character, string message, Action action = null)
			: base (action)
		{
			this.character = character;
			this.gameState = gameState;
			this.message = message;
		}

		public override void Update (GameTime gameTime)
		{
			
		}

		public override void Start (GameState gameState)
		{
			
		}

		private Character character;
		private GameState gameState;
		private string message;
	}
}
