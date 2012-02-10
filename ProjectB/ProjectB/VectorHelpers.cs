using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace ProjectB
{
	public static class VectorHelpers
	{
		public static Rectangle ToRect (this Vector2 self, float width, float height)
		{
			return new Rectangle ((int)self.X, (int)self.Y, (int)width, (int)height); 
		}
	}
}
