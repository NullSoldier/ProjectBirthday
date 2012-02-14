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
	class Projectile
		: GameObject
	{
		public Projectile (Directions directions, Vector2 location)
		{
			Texture = Engine.ContentManager.Load<Texture2D> ("Projectile");
			IsActive = true;
			this.Location = location;
			if (directions == Directions.Left)
				speed *= -1;

			if (directions == Directions.Right)
				flip = SpriteEffects.FlipHorizontally;


		}

		public float speed = 5;
		public float Damage;
		public float Life = -1;

		public SpriteEffects flip = SpriteEffects.None;

		public override void Update(GameTime gameTime)
		{
			if (Life != -1)
				Life = RandomHelper.LowClamp (Life - (float)gameTime.ElapsedGameTime.TotalSeconds, 0);

			Location += new Vector2(speed,0);	
		}

		public override void Draw (SpriteBatch spriteBatch)
		{
			spriteBatch.Draw (Texture, Location, null, Color.White, 0f, Vector2.Zero, 1f, flip, 1f);
		}

		public override Rectangle GetBounds()
		{
			return new Rectangle((int)Location.X, (int)Location.Y, Texture.Width, Texture.Height);
		}

	}
}
