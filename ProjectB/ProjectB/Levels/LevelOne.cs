using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ProjectB.Objects;

namespace ProjectB.Levels
{
	public class LevelOne
		: BaseLevel
	{
		public LevelOne ()
			: base ("LevelOne")
		{
			this.StartPoint = new Vector2(30, 700);
		}
	}
}
