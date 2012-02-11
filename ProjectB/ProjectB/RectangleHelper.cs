using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace ProjectB
{
	public static class RectangleHelpers
	{
		public static Rectangle FromVectors (Vector2 one, Vector2 two)
		{
			if (one.X == two.X || one.Y == two.Y)
				return Rectangle.Empty;

			Vector2 topLeft = one.X < two.X && one.Y < two.Y ? one : two;
			Vector2 bottomRight = one.X > two.X && one.Y > two.Y ? one : two;

			if (topLeft == two && bottomRight == two)
			{
				topLeft = new Vector2(one.X, two.Y);
				bottomRight = new Vector2 (two.X, one.Y);
			}

			return new Rectangle ((int)topLeft.X, (int)topLeft.Y, (int)(bottomRight.X - topLeft.X), (int)(bottomRight.Y - topLeft.Y));
		}

		public static Rectangle ToRectangle(this Vector2 self, int width, int height)
		{
			return new Rectangle((int)self.X, (int)self.Y, width, height);
		}

		public static Rectangle Shrink (this Rectangle self, int x, int y)
		{
			int newX = self.X + (x / 2);
			int newY = self.Y + (y / 2);

			return new Rectangle (newX, newY, x / 2, y / 2);
		}
	}
}
