using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using ProjectB.Objects;

namespace ProjectB.States
{
	public partial class GameState
		: BaseState
	{
		public override void Activate ()
		{
		}

		public override void Load ()
		{
			camera = new Camera (ProjectB.ScreenWidth, ProjectB.ScreenHeight);
			camera.Bounds = new Rectangle(0, 0, CurrentLevel.Level.Texture.Width, CurrentLevel.Level.Texture.Height);
			camera.UseBounds = true;

			cloudManager = new CloudManager (camera.Bounds.Width, camera.Bounds.Height);
			cats = new List<NyanCat>();

			batch = ProjectB.Batch;
			clearColor = new Color(166, 211, 239);
			catTexture = ProjectB.ContentManager.Load<Texture2D> ("NyanCat");
			SpawnPlayer (CurrentLevel.StartPoint);

			EditorLoad();
		}

		public override void Update (GameTime gameTime)
		{
			HandleControls ();

			player.Update (gameTime);
			cloudManager.Update (gameTime);

			foreach (NyanCat cat in cats)
				cat.Update (gameTime);

			camera.CenterOnPoint (player.Location);

			if (editorModeEnabled)
				EditorUpdate (gameTime);
		}

		public override void Draw ()
		{
			ProjectB.Graphics.GraphicsDevice.Clear (clearColor);

			batch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, SamplerState.PointWrap, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, camera.Transformation);

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
			get { return ProjectB.Project.CurrentLevel; }
		}

		private SpriteBatch batch;
		private Camera camera;
		private Player player;
		private CloudManager cloudManager;
		private float playerSpeed = 0.3f;
		private bool editorModeEnabled = false;
		private Color clearColor;
		private List<NyanCat> cats;
		private Texture2D catTexture;
		private Vector2 gravity = new Vector2(0, 5);

		private void SpawnPlayer (Vector2 location)
		{
			player = new Player
			{
				Texture = ProjectB.ContentManager.Load<Texture2D> ("Player"),
				Location = location
			};
		}

		private void SpawnCat(Vector2 location, Directions direction)
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
			if (ProjectB.NewKeyboard.IsKeyDown (Keys.F1) && ProjectB.OldKeyboard.IsKeyUp (Keys.F1))
				editorModeEnabled = !editorModeEnabled;

			if (!editorModeEnabled && ProjectB.NewMouse.LeftButton == ButtonState.Pressed
				&& ProjectB.OldMouse.LeftButton == ButtonState.Released)
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
