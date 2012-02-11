using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using ProjectB.Scripts;
using ProjectB.States;

namespace ProjectB.Levels
{
	public class LevelIntro
		: BaseLevel
	{
		public LevelIntro ()
			: base ("LevelIntro")
		{
			this.StartPoint = new Vector2(10, 150);
			this.SkyColor = Color.Gray;
			this.UseClouds = false;

			AddGeometry (new Rectangle(0, 0, 479, 52), CollisionType.Impassable);
			AddGeometry (new Rectangle(-1, 184, 490, 70), CollisionType.Impassable);
			AddGeometry (new Rectangle(-57, -3, 56, 254), CollisionType.Impassable);
			AddGeometry (new Rectangle(477, 0, 21, 246), CollisionType.Impassable);
		}

		public override void Start (GameState gameState)
		{
			gameState.SpawnPlayer (StartPoint);

			player = gameState.player;
			effects = gameState.effectManager;
			camera = gameState.camera;
			{
				camera.UseBounds = false;
				camera.Scale =  1.8f;
				camera.CenterOnPoint (240, 122);
			}
			
			effects.Add ("Player", new CharacterMoveEffect(player, 100, Directions.Right));
		}

		public override void Update(GameTime gameTime)
		{
			
		}

		private Camera camera;
		private EffectManager effects;
		private Player player;
	}
}
