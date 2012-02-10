using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace ProjectB
{
	public static class RandomHelper
	{
		public static Vector2 GetRandomVector2 (this Random self, int maxX, int maxY)
		{
			return new Vector2
			(
				self.Next (0, maxX),
				self.Next (0, maxY)
			);
		}
	}
}
