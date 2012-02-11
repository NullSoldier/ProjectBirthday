using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ProjectB.Objects;
using ProjectB.Scripts;
using ProjectB.States;

namespace ProjectB.Levels
{
	public class LevelOutro
		: BaseLevel
	{
		public LevelOutro ()
			: base ("LevelIntro")
		{
			this.StartPoint = new Vector2(10, 200);
			this.SkyColor = Color.Transparent;
			this.UseClouds = false;
			this.FriendCount = 1;

			AddGeometry (new Rectangle (0, 0, 479, 52), CollisionType.Impassable);
			AddGeometry (new Rectangle (-1, 211, 482, 31), CollisionType.Impassable);
			AddGeometry (new Rectangle (-57, -3, 56, 254), CollisionType.Impassable);
			AddGeometry (new Rectangle (477, 0, 21, 246), CollisionType.Impassable);
		}

		private GameObject crowd;

		public override void Start (GameState gameState)
		{
			SpawnPlayer (StartPoint);

			effects = gameState.effectManager;
			camera = gameState.camera;
			{
				camera.UseBounds = false;
				camera.Scale = 1.8f;
				camera.CenterOnPoint (240, 122);
			}

			SpawnGiftBox ("Boxes", new Vector2(260, 145));

			crowd = new GameObject
			{
				Texture = Engine.ContentManager.Load<Texture2D> ("Crowd"),
				Location = new Vector2 (130, 130)
			};

			GameObjects.Add (crowd);
			GameObjects.Add (Player);

			effects.Add ("Character", new CharacterMoveEffect(Player, 50, Directions.Right));
			effects.Add ("Character", new WaitEffect (2f, () =>
			{
				Engine.DialogRunner.EnqueueMessageBox ("Megan", "Yay! Everyone is here!");
				Engine.DialogRunner.EnqueueMessageBox ("Megan", "Now we can party!", () =>
				{
					effects.Add ("Character", new WaitEffect (2f, () =>
					{
						Engine.Project.Fade (255, 0, 5f, () => Engine.displayTeam = true);
					}));
				});
			}));
		}

		public override void Update(GameTime gameTime)
		{
			crowd.Location = new Vector2 (crowd.Location.X, crowd.Location.Y + upSpeed);

			upPassed += (float)gameTime.ElapsedGameTime.TotalSeconds;

			if (upPassed >= upTotal)
			{
				upSpeed *= -1;
				upPassed = 0;
			}

			base.Update(gameTime);
		}

		private Camera camera;
		private EffectManager effects;
		private Character boss;

		private float upPassed;
		private float upTotal = 0.25f;
		private float upSpeed = -1;

		private void SpawnGiftBox (string name, Vector2 location)
		{
			GameObjects.Add(new GameObject
			{
				Location = location,
				Texture = Engine.ContentManager.Load<Texture2D> ("Gifts\\" + name)
			});
		}
	}
}
