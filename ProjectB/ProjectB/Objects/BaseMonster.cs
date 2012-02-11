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

		public virtual Rectangle GetBounds ()
		{
			if (!boundsCalculated)
			{
				CalculateBounds ();
				boundsCalculated = true;
			}

			int left = (int)Math.Round(Location.X - Sprite.Origin.X) + localBounds.X;
            int top = (int)Math.Round(Location.Y - Sprite.Origin.Y) + localBounds.Y;

            return new Rectangle(left, top, localBounds.Width, localBounds.Height);
		}
		
		protected Animation idleAnimation;
		protected Vector2 HealthBarPosition;
		protected float HealthBarWidth;
		protected AnimationPlayer Sprite;
		protected Rectangle localBounds;
		private bool boundsCalculated;

		private void CalculateBounds ()
		{
			// Calculate bounds within texture size.            
            int width = (int)(idleAnimation.FrameWidth * 0.4);
            int left = (idleAnimation.FrameWidth - width) / 2;
            int height = (int)(idleAnimation.FrameWidth * 0.8);
            int top = idleAnimation.FrameHeight - height;
            localBounds = new Rectangle(left, top, width, height);
		}
	}
}
