using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ProjectB.Scripts;
using ProjectB.States;

namespace ProjectB.Objects
{
	public class Friend
		: Character
	{
		public Friend ()
		{
			Texture = Engine.ContentManager.Load<Texture2D> ("Friend");
			AcceptPhysicalInput = false;
		}

		public List<string> Messages;
		public int CurrentMessage;
		public float MessageDelay;
		public string ThanksMessage;
		public Color Color;

		public void Spawn (GameState gameState)
		{
			//gameState.effectManager.Add ("Friend", new CharacterMessageEffect (this, ThanksMessage));
			gameState.effectManager.Add ("Friend", new CharacterFadeEffect(this, 1f, 0f, 3f,
				() => gameState.CaptureFriend()));
		}

		public override Rectangle GetBounds()
		{
			return new Rectangle((int)Location.X, (int)Location.Y, Texture.Width, Texture.Height);
		}

		public override void Update(GameTime gameTime)
		{
			base.Update(gameTime);
		}

		public override void Draw (SpriteBatch spriteBatch)
		{
			spriteBatch.Draw (Texture, Location, new Color(Color.R, Color.G, Color.B, drawColor.A));
		}
	}
}
