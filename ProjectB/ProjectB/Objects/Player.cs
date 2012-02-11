using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using ProjectB.Objects;

namespace ProjectB
{
	public class Player
		: GameObject
	{
		public bool IsAlive
		{
			get { return Health > 0; }
		}

		public Directions Direction;
		public int Health;
		public bool IsOnGround;
		public bool AcceptPhysicalInput = true;

		public override void Update(GameTime gameTime)
		{
			if (AcceptPhysicalInput)
				HandleInput ();

			ApplyPhysics (gameTime, Engine.Project.CurrentLevel);

			movement = 0f;
			isJumping = false;
		}

		public override void Draw(SpriteBatch spriteBatch)
		{
			spriteBatch.Draw (Texture, Location, Color.Red);
		}

		public override Rectangle GetBounds()
		{
			return new Rectangle ((int)Location.X, (int)Location.Y, Texture.Width, Texture.Height);
		}

		public void PerformAction (CharacterAction action)
		{
			switch (action)
			{
				case CharacterAction.MoveLeft:
					movement = -1.0f;
					break;
				case CharacterAction.MoveRight:
					movement = 1.0f;
					break;
				case CharacterAction.Jump:
					isJumping = true;
					break;
			}

			ResolveDirection();
		}

		private float previousBottom;
		private Vector2 velocity;
		private bool isJumping;
		private bool wasJumping;
		private float jumpTime;
		private float movement;

		// Constants for controling horizontal movement
		private const float MoveAcceleration = 13000.0f;
		private const float MaxMoveSpeed = 1750.0f;
		private const float GroundDragFactor = 0.48f;
		private const float AirDragFactor = 0.58f;

		// Constants for controlling vertical movement
		private const float MaxJumpTime = 0.25f;
		private const float JumpLaunchVelocity = -3500.0f;
		private const float GravityAcceleration = 3400.0f;
		private const float MaxFallSpeed = 550.0f;
		private const float JumpControlPower = 0.14f;

		private void ApplyPhysics (GameTime gameTime, BaseLevel level)
		{
			float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;

			Vector2 previousPosition = Location;

			// Base velocity is a combination of horizontal movement control and
			// acceleration downward due to gravity.
			velocity.X += movement * MoveAcceleration * elapsed;
			velocity.Y = MathHelper.Clamp (velocity.Y + GravityAcceleration * elapsed, -MaxFallSpeed, MaxFallSpeed);

			velocity.Y = DoJump (velocity.Y, gameTime);

			// Apply pseudo-drag horizontally.
			if (this.IsOnGround)
				velocity.X *= GroundDragFactor;
			else
				velocity.X *= AirDragFactor;

			// Prevent the player from running faster than his top speed.            
			velocity.X = MathHelper.Clamp (velocity.X, -MaxMoveSpeed, MaxMoveSpeed);

			// Apply velocity.
			Location += velocity * elapsed;
			Location = new Vector2 ((float)Math.Round (Location.X), (float)Math.Round (Location.Y));

			// If the player is now colliding with the level, separate them.
			HandleCollisions (level);

			// If the collision stopped us from moving, reset the velocity to zero.
			if (Location.X == previousPosition.X)
				velocity.X = 0;

			if (Location.Y == previousPosition.Y)
				velocity.Y = 0;
		}

		private float DoJump(float velocityY, GameTime gameTime)
		{
			// If the player wants to jump
			if (isJumping)
			{
				// Begin or continue a jump
				if ((!wasJumping && this.IsOnGround) || jumpTime > 0.0f)
				{
					jumpTime += (float)gameTime.ElapsedGameTime.TotalSeconds;
				}

				// If we are in the ascent of the jump
				if (0.0f < jumpTime && jumpTime <= MaxJumpTime)
				{
					// Fully override the vertical velocity with a power curve that gives players more control over the top of the jump
					velocityY = JumpLaunchVelocity * (1.0f - (float)Math.Pow (jumpTime / MaxJumpTime, JumpControlPower));
				}
				else
				{
					// Reached the apex of the jump
					jumpTime = 0.0f;
				}
			}
			else
			{
				// Continues not jumping or cancels a jump in progress
				jumpTime = 0.0f;
			}
			wasJumping = isJumping;

			return velocityY;
		}

		private void HandleCollisions (BaseLevel level)
		{
			// Get the player's bounding rectangle and find neighboring tiles.
			Rectangle bounds = GetBounds();

			// Reset flag to search for ground collision.
			this.IsOnGround = false;

			foreach (var geometry in level.CollisionGeometry)
			{
				// Determine collision depth (with direction) and magnitude.
				Rectangle tileBounds = geometry.Rectangle;
				CollisionType collision = geometry.CollisionType;
				Vector2 depth = RectangleExtensions.GetIntersectionDepth (bounds, tileBounds);

				if (depth != Vector2.Zero)
				{
					float absDepthX = Math.Abs (depth.X);
					float absDepthY = Math.Abs (depth.Y);

					// Resolve the collision along the shallow axis.
					if (absDepthY < absDepthX || collision == CollisionType.Platform) // Or if floating platform
					{
						// If we crossed the top of a tile, we are on the ground.
						if (previousBottom <= tileBounds.Top)
							this.IsOnGround = true;

						// Ignore platforms, unless we are on the ground.
						if (collision == CollisionType.Impassable || this.IsOnGround)
						{
							// Resolve the collision along the Y axis.
							Location = new Vector2 (Location.X, Location.Y + depth.Y);

							// Perform further collisions with the new bounds.
							bounds = GetBounds();
						}
					}
					else if (collision == CollisionType.Impassable) // Ignore platforms.
					{
						// Resolve the collision along the X axis.
						Location = new Vector2 (Location.X + depth.X, Location.Y);

						// Perform further collisions with the new bounds.
						bounds = GetBounds ();
					}
				}
			}

			// Save the new bounds bottom.
			previousBottom = bounds.Bottom;
		}

		private void HandleInput()
		{
			if (Engine.NewKeyboard.IsKeyDown (Keys.A))
				movement = -1.0f;
			else if (Engine.NewKeyboard.IsKeyDown (Keys.D))
				movement = 1.0f;

			// Check if the player wants to jump.
			isJumping = Engine.NewKeyboard.IsKeyDown (Keys.Space)
				|| Engine.NewKeyboard.IsKeyDown (Keys.W);

			ResolveDirection();
		}

		private void ResolveDirection()
		{
			if (movement > 0)
				Direction = Directions.Right;
			else if (movement < 0)
				Direction = Directions.Left;
		}
	}

	public enum CharacterAction
	{
		MoveLeft,
		MoveRight,
		Jump
	}
}
