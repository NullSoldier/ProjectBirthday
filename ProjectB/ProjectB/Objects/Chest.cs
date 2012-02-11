using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ProjectB.States;

namespace ProjectB.Objects
{
	public class Chest
		: GameObject
	{
		public Chest ()
		{
			Texture = Engine.ContentManager.Load<Texture2D> ("ChestClosed");
		}

		public Friend Friend;
		public bool Opened;

		public override void Draw (SpriteBatch spriteBatch)
		{
			if (!Opened)
				spriteBatch.Draw (Texture, Location, Color.White);
		}

		public override Rectangle GetBounds()
		{
			return new Rectangle((int)Location.X, (int)Location.Y, Texture.Width, Texture.Height);
		}

		public void Open (GameState gameState)
		{
			Friend.IsActive = true;
			Friend.Location = this.Location;

			gameState.CurrentLevel.GameObjects.Add (Friend);
		}
	}
}
