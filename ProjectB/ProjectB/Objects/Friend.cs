using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ProjectB.Objects
{
	public class Friend
		: GameObject
	{
		public Friend ()
		{
			Texture = Engine.ContentManager.Load<Texture2D> ("Friend");
		}

		public List<string> Messages;
		public int CurrentMessage;
		public float MessageDelay;
		public Color Color;

		public override void Draw (SpriteBatch spriteBatch)
		{
			spriteBatch.Draw (Texture, Location, Color);
		}
	}
}
