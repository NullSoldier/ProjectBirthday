﻿using System;
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
			transitioning = false;
			FriendsCaptured = 0;
			CurrentLevel.Start (this);
		}

		public override void Load ()
		{
			camera = new Camera (Engine.ScreenWidth, Engine.ScreenHeight);
			effectManager = new EffectManager (this);
			cats = new List<NyanCat>();
			projectiles = new List<Projectile>();

			batch = Engine.Batch;
			catTexture = Engine.ContentManager.Load<Texture2D> ("NyanCat");
			deathTexture = Engine.ContentManager.Load<Texture2D> ("Death");
			healthBarTexture = Engine.ContentManager.Load<Texture2D> ("HealthBar");
			reloadBarTexture = Engine.ContentManager.Load<Texture2D> ("ReloadBar");
			nyanSoundEffect = Engine.ContentManager.Load<SoundEffect> ("Sound/Nyan");
			
			nyanInstance = nyanSoundEffect.CreateInstance();
			nyanInstance.IsLooped = false;
			nyanInstance.Volume = 0.5f;
			nyanInstance.Stop();

			deathFont = Engine.ContentManager.Load<SpriteFont> ("DeathFont");
			deathLocation = new Vector2 (HealthBarWidth + 20, 5);

			EditorLoad();
		}

		public override void Update (GameTime gameTime)
		{
			if (transitioning)
				return;

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
			CheckPlayerWin();

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

			var playerBounds = CurrentLevel.Player.GetBounds(); 

			foreach(Projectile P in projectiles.ToArray())
			{
				P.Update(gameTime);

				if (playerBounds.Intersects(P.GetBounds()))
				{
					CurrentLevel.Player.Health -= (int)P.Damage;
					projectiles.Remove (P);
				}

				if (P.Location.X < 0 - 20 || P.Location.X > camera.Bounds.Width || (P.Life != -1 && P.Life <= 0))
					projectiles.Remove (P);
			}
		}

		public override void Draw ()
		{
			Engine.Graphics.GraphicsDevice.Clear (CurrentLevel.SkyColor);

			batch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, SamplerState.PointWrap, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, camera.Transformation);
			
			if (CurrentLevel.UseClouds)
				cloudManager.Draw (batch);

			batch.Draw (CurrentLevel.Level.Texture, Vector2.Zero, Color.White);

			if (FriendsCaptured >= CurrentLevel.FriendCount && CurrentLevel.Exit != null)
				CurrentLevel.Exit.Draw (batch);

			foreach (GameObject entity in CurrentLevel.GameObjects)
				entity.Draw (batch);

			foreach (NyanCat cat in cats)
				cat.Draw (batch);

			foreach (Projectile p in projectiles)
				p.Draw (batch);

			batch.End();

			if (CurrentLevel.DisplayInterface)
			{
				batch.Begin (SpriteSortMode.Immediate, BlendState.AlphaBlend);
				DrawHealthBar (batch);

				if (!canFireGun)
					DrawReloadBar (batch);

				batch.Draw (deathTexture, deathLocation, null, Color.White);
				batch.DrawString (deathFont, deathCount.ToString (), deathLocation + new Vector2 (deathTexture.Width + 13, -3), Color.Black);

				batch.End();
			}

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
		public List<NyanCat> cats;
		private List<Projectile> projectiles;
		private Texture2D catTexture;
		private Texture2D deathTexture;
		private Texture2D healthBarTexture;
		private Texture2D reloadBarTexture;
		private SpriteFont deathFont;
		private bool transitioning;
		private bool editorModeEnabled = false;
		public int FriendsCaptured;
		private int deathCount = 0;
		private Vector2 deathLocation;

		public void SpawnProjectile (Vector2 location, Directions direction, float damage, float life)
		{
			var projectile = new Projectile(direction, location)
			{
				Damage = damage,
				Life = life
			};
			projectiles.Add(projectile);
		}

		public void CaptureFriend ()
		{
			FriendsCaptured++;

			if (FriendsCaptured >= CurrentLevel.FriendCount)
				CurrentLevel.Exit.IsActive = true;
		}

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

			if ((Engine.NewKeyboard.IsKeyDown (Keys.Enter) && Engine.OldKeyboard.IsKeyUp (Keys.Enter))
				|| (Engine.NewPad.IsButtonDown (Buttons.Y) && Engine.OldPad.IsButtonUp (Buttons.Y)))
			{
				Rectangle playerBounds = CurrentLevel.Player.GetBounds ();

				foreach (Chest chest in CurrentLevel.Chests)
				{
					if (!chest.IsActive || chest.Opened)
						continue;

					if (!chest.GetBounds ().Intersects (playerBounds))
						continue;

					chest.Open (this);
				}
			}

			if (((Engine.NewKeyboard.IsKeyDown (Keys.Space)
				&& Engine.OldKeyboard.IsKeyUp (Keys.Space))
				|| (Engine.NewPad.IsButtonDown(Buttons.X)
				&& Engine.OldPad.IsButtonUp(Buttons.X)))

				&& !CurrentLevel.Player.isClimbing
				&& canFireGun
				&& CurrentLevel.DisplayInterface)
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

			deathCount++;
		}

		private void CheckPlayerWin ()
		{
			if (CurrentLevel.Player == null || CurrentLevel.Exit == null)
				return;

			if (FriendsCaptured < CurrentLevel.FriendCount)
				return;

			if (CurrentLevel.Player.GetBounds().Intersects(CurrentLevel.Exit.GetBounds()))
			{
				Engine.Project.NextLevel();
				transitioning = true;
			}
		}

		private bool warnInvincible = false;

		private void HandleNyanCats (GameTime gameTime)
		{
			foreach (NyanCat cat in cats.ToArray())
			{
				cat.Update (gameTime);

				foreach (var entity in CurrentLevel.GameObjects)
				{
					var enemy = entity as BaseMonster;
					var turret = entity as SentryEnemy;

					if (enemy != null && enemy.GetBounds().Intersects(cat.GetBounds()))
					{
						if (turret != null && turret.Invincible)
						{
							if (!warnInvincible)
							{
								Engine.DialogRunner.ShowTimed ("Megan", "It looks like those golden\nturrets can't be destroyed.", 1.3f);
								warnInvincible = true;
							}

							continue;
						}

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
		private int HealthBarHeight = 40;
		private int ReloadBarHeight = 35;
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
			Rectangle healthRectangle = new Rectangle (destRectangle.X + 2, destRectangle.Y,
				(int)(destRectangle.Width * percentage) - 4, destRectangle.Height);

			spriteBatch.Draw (Engine.BlankTexture, healthRectangle, null, Color.Red);
			spriteBatch.Draw (healthBarTexture, destRectangle, null, Color.White);
		}

		private void DrawReloadBar (SpriteBatch spriteBatch)
		{
			float percentage = gunTimePassed / gunTimeTotal;

			Rectangle destRectangle = new Rectangle (10, Engine.ScreenHeight - HealthBarHeight - 10, Engine.ScreenWidth - 20, ReloadBarHeight);
			Rectangle reloadRectangle = new Rectangle (destRectangle.X + 2, destRectangle.Y,
				(int)(destRectangle.Width  * percentage) - 4, destRectangle.Height);

			spriteBatch.Draw (Engine.BlankTexture, reloadRectangle, null, Color.Purple);
			spriteBatch.Draw (reloadBarTexture, destRectangle, null, Color.White);
		}
	}
}
