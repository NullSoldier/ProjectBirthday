using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using ProjectB.Objects;

namespace ProjectB
{
	public class Player
		: GameObject
	{
		public Directions Direction;

		public void Draw (SpriteBatch spriteBatch)
		{

		}
	}

	public enum Directions
	{
		Left,
		Right
	}
}
