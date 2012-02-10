using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ProjectB
{
	public class CloudManager
	{
		public CloudManager (int width, int height)
		{
			random = new Random();
			cloudTexture = ProjectB.ContentManager.Load<Texture2D> ("Cloud");
			clouds = new Cloud[cloudCount];
			cloudColor = new Color(1f, 1f, 1f, 0.7f);
			
			managerWidth = width;
			managerHeight = height;
			
			GenerateClouds ();
		}

		public void Update (GameTime gameTime)
		{
			foreach (Cloud cloud in clouds)
			{
				cloud.Position += new Vector2 (-cloud.Speed, 0);

				if (cloud.Position.X + (cloudTexture.Width * cloud.Scale) < 0)
					cloud.Position.X = managerWidth;
			}
		}

		public void Draw(SpriteBatch spriteBatch)
		{
			foreach (Cloud cloud in clouds)
				spriteBatch.Draw (cloudTexture, cloud.Position, null, cloudColor, 0f, Vector2.Zero, cloud.Scale, SpriteEffects.None, 1f);
		}

		private Cloud[] clouds;
		private Color cloudColor;
		private int cloudCount = 4;
		private Texture2D cloudTexture;
		private Random random;
		private int managerWidth;
		private int managerHeight;

		private void GenerateClouds()
		{
			for (int i = 0; i < cloudCount; i++)
			{
				clouds[i] = new Cloud
				{
					Scale = random.Next (2, 3),
					Speed = 0.09f
				};

				int tries = 0;
				int maxTries = 5;
				do
				{
					clouds[i].Position = random.GetRandomVector2 (managerWidth, managerHeight);
					tries++;

					if (tries > maxTries)
						break;
				}
				while (IsCollidingWithAnotherCloud (clouds[i]));
			}
		}

		private bool IsCollidingWithAnotherCloud (Cloud src)
		{
			Rectangle srcRect = GetCloudRectangle (src);

			for (int i=0; i < clouds.Length; i++)
			{
				Cloud dest = clouds[i];

				if (dest == null || dest == src)
					continue;

				if (GetCloudRectangle (dest).Intersects (srcRect))
					return true;
			}

			return false;
		}

		private Rectangle GetCloudRectangle (Cloud cloud)
		{
			return new Rectangle (
				(int)cloud.Position.X,
				(int)cloud.Position.Y,
				(int)(cloudTexture.Width * cloud.Scale),
				(int)(cloudTexture.Height * cloud.Scale));
		}
	}

	public class Cloud
	{
		public Vector2 Position;
		public float Scale;
		public float Speed;
	}
}
