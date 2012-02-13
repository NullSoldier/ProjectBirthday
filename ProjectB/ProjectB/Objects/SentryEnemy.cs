using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ProjectB.States;

namespace ProjectB.Objects
{
	public class SentryEnemy
		: BaseMonster
	{
		public SentryEnemy (Vector2 location, Directions defaultDirection, float fireSpeed, GameState gameState)
		{
			this.Location = location;
			this.FireSpeed = fireSpeed;
			this.direction = defaultDirection;
			this.gameState = gameState;

			if (direction == Directions.Left)
				horizontalOffset = -64;

			if (direction == Directions.Right)
				flip = SpriteEffects.FlipHorizontally;

			idleAnimation = new Animation (Engine.ContentManager.Load<Texture2D> ("Monsters/SentryIdle"), 0.1f, true);
			idleInvincable = new Animation (Engine.ContentManager.Load<Texture2D> ("Monsters/SentryIdle2"), 0.1f, true);
			Sprite.PlayAnimation (idleAnimation);

			HealthBarWidth = idleAnimation.FrameWidth * 0.80f;
			HealthBarPosition = new Vector2((-idleAnimation.FrameWidth / 2) + 5, -idleAnimation.FrameHeight + 10);

			Health = 100;
			MaxHealth = 100;
			Damage = 40;
		}

		public override void Update(GameTime gameTime)
		{
			lastGameTime = gameTime;

			timePassed += (float)gameTime.ElapsedGameTime.TotalMilliseconds;

			if (timePassed >= FireSpeed)
			{
				gameState.SpawnProjectile (this.Location + new Vector2(horizontalOffset, -64), this.direction, Damage);
				timePassed = 0;
			}

		}

		private float timePassed;
		private GameState gameState;

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
		private float horizontalOffset;
		private Directions direction;
		private bool invincible = false;

		private Animation idleInvincable;
		public float FireSpeed = 5000;

		public bool Invincible
		{
			get { return invincible; }
			set
			{
				if (value)
					Sprite.PlayAnimation (idleInvincable);
				else
					Sprite.PlayAnimation (idleAnimation);

				invincible = value;
			}
		}
	}
}
