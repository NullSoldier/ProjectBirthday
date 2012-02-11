using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ProjectB.Objects
{
	public class CaveExit
		: Character
	{
		public CaveExit ()
		{
			Texture = Engine.ContentManager.Load<Texture2D> ("Exit");
			AcceptPhysicalInput = false;
		}

		public override void Draw(SpriteBatch spriteBatch)
		{
			spriteBatch.Draw (Texture, Location, drawColor);
		}

		public override Rectangle GetBounds()
		{
			return new Rectangle((int)Location.X, (int)Location.Y, Texture.Width, Texture.Height);
		}
	}
}
