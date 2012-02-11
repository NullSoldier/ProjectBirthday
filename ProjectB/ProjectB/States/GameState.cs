using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using ProjectB.Objects;
using ProjectB.Scripts;

namespace ProjectB.States
{
	public partial class GameState
		: BaseState
	{
		public override void Activate ()
		{
			CurrentLevel.Start (this);
		}

		public override void Load ()
		{
			camera = new Camera (Engine.ScreenWidth, Engine.ScreenHeight);
			effectManager = new EffectManager (this);
			cats = new List<NyanCat>();

			batch = Engine.Batch;
			catTexture = Engine.ContentManager.Load<Texture2D> ("NyanCat");
			nyanSoundEffect = Engine.ContentManager.Load<SoundEffect> ("Sound/Nyan");
			
			nyanInstance = nyanSoundEffect.CreateInstance();
			nyanInstance.IsLooped = false;
			nyanInstance.Volume = 0.5f;
			nyanInstance.Stop();

			EditorLoad();
		}

		public override void Update (GameTime gameTime)
		{
			if (!Engine.IgnoreInput)
				HandleControls ();

			foreach (GameObject entity in CurrentLevel.GameObjects)
				entity.Update (gameTime);

			effectManager.Update (gameTime);

			if (CurrentLevel.UseClouds)
				cloudManager.Update (gameTime);

			HandleNyanCats (gameTime);
			HandleEnemyCollision (gameTime);

			if (editorModeEnabled)
				EditorUpdate (gameTime);

			CurrentLevel.Update (gameTime);

			foreach (var entity in CurrentLevel.GameObjects.ToArray())
				if (entity.MarkedForDeletion)
					CurrentLevel.GameObjects.Remove (entity);

			CheckPlayerDeath();

			// Fade Nyan sound
			if (!isNyanFading)
				nyanInstance.Volume = 1f;
			else
			{
				nyanFadePassed += (float)gameTime.ElapsedGameTime.TotalMilliseconds;
				nyanInstance.Volume = 1 - MathHelper.Clamp(nyanFadePassed / nyanFadeTotal, 0, 1);

				if (nyanInstance.Volume == 0f)
				{
					isNyanFading = false;
					nyanInstance.Stop();
				}
			}

			// Gun reload update
			if (!canFireGun)
			{
				gunTimePassed += (float)gameTime.ElapsedGameTime.TotalMilliseconds;
				if (gunTimePassed > gunTimeTotal)
					canFireGun = true;
			}

			if (!canTakedamage)
			{
				damageTimePassed += (float)gameTime.ElapsedGameTime.TotalMilliseconds;
				if (damageTimePassed > damageTimeTotal)
				{
					CurrentLevel.Player.Alpha = 1f;
					canTakedamage = true;
				}
			}
		}

		public override void Draw ()
		{
			Engine.Graphics.GraphicsDevice.Clear (CurrentLevel.SkyColor);

			batch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, SamplerState.PointWrap, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, camera.Transformation);
			
			if (CurrentLevel.UseClouds)
				cloudManager.Draw (batch);

			batch.Draw (CurrentLevel.Level.Texture, Vector2.Zero, Color.White);

			foreach (GameObject entity in CurrentLevel.GameObjects)
				entity.Draw (batch);

			foreach (NyanCat cat in cats)
				cat.Draw (batch);

			batch.End();

			batch.Begin();
			DrawHealthBar (batch);

			if (!canFireGun)
				DrawReloadBar (batch);

			batch.End();
			
			if (editorModeEnabled)
				EditorDraw();
		}
		
		public BaseLevel CurrentLevel
		{
			get { return Engine.Project.CurrentLevel; }
		}

		private SpriteBatch batch;
		public Camera camera;
		public CloudManager cloudManager;
		public EffectManager effectManager;
		private bool editorModeEnabled = false;
		public List<NyanCat> cats;
		private Texture2D catTexture;

		public void SpawnCat(Vector2 location, Directions direction)
		{
			NyanCat cat = new NyanCat (direction)
			{
				Texture = catTexture,
				Location = location
			};
			cats.Add (cat);

			nyanCount++;

			isNyanFading = false;
			nyanInstance.Volume = 1f;
			nyanInstance.Play();
		}
		
		private void HandleControls()
		{
			if (Engine.NewKeyboard.IsKeyDown (Keys.F1) && Engine.OldKeyboard.IsKeyUp (Keys.F1))
				editorModeEnabled = !editorModeEnabled;

			if (((Engine.NewMouse.LeftButton == ButtonState.Pressed
				&& Engine.OldMouse.LeftButton == ButtonState.Released)
				|| (Engine.NewPad.IsButtonDown(Buttons.X)
				&& Engine.OldPad.IsButtonUp(Buttons.X)))

				&& !CurrentLevel.Player.isClimbing
				&& canFireGun)
			{
				float offset = 0;
				if (CurrentLevel.Player.Direction == Directions.Left)
					offset -= 30;
				else
					offset += 20;
				
				SpawnCat (CurrentLevel.Player.Location + new Vector2 (offset, 0), CurrentLevel.Player.Direction);
				canFireGun = false;
				gunTimePassed = 0;
			}
			if (Engine.NewKeyboard.IsKeyDown (Keys.Z) && Engine.OldKeyboard.IsKeyUp (Keys.Z))
			{
				Rectangle playerBounds = CurrentLevel.Player.GetBounds();

				foreach (Chest chest in CurrentLevel.Chests)
				{
					if (!chest.GetBounds().Intersects(playerBounds))
						continue;

					chest.Open (this);
				}
			}
		}

		private void HandleEnemyCollision (GameTime gameTime)
		{
			if (!canTakedamage)
				return;

			var playerBounds = CurrentLevel.Player.GetBounds();

			foreach (var entity in CurrentLevel.GameObjects)
			{
				var enemy = entity as BaseMonster;
				if (enemy != null && playerBounds.Intersects (enemy.GetBounds()))
				{
					canTakedamage = false;
					damageTimePassed = 0;
					CurrentLevel.Player.Alpha = 0.5f;
					CurrentLevel.Player.Health -= enemy.Damage;

					CheckPlayerDeath();
				}
			}
		}

		private void CheckPlayerDeath ()
		{
			if (CurrentLevel.Player.IsAlive
				&& CurrentLevel.Player.Location.Y <= CurrentLevel.Level.Texture.Height + CurrentLevel.Player.GetBounds().Height)
				return;

			CurrentLevel.Player.Location = CurrentLevel.StartPoint;
			CurrentLevel.Player.Health = CurrentLevel.Player.MaxHealth;
			CurrentLevel.Player.Reset();
		}

		private void HandleNyanCats (GameTime gameTime)
		{
			foreach (NyanCat cat in cats.ToArray())
			{
				cat.Update (gameTime);

				foreach (var entity in CurrentLevel.GameObjects)
				{
					var enemy = entity as BaseMonster;
					if (enemy != null 
						&& enemy.GetBounds().Intersects(cat.GetBounds()))
					{
						enemy.Health -= NyanDamage;
 						cats.Remove(cat);

						if (!enemy.IsAlive)
						{
							enemy.IsActive = false;
							enemy.MarkedForDeletion = true;
						}

						KillNyanCat ();
					}

				}
				if (cat.Location.X < 0 || cat.Location.X > camera.Bounds.Width)
				{
					cats.Remove (cat);
					KillNyanCat();
				}
			}
		}

		private void KillNyanCat ()
		{
			nyanCount--;
							
			if (nyanCount <= 0)
			{
				isNyanFading = true;
				nyanFadePassed = 0;
			}
		}
		private int nyanCount = 0;
		private SoundEffect nyanSoundEffect;
		private SoundEffectInstance nyanInstance;
		private int HealthBarWidth = 300;
		private int HealthBarHeight = 20;
		private int NyanDamage = 30;
		private bool canFireGun = true;
		private bool canTakedamage = true;
		private int damageTimerDelay = 20;

		private float damageTimePassed;
		private float damageTimeTotal = 1100;
		private float gunTimePassed;
		private float gunTimeTotal = 500;

		private float nyanFadePassed;
		private float nyanFadeTotal = 500;
		private bool isNyanFading = false;

		private void DrawHealthBar (SpriteBatch spriteBatch)
		{
			var player = CurrentLevel.Player;
			float percentage = (float)player.Health / (float)player.MaxHealth;

			Rectangle destRectangle = new Rectangle (10, 10, HealthBarWidth, HealthBarHeight);
			Rectangle healthRectangle = new Rectangle (destRectangle.X, destRectangle.Y,
				(int)(destRectangle.Width  * percentage), destRectangle.Height);

			spriteBatch.Draw (Engine.BlankTexture, destRectangle, null, Color.DarkGray);
			spriteBatch.Draw (Engine.BlankTexture, healthRectangle, null, Color.Red);
		}

		private void DrawReloadBar (SpriteBatch spriteBatch)
		{
			float percentage = gunTimePassed / gunTimeTotal;

			Rectangle destRectangle = new Rectangle (10, Engine.ScreenHeight - HealthBarHeight - 10, Engine.ScreenWidth - 20, HealthBarHeight);
			Rectangle reloadRectangle = new Rectangle (destRectangle.X, destRectangle.Y,
				(int)(destRectangle.Width  * percentage), destRectangle.Height);

			spriteBatch.Draw (Engine.BlankTexture, destRectangle, null, Color.DarkGray);
			spriteBatch.Draw (Engine.BlankTexture, reloadRectangle, null, Color.Purple);
		}

		private void DamagedTimerDone (object state)
		{
			canTakedamage = true;
		}

		private void NyanFireTimerDone (object state)
		{
			canFireGun = true;
		}
	}
}
