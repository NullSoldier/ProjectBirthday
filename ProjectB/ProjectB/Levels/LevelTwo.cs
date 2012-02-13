using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using ProjectB.Objects;
using ProjectB.States;

namespace ProjectB.Levels
{
	public class LevelTwo
		: BaseLevel
	{
		public LevelTwo ()
			: base ("LevelTwo")
		{
			this.StartPoint = new Vector2(30, 700);
			this.SkyColor = new Color(255, 170, 85);
			this.UseClouds = true;
			this.DisplayInterface = true;

			this.Exit = new CaveExit
			{
				Location = new Vector2(780, 623)
			};
			
			AddGeometry (new Rectangle (-1, 751, 126, 46), CollisionType.Impassable);
			AddGeometry (new Rectangle(163, 699, 64, 54), CollisionType.Impassable);
			AddGeometry (new Rectangle(269, 646, 265, 56), CollisionType.Impassable);
			AddGeometry (new Rectangle(576, 751, 121, 47), CollisionType.Impassable);
			AddGeometry (new Rectangle(764, 750, 36, 50), CollisionType.Impassable);

			AddGeometry (new Rectangle(454, 443, 268, 56), CollisionType.Platform);
			AddGeometry (new Rectangle(231, 443, 149, 53), CollisionType.Impassable);
			AddGeometry (new Rectangle(9, 364, 65, 54), CollisionType.Platform);

			AddGeometry (new Rectangle(0, 176, 288, 54), CollisionType.Platform);

			AddGeometry (new Rectangle(-43, -10, 44, 810), CollisionType.Impassable);
			AddGeometry (new Rectangle(860, 261, 47, 543), CollisionType.Impassable);
			AddGeometry (new Rectangle(799, 751, 61, 50), CollisionType.Impassable);

			AddLadder (new Rectangle(634, 429, 23, 320));
			AddLadder (new Rectangle(241, 156, 25, 285));

			AddChest (new Vector2(380, 620), FriendFactory.GetJason());
			AddChest (new Vector2(544, 414), FriendFactory.GetJason());
			AddChest (new Vector2(100, 150), FriendFactory.GetJason());

			FriendCount = 3;
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

			var w1 = SpawnWalker (new Vector2(396, 647), Directions.Left, 100f, 115f);
			var w2 = SpawnWalker (new Vector2(589, 443), Directions.Right, 100f, 110f);
			var w3 = SpawnWalker (new Vector2(143, 175), Directions.Right, 100f, 100f);
			var s1 = SpawnSentry (new Vector2(41, 365), Directions.Right, 1600, gameState);

			w1.Speed = 150;
		}

		private Camera camera;
		private GameState gameState;
	}
}
