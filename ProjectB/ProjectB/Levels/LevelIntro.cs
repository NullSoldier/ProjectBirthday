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
	public class LevelIntro
		: BaseLevel
	{
		public LevelIntro ()
			: base ("LevelIntro")
		{
			this.StartPoint = new Vector2(10, 200);
			this.SkyColor = Color.Transparent;
			this.UseClouds = false;

			AddGeometry (new Rectangle(0, 0, 479, 52), CollisionType.Impassable);
			AddGeometry (new Rectangle(-1, 211, 482, 31), CollisionType.Impassable);
			AddGeometry (new Rectangle(-57, -3, 56, 254), CollisionType.Impassable);
			AddGeometry (new Rectangle(477, 0, 21, 246), CollisionType.Impassable);

		}

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

			SpawnGiftBox ("Boxes", new Vector2(210, 145));

			boss = new Boss
			{
				Texture = Engine.ContentManager.Load<Texture2D> ("Boss"),
				Location = new Vector2(350, 80),
				Alpha = 0f,
				AcceptPhysicalInput = false
			};

			GameObjects.Add (Player);
			GameObjects.Add (boss);

			effects.Add ("Character", new CharacterMoveEffect(Player, 100, Directions.Right));
			effects.Add ("Character", new WaitEffect (2f, () =>
			{
			Engine.DialogRunner.EnqueueMessageBox ("Megan", "Why isn't anyone at my party?");
			Engine.DialogRunner.EnqueueMessageBox ("Megan", "Where is everyone?", () =>
			{
			effects.Add ("Character", new CharacterFadeEffect  (boss, 0f, 1f, 1f));
			effects.Add ("Character", new CharacterMoveEffect (Player, 50, Directions.Left, () =>
			{
			Engine.DialogRunner.EnqueueMessageBox ("Megusta", "Haha! I am the evil overlord Megusta!");
			Engine.DialogRunner.EnqueueMessageBox ("Megusta", "I've captured all of your friends!\nIf you want to get them back you'll have to defeat me!", () =>
			{
			effects.Add ("Character", new CharacterFadeEffect  (boss, 1f, 0f, 1f));
			effects.Add ("Character", new WaitEffect (2f, () =>
			{
			Engine.DialogRunner.EnqueueMessageBox ("Megan", "I'll save my friends! We'll have my birthday yet!", () =>
			{
				effects.Add ("Character", new CharacterMoveEffect (Player, 200, Directions.Right));
				effects.Add ("Character", new WaitEffect (2f, () => Engine.Project.NextLevel()));
			});
			}));
			});
			}));
			});
			}));
		}

		private Camera camera;
		private EffectManager effects;
		private Character boss;

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
