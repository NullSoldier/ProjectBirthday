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

			AddGeometry (new Rectangle (-1, 751, 146, 46), CollisionType.Impassable);
			AddGeometry (new Rectangle (207, 751, 376, 48), CollisionType.Impassable);
			AddGeometry (new Rectangle (647, 751, 152, 48), CollisionType.Impassable);
		}
	}
}
