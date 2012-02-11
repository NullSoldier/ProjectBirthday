using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ProjectB.Objects;
using ProjectB.States;

namespace ProjectB.Levels
{
	public class LevelOne
		: BaseLevel
	{
		public LevelOne ()
			: base ("LevelOne")
		{
			this.StartPoint = new Vector2(30, 700);
			this.SkyColor = new Color(166, 211, 239);
			this.UseClouds = true;

			AddGeometry (new Rectangle (-1, 751, 146, 46), CollisionType.Impassable);
			AddGeometry (new Rectangle (207, 751, 376, 48), CollisionType.Impassable);
			AddGeometry (new Rectangle (647, 751, 152, 48), CollisionType.Impassable);
		}

		public override void Update (GameTime gametime)
		{
			camera.CenterOnPoint (player.Location);
		}

		public override void Start (GameState gameState)
		{
			gameState.SpawnPlayer (StartPoint);

			player = gameState.player;
			camera = gameState.camera;
			{
				camera.UseBounds = true;
			}
		}

		private Camera camera;
		private Player player;
	}
}
