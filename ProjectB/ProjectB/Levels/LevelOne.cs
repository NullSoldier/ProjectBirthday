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

			AddGeometry (new Rectangle(397, 538, 402, 55), CollisionType.Platform);
			AddGeometry (new Rectangle(-1, 538, 334, 54), CollisionType.Impassable);

			AddGeometry (new Rectangle(465, 359, 264, 52), CollisionType.Impassable);
			AddGeometry (new Rectangle(0, 184, 663, 54), CollisionType.Platform);

			AddGeometry (new Rectangle(-56, -78, 56, 878), CollisionType.Impassable);
			AddGeometry (new Rectangle(801, 59, 52, 746), CollisionType.Impassable);

			AddLadder (new Rectangle(452, 518, 25, 231));
			AddLadder (new Rectangle(124, 164, 22, 373));
			AddLadder (new Rectangle(480, 173, 25, 183));

			AddChest (new Vector2(657, 476), FriendFactory.GetJason());
		}

		public override void Update (GameTime gametime)
		{
			camera.CenterOnPoint (Player.Location);
		}

		public override void Start (GameState gameState)
		{
			this.gameState = gameState;

			SpawnPlayer (StartPoint);

			camera = gameState.camera;
			{
				camera.Bounds = new Rectangle(0, 0, Level.Texture.Width, Level.Texture.Height);
				camera.UseBounds = true;
				camera.Scale =  1f;
				camera.CenterOnPoint (StartPoint);
			}

			gameState.cloudManager = new CloudManager (camera.Bounds.Width, camera.Bounds.Height);

			GameObjects.Add (Player);

			SpawnWalker (new Vector2(426, 750), Directions.Left, 200f, 150f);
			var w2 = SpawnWalker (new Vector2(150, 539), Directions.Left, 125f, 150f);
			w2.Speed = 150;
			SpawnWalker (new Vector2(605, 539), Directions.Right, 200f, 160f);
			SpawnWalker (new Vector2(593, 357), Directions.Right, 100f, 100f);
		}

		private Camera camera;
		private GameState gameState;
	}
}
