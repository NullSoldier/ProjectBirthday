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
			Level = new GameObject
			{
				Texture = ProjectB.ContentManager.Load<Texture2D> (levelTexture)
			};
		}

		public GameObject Level;
		public Vector2 StartPoint;
	}
}
