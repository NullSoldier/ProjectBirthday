using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using ProjectB.Objects;
using ProjectB.States;

namespace ProjectB.Levels
{
	public class LevelThree
		: BaseLevel
	{
		public LevelThree ()
			: base ("LevelThree")
		{
			this.StartPoint = new Vector2(30, 700);
			this.SkyColor = new Color(56, 56, 194);
			this.UseClouds = true;
			this.DisplayInterface = true;

			this.Exit = new CaveExit
			{
				Location = new Vector2(730, 300)
			};
			
			AddGeometry (new Rectangle (-1, 751, 146, 46), CollisionType.Impassable);
			AddGeometry (new Rectangle(200, 746, 64, 52), CollisionType.Impassable);
			AddGeometry (new Rectangle(330, 746, 66, 53), CollisionType.Impassable);
			AddGeometry (new Rectangle(455, 746, 66, 54), CollisionType.Impassable);
			AddGeometry (new Rectangle(593, 744, 257, 54), CollisionType.Impassable);
			AddGeometry (new Rectangle(691, 428, 159, 52), CollisionType.Platform);
			AddGeometry (new Rectangle(456, 535, 128, 51), CollisionType.Impassable);
			AddGeometry (new Rectangle(231, 534, 152, 52), CollisionType.Platform);
			AddGeometry (new Rectangle(-2, 533, 167, 54), CollisionType.Platform);
			AddGeometry (new Rectangle(487, 349, 125, 54), CollisionType.Platform);
			AddGeometry (new Rectangle(0, 301, 386, 53), CollisionType.Platform);

			AddGeometry (new Rectangle(-54, 68, 55, 731), CollisionType.Impassable);
			AddGeometry (new Rectangle(849, 203, 50, 599), CollisionType.Impassable);

			AddLadder (new Rectangle(87, 526, 23, 224));
			AddLadder (new Rectangle(332, 297, 24, 235));

			AddChest (new Vector2(600, 735), FriendFactory.GetJason());
			AddChest (new Vector2(490, 525), FriendFactory.GetJason());
			AddChest (new Vector2(100, 290), FriendFactory.GetJason());

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

			var w1 = SpawnWalker (new Vector2(195, 301), Directions.Left, 140f, 135f);
			var w2 = SpawnWalker (new Vector2(304, 533), Directions.Right, 55f, 55f);
			var s1 = SpawnSentry (new Vector2(780, 744), Directions.Left, 1800, gameState, 1.8f);
			var s2 = SpawnSentry (new Vector2(30, 300), Directions.Right, 2000, gameState);

			s1.Invincible = true;
			w1.Speed = 150;
		}

		private Camera camera;
		private GameState gameState;
	}
}
