using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace ProjectB
{
	public class CollisionGeometry
	{
		public CollisionGeometry(Rectangle rect, CollisionType collisionType)
		{
			this.Rectangle = rect;
			this.CollisionType = collisionType;
		}

		public Rectangle Rectangle;
		public CollisionType CollisionType;
	}

	public enum CollisionType
	{
		Platform,
		Impassable
	}
}
