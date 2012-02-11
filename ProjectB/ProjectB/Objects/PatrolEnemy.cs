using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ProjectB.Objects
{
	public class PatrolEnemy
		: BaseMonster
	{
		public PatrolEnemy (Vector2 location, Directions defaultDirection, float maxLeft, float maxRight)
		{
			this.Location = location;
			this.homeX = location.X;
			this.maxLeft = maxLeft;
			this.maxRight = maxRight;

			if (defaultDirection == Directions.Left)
				Speed *= -1;

			moveAnimation = new Animation (Engine.ContentManager.Load<Texture2D> ("Monsters/WalkerMove"), 0.1f, true);
			idleAnimation = new Animation (Engine.ContentManager.Load<Texture2D> ("Monsters/WalkerIdle"), 0.1f, true);
			Sprite.PlayAnimation (moveAnimation);

			HealthBarWidth = moveAnimation.FrameWidth * 0.80f;
			HealthBarPosition = new Vector2((-moveAnimation.FrameWidth / 2) + 5, -moveAnimation.FrameHeight + 10);

			Health = 100;
			MaxHealth = 100;
			Damage = 25;
		}

		public override void Update(GameTime gameTime)
		{
			float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;

			Location += new Vector2(Speed * elapsed, 0);

			if (Speed < 0 && homeX - Location.X > maxLeft)
				Speed *= -1;
			else if (Speed > 0 && Location.X - homeX > maxRight)
				Speed *= -1;

			flip = Speed < 0 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

			lastGameTime = gameTime;
		}

		public override void Draw(SpriteBatch spriteBatch)
		{
			if (lastGameTime == null)
				return;

			Sprite.Draw (lastGameTime, spriteBatch, Location, flip, Color.White);

			base.Draw (spriteBatch);
		}

		public override Rectangle GetBounds()
		{
			return base.GetBounds();
		}

		private GameTime lastGameTime;
		private SpriteEffects flip;
		private Directions direction;
		private float maxLeft;
		private float maxRight;
		private float homeX = 0;

		private Animation moveAnimation;
		public float Speed = 100;
	}
}
