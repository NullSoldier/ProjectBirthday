using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ProjectB.Objects
{
	public class NyanCat
		: GameObject
	{
		public NyanCat (Directions direction)
		{
			// Create a blank texture to use in rendering the rainbow
			blankTexture = new Texture2D (Engine.Graphics.GraphicsDevice, 1, 1);
			blankTexture.SetData (new [] { Color.White });

			moveDirection = direction;
			chunks = new Queue<RainbowChunk>();

			if (direction == Directions.Left)
				catSpeed *= -1;

			moveAnimation = new Animation  (Engine.ContentManager.Load<Texture2D> ("NyanCat"), 0.1f, true);
			sprite = new AnimationPlayer();
			sprite.PlayAnimation (moveAnimation);
		}

		public override void Update (GameTime gameTime)
		{
			// Move the Nyan cat
			Location += new Vector2(catSpeed * (float)gameTime.ElapsedGameTime.TotalMilliseconds, 0);

			trailPassed += (float)gameTime.ElapsedGameTime.TotalSeconds;
			trailDownPassed += (float)gameTime.ElapsedGameTime.TotalSeconds;

			// Should we spawn a chunk?
			if (trailPassed >= trailDelay)
			{
				SpawnTrailChunk (gameTime);
				trailPassed = 0;
			}

			// Should we spawn the trail lower or higher?
			if (trailDownPassed >= trailDownDelay)
			{
				this.trailDown = !this.trailDown;
				trailOffset = new Vector2 (0, this.trailDown ? trailDownAmount : 0f);
				trailDownPassed = 0;
			}

			// Decay each chunk
			foreach (RainbowChunk chunk in chunks)
				chunk.Life -= (float)gameTime.ElapsedGameTime.TotalSeconds;

			// Look to kill off the last chunk
			if (chunks.Peek().Life <= 0)
				chunks.Dequeue ();

			lastGametime = gameTime;
		}

		public void Draw(SpriteBatch spriteBatch)
		{
			if (lastGametime == null)
				return;

			if (!initialized)
				Initialize();

			foreach (RainbowChunk chunk in chunks)
				chunk.Draw (spriteBatch);

			sprite.Draw (lastGametime, spriteBatch, Location, effects, Color.White, catScale);
			//spriteBatch.Draw (Texture, Location + new Vector2 (0, 23 * catScale), null, Color.White, 0f, origin, catScale, effects, 1f);
		}

		private AnimationPlayer sprite;
		private Animation moveAnimation;
		private GameTime lastGametime;
		private Texture2D blankTexture;
		private float catSpeed = 0.5f;
		private float trailLife = 0.25f;
		private float trailDelay;
		private float trailPassed;
		private float catScale = 0.25f;
		private float chunkWidth = 10f;
		private float chunkHeight;
		private Directions moveDirection;
		private Queue<RainbowChunk> chunks;
		private bool initialized;
		private Vector2 origin;
		private SpriteEffects effects;

		private bool trailDown;
		private float trailDownDelay = 0.05f;
		private float trailDownPassed;
		private float trailDownAmount = 3f;
		private Vector2 trailOffset;

		private void Initialize()
		{
			// Initialize and cache some needed values for later
			origin = new Vector2(Texture.Width / 2, Texture.Height / 2);
			chunkHeight = Texture.Height * catScale;

			if (moveDirection == Directions.Left)
				effects = SpriteEffects.FlipHorizontally;
		}

		private void SpawnTrailChunk (GameTime gameTime)
		{
			RainbowChunk chunk = new RainbowChunk
			{
				Life = this.trailLife,
				Width = this.chunkWidth,
				Height = this.chunkHeight,
				Location = this.Location + trailOffset + new Vector2(0, -42),
				Texture = this.blankTexture
			};
			chunk.Initialize();
			chunks.Enqueue (chunk);
		}
	}
	
	public class RainbowChunk
			: GameObject
	{
		public float Life;
		public float Width;
		public float Height;

		public void Initialize()
		{
			scaler = new Vector2 (Width, Height / 10);
			firstLocation = new Vector2(Location.X, Location.Y - (Height / 10));
		}

		public override void Draw(SpriteBatch spriteBatch)
		{
			DrawChunkColor (spriteBatch, Color.Red, 1);
			DrawChunkColor (spriteBatch, Color.Orange, 2);
			DrawChunkColor (spriteBatch, Color.Yellow, 3);
			DrawChunkColor (spriteBatch, Color.Green, 4);
			DrawChunkColor (spriteBatch, Color.Blue, 5);
			DrawChunkColor (spriteBatch, Color.Purple, 6);
		}

		private Vector2 scaler;
		private Vector2 firstLocation;

		private void DrawChunkColor (SpriteBatch spriteBatch, Color color, int order)
		{
			spriteBatch.Draw (Texture, firstLocation + (scaler * new Vector2 (1, order)), null, color, 0f, Vector2.Zero, scaler, SpriteEffects.None, 1f);
		}
	}
}
