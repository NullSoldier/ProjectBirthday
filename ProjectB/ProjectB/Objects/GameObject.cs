﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ProjectB.Objects
{
	public class GameObject
	{
		public Texture2D Texture;
		public Vector2 Location;

		public virtual void Update(GameTime gameTime)
		{
		}

		public virtual void Draw(SpriteBatch spriteBatch)
		{
		}

		public virtual Rectangle GetBounds()
		{
			throw new NotImplementedException();
		}
	}
}
