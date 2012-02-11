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
			effectManager = new EffectManager (this);
			cats = new List<NyanCat>();

			batch = Engine.Batch;
			catTexture = Engine.ContentManager.Load<Texture2D> ("NyanCat");

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
			
			foreach (GameObject entity in CurrentLevel.GameObjects)
				entity.Draw (batch);

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
		}
		
		private void HandleControls()
		{
			if (Engine.NewKeyboard.IsKeyDown (Keys.F1) && Engine.OldKeyboard.IsKeyUp (Keys.F1))
				editorModeEnabled = !editorModeEnabled;

			if (!editorModeEnabled && Engine.NewMouse.LeftButton == ButtonState.Pressed
				&& Engine.OldMouse.LeftButton == ButtonState.Released && !CurrentLevel.Player.isClimbing)
			{
				float offset = 0;
				if (CurrentLevel.Player.Direction == Directions.Left)
					offset -= 30;
				else
					offset += 20;
				
				SpawnCat (CurrentLevel.Player.Location + new Vector2 (offset, 0), CurrentLevel.Player.Direction);
			}
		}
	}
}
