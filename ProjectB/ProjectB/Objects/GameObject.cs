using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ProjectB.Objects
{
	public class GameObject
	{
		public Texture2D Texture;
		public Vector2 Location;
		public bool IsActive;
		public bool MarkedForDeletion;
		public float Scale = 1f;

		public virtual void Update(GameTime gameTime)
		{
		}

		public virtual void Draw(SpriteBatch spriteBatch)
		{
			spriteBatch.Draw (Texture, Location, Color.White);
		}

		public virtual Rectangle GetBounds()
		{
			throw new NotImplementedException();
		}
	}
}
