using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
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
			camera.Bounds = new Rectangle(0, 0, CurrentLevel.Level.Texture.Width, CurrentLevel.Level.Texture.Height);
			camera.UseBounds = false;

			cloudManager = new CloudManager (camera.Bounds.Width, camera.Bounds.Height);
			effectManager = new EffectManager (this);
			cats = new List<NyanCat>();

			batch = Engine.Batch;
			catTexture = Engine.ContentManager.Load<Texture2D> ("NyanCat");

			EditorLoad();
		}

		public override void Update (GameTime gameTime)
		{
			HandleControls ();

			player.Update (gameTime);

			effectManager.Update (gameTime);

			if (CurrentLevel.UseClouds)
				cloudManager.Update (gameTime);

			foreach (NyanCat cat in cats)
				cat.Update (gameTime);

			if (editorModeEnabled)
				EditorUpdate (gameTime);

			CurrentLevel.Update (gameTime);
		}

		public override void Draw ()
		{
			Engine.Graphics.GraphicsDevice.Clear (CurrentLevel.SkyColor);

			batch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, SamplerState.PointWrap, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, camera.Transformation);
			
			if (CurrentLevel.UseClouds)
				cloudManager.Draw (batch);

			batch.Draw (CurrentLevel.Level.Texture, Vector2.Zero, Color.White);
			player.Draw (batch);

			foreach (NyanCat cat in cats)
				cat.Draw (batch);

			batch.End();
			
			if (editorModeEnabled)
				EditorDraw();
		}
		
		private BaseLevel CurrentLevel
		{
			get { return Engine.Project.CurrentLevel; }
		}

		private SpriteBatch batch;
		public Camera camera;
		public Player player;
		private CloudManager cloudManager;
		public EffectManager effectManager;
		private bool editorModeEnabled = false;
		private List<NyanCat> cats;
		private Texture2D catTexture;

		public void SpawnPlayer (Vector2 location)
		{
			player = new Player
			{
				Texture = Engine.ContentManager.Load<Texture2D> ("Player"),
				Location = location
			};
		}

		public void SpawnCat(Vector2 location, Directions direction)
		{
			NyanCat cat = new NyanCat (direction)
			{
				Texture = catTexture,
				Location = location
			};
			cats.Add (cat);
		}
		
		private void HandleControls()
		{
			if (Engine.NewKeyboard.IsKeyDown (Keys.F1) && Engine.OldKeyboard.IsKeyUp (Keys.F1))
				editorModeEnabled = !editorModeEnabled;

			if (!editorModeEnabled && Engine.NewMouse.LeftButton == ButtonState.Pressed
				&& Engine.OldMouse.LeftButton == ButtonState.Released)
			{
				float offset = 0;
				if (player.Direction == Directions.Right)
					offset += player.Texture.Width;
				else
					offset += 10;

				SpawnCat (player.Location + new Vector2 (offset, 0), player.Direction);
			}
		}
	}
}
