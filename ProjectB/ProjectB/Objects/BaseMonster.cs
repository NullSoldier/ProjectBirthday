using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ProjectB.Objects
{
	public class BaseMonster
		: GameObject
	{
		public int Health;
		public int MaxHealth;
		public override void Draw(SpriteBatch spriteBatch)
		{
			float percentage = (float)Health / (float)MaxHealth;

			if (percentage == 1.0f)
				return;

			Vector2 loc = Location + HealthBarPosition;

			Rectangle destRectangle = new Rectangle ((int)loc.X, (int)loc.Y, (int)HealthBarWidth, 10);
			Rectangle healthRectangle = new Rectangle (destRectangle.X, destRectangle.Y,
				(int)(destRectangle.Width  * percentage), destRectangle.Height);

			spriteBatch.Draw (Engine.BlankTexture, destRectangle, null, Color.Gray);
			spriteBatch.Draw (Engine.BlankTexture, healthRectangle, null, Color.Green);
		}

		
		protected Vector2 HealthBarPosition;
		protected float HealthBarWidth;
	}
}
