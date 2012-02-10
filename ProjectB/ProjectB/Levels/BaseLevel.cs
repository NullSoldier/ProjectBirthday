using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ProjectB.Objects;

namespace ProjectB
{
	public class BaseLevel
	{
		public BaseLevel (string levelTexture)
		{
			CollisionGeometry = new List<CollisionGeometry> ();

			Level = new GameObject
			{
				Texture = ProjectB.ContentManager.Load<Texture2D> (levelTexture)
			};
		}

		public GameObject Level;
		public Vector2 StartPoint;
		public List<CollisionGeometry> CollisionGeometry;

		protected void AddGeometry (Rectangle rect, CollisionType collisionType)
		{
			CollisionGeometry.Add (new CollisionGeometry(rect, collisionType));
		}
	}
}
