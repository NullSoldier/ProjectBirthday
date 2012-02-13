using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ProjectB.Scripts;
using ProjectB.States;

namespace ProjectB.Objects
{
	public class Chest
		: Character
	{
		public Chest ()
		{
			Texture = Engine.ContentManager.Load<Texture2D> ("ChestClosed");
			AcceptPhysicalInput = false;
			IsActive = true;
		}

		public Friend Friend;
		public bool Opened;

		public override void Draw (SpriteBatch spriteBatch)
		{
			if (!Opened)
				spriteBatch.Draw (Texture, Location, drawColor);
		}

		public override Rectangle GetBounds()
		{
			return new Rectangle((int)Location.X, (int)Location.Y, Texture.Width, Texture.Height);
		}

		public void Open (GameState gameState)
		{
			Friend.Spawn (gameState);
			Friend.IsActive = true;
			Friend.Location = this.Location;

			gameState.CurrentLevel.GameObjects.Add (Friend);

			gameState.effectManager.Add ("Chest", new CharacterFadeEffect (this, 1f, 0f, 1f,
				() => this.IsActive = false));

			Opened = true;
		}
	}
}
