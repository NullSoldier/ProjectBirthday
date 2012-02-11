using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ProjectB.Objects;
using ProjectB.States;

namespace ProjectB
{
	public class BaseLevel
	{
		public BaseLevel (string levelTexture)
		{
			CollisionGeometry = new List<CollisionGeometry> ();

			Level = new GameObject
			{
				Texture = Engine.ContentManager.Load<Texture2D> (levelTexture)
			};
		}

		public GameObject Level;
		public Vector2 StartPoint;
		public Color SkyColor;
		public bool UseClouds;
		public List<CollisionGeometry> CollisionGeometry;

		public virtual void Update (GameTime gameTime)
		{
		}

		public virtual void Start (GameState gameState)
		{
		}

		protected void AddGeometry (Rectangle rect, CollisionType collisionType)
		{
			CollisionGeometry.Add (new CollisionGeometry(rect, collisionType));
		}
	}
}
