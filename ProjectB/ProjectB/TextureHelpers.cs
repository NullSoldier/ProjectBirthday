using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ProjectB
{
	public static class TextureHelpers
	{
		public static Vector2 Origin (this Texture2D self)
		{
			return new Vector2(self.Width / 2, self.Height / 2);
		}
	}
}
