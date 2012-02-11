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
	public class Character
		: GameObject
	{
		public bool IsAlive
		{
			get { return Health > 0; }
		}

		public float Alpha
		{
			set { drawColor = new Color (1f, 1f, 1f, value); }
		}

		public Directions Direction;
		public int Health;
		public bool IsOnGround;
		public bool AcceptPhysicalInput = true;
		public SpriteEffects flip = SpriteEffects.None;
		public AnimationPlayer Sprite;

		public override void Update (GameTime gameTime)
		{
			if (AcceptPhysicalInput)
				HandleInput ();

			ApplyPhysics (gameTime, Engine.Project.CurrentLevel);

			if (IsAlive && IsOnGround)
            {
                if (Math.Abs(velocity.X) - 0.02f > 0)
                {
                    Sprite.PlayAnimation(moveAnimation);
                }
                else
                {
                    Sprite.PlayAnimation(idleAnimation);
                }
            }
			else if (isClimbing)
			{
				 if (Math.Abs(velocity.Y) - 0.02f > 0)
                {
                    Sprite.PlayAnimation(climbAnimation);
                }
                else
                {
                    Sprite.PlayAnimation(climbIdleAnimation);
                }
			}

			movement = 0f;
			climbMovement = 0f;
			isJumping = false;
		
			lastGametime = gameTime;
		}

		public override void Draw (SpriteBatch spriteBatch)
		{
			if (lastGametime == null)
				return;

			if (Direction == Directions.Right)
				flip = SpriteEffects.FlipHorizontally;

			Sprite.Draw (lastGametime, spriteBatch, Location, flip, drawColor);

			flip = SpriteEffects.None;
		}

		public void Reset ()
		{
			Sprite.PlayAnimation(idleAnimation);
		}

		public override Rectangle GetBounds()
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
				case CharacterAction.ClimbUp:
					climbMovement = -1.0f;
					break;
				case CharacterAction.ClimbDown:
					climbMovement = 1.0f;
					break;
				case CharacterAction.Jump:
					isJumping = true;
					break;
			}

			ResolveDirection();
		}

		private float previousBottom;
		private Vector2 velocity;
		public bool isClimbing;
		private bool isJumping;
		private bool wasJumping;
		private float jumpTime;
		private float movement;
		private float climbMovement;
		private Color drawColor = Color.White;
		private GameTime lastGametime;

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

		private const float ClimbSpeed = 6000.0f;

		// Animations
		protected Animation idleAnimation;
		protected Animation moveAnimation;
		protected Animation jumpAnimation;
		protected Animation climbAnimation;
		protected Animation climbIdleAnimation;
		private Rectangle localBounds;
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

			// For climbing ladders
			if (this.isClimbing)
			{
				velocity.Y = 0f;

				if (climbMovement != 0)
					velocity.Y = ClimbSpeed * climbMovement * elapsed;
			}

			// Prevent the Character from running faster than his top speed.            
			velocity.X = MathHelper.Clamp (velocity.X, -MaxMoveSpeed, MaxMoveSpeed);

			// Apply velocity.
			Location += velocity * elapsed;
			Location = new Vector2 ((float)Math.Round (Location.X), (float)Math.Round (Location.Y));

			HandleClimbing (level);

			// If the Character is now colliding with the level, separate them.
			HandleCollisions (level);

			// If the collision stopped us from moving, reset the velocity to zero.
			if (Location.X == previousPosition.X)
				velocity.X = 0;

			if (Location.Y == previousPosition.Y)
				velocity.Y = 0;
		}

		private float DoJump(float velocityY, GameTime gameTime)
		{
			// If the Character wants to jump
			if (isJumping)
			{
				// Begin or continue a jump
				if ((!wasJumping && this.IsOnGround) || jumpTime > 0.0f)
				{
					jumpTime += (float)gameTime.ElapsedGameTime.TotalSeconds;
					Sprite.PlayAnimation(jumpAnimation);
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

		private void HandleClimbing (BaseLevel level)
		{
			Rectangle bounds = GetBounds();
			Ladder climbingOn = null;

			foreach (Ladder ladder in level.Ladders)
			{
				if (ladder.Region.Intersects (bounds))
				{
					climbingOn = ladder;
					break;
				}
			}

			if (isClimbing && climbingOn == null)
				isClimbing = false;
			else if (!isClimbing && climbingOn != null && ShouldAttachToLadder())
				isClimbing = true;

		}

		private bool ShouldAttachToLadder ()
		{
			return Engine.NewKeyboard.IsKeyDown (Keys.W)
				|| Engine.NewKeyboard.IsKeyDown (Keys.S);
		}

		private void HandleCollisions (BaseLevel level)
		{
			// Get the Character's bounding rectangle and find neighboring tiles.
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
						if (!isClimbing)
						{
							// If we crossed the top of a tile, we are on the ground.
							if (previousBottom <= tileBounds.Top)
								this.IsOnGround = true;
						}

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
				PerformAction (CharacterAction.MoveLeft);
			else if (Engine.NewKeyboard.IsKeyDown (Keys.D))
				PerformAction (CharacterAction.MoveRight);

			if (isClimbing)
			{
				if (Engine.NewKeyboard.IsKeyDown (Keys.W))
					PerformAction (CharacterAction.ClimbUp);
				else if (Engine.NewKeyboard.IsKeyDown (Keys.S))
					PerformAction (CharacterAction.ClimbDown);
			}

			// Check if the Character wants to jump.
			isJumping = Engine.NewKeyboard.IsKeyDown (Keys.Space)
				|| Engine.NewKeyboard.IsKeyDown (Keys.W);
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
		ClimbUp,
		ClimbDown,
		Jump
	}
}
